using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using static RenderGraphPostProcessUtils; // RenderGraphPostProcessUtils.cs は別途作成してください

public class DepthFogPass : ScriptableRenderPass
{
    // プロファイラで表示するタグ名
    private const string APPLY_FOG_PASSNAME = "Apply Depth Fog Pass";
    private const string COPY_FOG_TO_SCREEN_PASSNAME = "Copy Fog To Screen Pass";

    // シェーダープロパティIDをここで定義
    //private static readonly int _rampTexId = Shader.PropertyToID("_RampTex");
    private static readonly int _intensityId = Shader.PropertyToID("_Intensity");
    private static readonly int _fogColorId = Shader.PropertyToID("_FogColor");
    private static readonly int _cameraMainTextureId = Shader.PropertyToID("_MainTex");    
    //private static readonly int _cameraDepthTextureId = Shader.PropertyToID("_CameraDepthTexture");

    private class PassData
    {
        public TextureHandle srcTextureHandle;
        //public TextureHandle depthTextureHandle;

        public Material depthFogMaterial;
        public ClampedFloatParameter intensity;
        public ColorParameter fogColor;
        //public TextureParameter rampTexture;
    }

    private Material _depthFogMaterial;
    private Shader _depthFogShader;

    public Material DepthFogMaterial => _depthFogMaterial;
    private int _index;
    private int _maxIndex;

    public DepthFogPass(RenderPassEvent renderPassEvent, Shader shader)
    {
        this.renderPassEvent = renderPassEvent;
        if (shader != null)
        {
            _depthFogMaterial = CoreUtils.CreateEngineMaterial(shader);
        }
    }

    public void SetFrameOrder(int index, int maxIndex)
    {
        _index = index; _maxIndex = maxIndex;
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (_depthFogMaterial == null || _index < 0)
        {
            return;
        }
        
        // 1:カメラのカラーバッファの内容にフルスクリーンエフェクトを適⽤して⼀時的なレンダーテクスチャーに書き込み
        // 2:⼀時的なレンダーテクスチャーの内容を⼀時的なレンダーテクスチャーにコピー
        var resourceData = frameData.Get<UniversalResourceData>();
        var cameraData = frameData.Get<UniversalCameraData>();
        var volumeStack = VolumeManager.instance.stack;
        var depthFogComponent = volumeStack.GetComponent<DepthFog>();

        if (!cameraData.postProcessEnabled || depthFogComponent == null || !depthFogComponent.active)
        {
            return;
        }
        
        //カメラカラーバッファのテクスチャハンドル
        //TextureHandle cameraColorTextureHandle = resourceData.activeColorTexture;
        RenderTextureDescriptor descriptor = cameraData.cameraTargetDescriptor;
        descriptor.msaaSamples = 1;
        descriptor.depthBufferBits = 0;
        var cameraDepthTextureHandle = resourceData.activeDepthTexture;
        
        //1:⼀時的なレンダーテクスチャ
        TextureHandle srcTextureHandle = GetTemporaryTexture(frameData, _index, resourceData);
        TextureHandle dstTextureHandle = CreateTemporaryTexture(renderGraph, frameData, srcTextureHandle, _index, "DepthFog");

        
        //カメラのカラーバッファをマテリアルを適⽤しながら⼀時的なレンダーテクスチャーに書き込む
        using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass(APPLY_FOG_PASSNAME, out PassData passData, profilingSampler))
        {
            builder.UseTexture(srcTextureHandle, AccessFlags.Read);//src
            builder.UseTexture(cameraDepthTextureHandle, AccessFlags.Read);
            builder.SetRenderAttachment(dstTextureHandle, 0, AccessFlags.Write);//dest
            passData.srcTextureHandle = srcTextureHandle;
            passData.depthFogMaterial = _depthFogMaterial;
            //passData.depthTextureHandle = cameraDepthTextureHandle;
            passData.intensity = depthFogComponent.intensity;
            passData.fogColor = depthFogComponent.fogColor;
            //passData.rampTexture = depthFogComponent.rampTexture;

            builder.SetRenderFunc((PassData passData, RasterGraphContext graphContext) =>
            {
                //passData.depthFogMaterial.SetTexture(_rampTexId, passData.rampTexture.value);
                passData.depthFogMaterial.SetFloat(_intensityId, passData.intensity.value);
                passData.depthFogMaterial.SetColor(_fogColorId, passData.fogColor.value);
                passData.depthFogMaterial.SetTexture(_cameraMainTextureId, passData.srcTextureHandle
                );
                //passData.depthFogMaterial.SetTexture(_cameraDepthTextureId, passData.depthTextureHandle);
                ExecutePass(passData.srcTextureHandle, passData.depthFogMaterial, graphContext);
            });
        }
        
        //最後ならカメラにコピー
        if (_index == _maxIndex)
        {
            using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass(COPY_FOG_TO_SCREEN_PASSNAME, out PassData passData, profilingSampler))
            {
                builder.UseTexture(dstTextureHandle, AccessFlags.Read);//src
                builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);//dest
                passData.srcTextureHandle = dstTextureHandle;
                passData.depthFogMaterial = null;
                builder.SetRenderFunc((PassData passData, RasterGraphContext graphContext) =>
                {
                    ExecutePass(passData.srcTextureHandle, null, graphContext);
                });
            }
        }     
      
    }
    
}