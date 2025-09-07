using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using static RenderGraphPostProcessUtils;

public class DiffusionPass : ScriptableRenderPass
{
    private static readonly int blurTexId = UnityEngine.Shader.PropertyToID("_BlurTex");
    private static readonly int contrastId = UnityEngine.Shader.PropertyToID("_Contrast");
    private static readonly int intensityId = UnityEngine.Shader.PropertyToID("_Intensity");
        
    private readonly Material _diffusionMaterial;
    public Material DiffusionMaterial => _diffusionMaterial;
    private int _index;
    private int _maxIndex;

    // Passのデータを持つクラス
    private class PassData
    {
        public Material material;
        public TextureHandle srcTextureHandle;
        public TextureHandle blurBuffer1;
        public TextureHandle blurBuffer2;
        public float contrast;
        public float intensity;
    }
    
    public DiffusionPass(RenderPassEvent renderPassEvent, Shader shader)
    {
        this.renderPassEvent = renderPassEvent;
        if (shader != null) _diffusionMaterial = CoreUtils.CreateEngineMaterial(shader);
    }
    
    public void SetFrameOrder(int index, int maxIndex)
    {
        _index = index;
        _maxIndex = maxIndex;
    }
    
    public  override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (_diffusionMaterial == null || _index < 0)
        {
            return;
        }

        var resourceData = frameData.Get<UniversalResourceData>();
        var cameraData = frameData.Get<UniversalCameraData>();
        var volumeStack = VolumeManager.instance.stack;
        var comp = volumeStack.GetComponent<Diffusion>();

        // 有効でない場合は何もしない
        if (!cameraData.postProcessEnabled || comp == null || !comp.IsActive)
        {
            return;
        }

        // 入力テクスチャと出力テクスチャを決定
        TextureHandle srcTextureHandle = GetTemporaryTexture(frameData, _index, resourceData);
        TextureHandle dstTextureHandle = CreateTemporaryTexture(renderGraph, frameData, srcTextureHandle, _index, "Diffusion");

        var descHalf = renderGraph.GetTextureDesc(srcTextureHandle);
        descHalf.name = "Temp_DiffusionBlurBuffer";
        descHalf.width /= 2;
        descHalf.height /= 2;
        descHalf.msaaSamples = MSAASamples.None; // MSAA 無効
        descHalf.depthBufferBits = 0; // 深度バッファ無効

        // ぼかし用の一時テクスチャを作成
        TextureHandle blurBuffer1 = renderGraph.CreateTexture(descHalf);
        TextureHandle blurBuffer2 = renderGraph.CreateTexture(descHalf);

        // 1. コントラスト調整パス
        using (var builder = renderGraph.AddRasterRenderPass("Diffusion: Contrast", out PassData passData))
        {
            builder.UseTexture(srcTextureHandle, AccessFlags.Read);
            builder.SetRenderAttachment(blurBuffer1, 0, AccessFlags.Write);
            
            passData.material = _diffusionMaterial;
            passData.srcTextureHandle = srcTextureHandle;
            passData.contrast = comp.Contrast.value;

            builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
            {
                //data.material.SetTexture(_cameraMainTextureId, passData.srcTextureHandle);
                data.material.SetFloat(contrastId, data.contrast);
                ExecutePass(data.srcTextureHandle, data.material, context, 0);
            });
        }
        
        // 2. ブラー1パス
        using (var builder = renderGraph.AddRasterRenderPass("Diffusion: Blur1", out PassData passData))
        {
            builder.UseTexture(blurBuffer1, AccessFlags.Read);
            builder.SetRenderAttachment(blurBuffer2, 0, AccessFlags.Write);
            
            passData.material = _diffusionMaterial;
            passData.srcTextureHandle = blurBuffer1;

            builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
            {
                ExecutePass(data.srcTextureHandle, data.material, context, 1);
            });
        }
        
        // 3. ブラー2パス
        using (var builder = renderGraph.AddRasterRenderPass("Diffusion: Blur2", out PassData passData))
        {
            builder.UseTexture(blurBuffer2, AccessFlags.Read);
            builder.SetRenderAttachment(blurBuffer1, 0, AccessFlags.Write);
            
            passData.material = _diffusionMaterial;
            passData.srcTextureHandle = blurBuffer2;

            builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
            {
                ExecutePass(data.srcTextureHandle, data.material, context, 2);
            });
        }

        // 4. 合成パス
        using (var builder = renderGraph.AddRasterRenderPass("Diffusion: Composite", out PassData passData))
        {
            builder.UseTexture(srcTextureHandle, AccessFlags.Read);
            builder.UseTexture(blurBuffer1, AccessFlags.Read);
            builder.SetRenderAttachment(dstTextureHandle, 0, AccessFlags.Write);

            passData.material = _diffusionMaterial;
            passData.srcTextureHandle = srcTextureHandle;
            passData.blurBuffer1 = blurBuffer1;
            passData.intensity = comp.Intensity.value;
            
            builder.SetRenderFunc((PassData data, RasterGraphContext context) =>
            {
                data.material.SetFloat(intensityId, data.intensity);
                data.material.SetTexture(blurTexId, data.blurBuffer1);
                ExecutePass(data.srcTextureHandle, data.material, context, 3);
            });
        }

        // 最後のパスなら、カメラバッファに書き込む
        if (_index == _maxIndex)
        {
            using (var builder = renderGraph.AddRasterRenderPass("Final Copy Pass (Diffusion)", out PassData pd))
            {
                builder.UseTexture(dstTextureHandle, AccessFlags.Read);
                builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);
                pd.srcTextureHandle = dstTextureHandle;
                pd.material = null; // コピーなのでマテリアルは不要
                builder.SetRenderFunc((PassData data, RasterGraphContext ctx) => ExecutePass(data.srcTextureHandle, null, ctx));
            }
        }
    }
}
