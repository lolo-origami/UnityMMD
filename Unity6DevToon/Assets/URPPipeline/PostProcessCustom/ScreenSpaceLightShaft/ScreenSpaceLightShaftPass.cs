using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using static LLPostProcessBufferManager; 

public class ScreenSpaceLightShaftPass : LLPostProcessPassBase
{
    private static readonly int LightShaftTempId = UnityEngine.Shader.PropertyToID("_LightShaftTempTex");
    private static readonly int CamToWorldId = Shader.PropertyToID("_CamToWorld");
    private static readonly int MaxIterationsId = Shader.PropertyToID("_MaxIterations");
    private static readonly int MaxDistanceId = Shader.PropertyToID("_MaxDistance");
    private static readonly int MinDistanceId = Shader.PropertyToID("_MinDistance");
    private static readonly int IntensityId = Shader.PropertyToID("_Intensity");
    private static readonly int RayColorId        = Shader.PropertyToID("_RayColor");
    private static readonly int DecayId           = Shader.PropertyToID("_Decay");
    private static readonly int RaySpreadId       = Shader.PropertyToID("_RaySpread");
    private static readonly int JitterStrengthId  = Shader.PropertyToID("_JitterStrength");
    private static readonly int NoiseScaleId      = Shader.PropertyToID("_NoiseScale");
    private static readonly int BlueNoiseTexId    = Shader.PropertyToID("_BlueNoiseTex");
    private static readonly int FalloffPowerId      = Shader.PropertyToID("_FalloffPower");
    private static readonly int OcclusionStrengthId   = Shader.PropertyToID("_OcclusionStrength");
    
    private class PassData
    {
        public Material material;
        public TextureHandle SrcTextureHandle;
        public TextureHandle ShaftTextureHandle;
        public Matrix4x4 camToWorld;
        public int maxIterations;
        public float maxDistance;
        public float minDistance;
        public float intensity;
        public Color rayColor;
        public float decay;
        public float raySpread;
        public float jitterStrength;
        public float noiseScale;
        public Texture blueNoiseTex;
        public float falloffPower;
        public float occlusionStrength;
    }

    private Material _lightShaftMaterial;
    public Material LightShaftMaterial => _lightShaftMaterial;
    private Shader _lightShaftShader;

    public ScreenSpaceLightShaftPass(RenderPassEvent renderPassEvent, Shader shader)
    {
        this.renderPassEvent = renderPassEvent;
        if (shader != null)
        {
            _lightShaftMaterial = CoreUtils.CreateEngineMaterial(shader);
        }
    }

    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (_lightShaftMaterial == null)
        {
            return;
        }

        // 1:カメラのカラーバッファの内容にフルスクリーンエフェクトを適⽤して⼀時的なレンダーテクスチャーに書き込み
        // 2:⼀時的なレンダーテクスチャーの内容を⼀時的なレンダーテクスチャーにコピー
        var resourceData = frameData.Get<UniversalResourceData>();
        var cameraData = frameData.Get<UniversalCameraData>();
        var volumeStack = VolumeManager.instance.stack;
        var component = volumeStack.GetComponent<LightShaft>();

        if (!cameraData.postProcessEnabled || component == null || !component.active)
        {
            return;
        }

        // 一時的なレンダーテクスチャを確保
        TextureHandle srcTextureHandle = GetSrcHandle(frameData, resourceData);

        // 中間（SunShaft のみを書き出す一時RT）
        var desc = renderGraph.GetTextureDesc(srcTextureHandle);
        desc.name = "LightShaftTemp";
        desc.clearBuffer = false;
        desc.msaaSamples = MSAASamples.None;
        TextureHandle shaftTexHandle = renderGraph.CreateTexture(desc);
        
        TextureHandle dstTextureHandle = GetDstHandle(renderGraph, frameData, srcTextureHandle);
        
        // -------------------------
        // Pass 0: LightShaftのレイマーチ
        // -------------------------
        using (var builder = renderGraph.AddRasterRenderPass("Light Shaft Pass", out PassData passData))
        {
            builder.UseTexture(srcTextureHandle, AccessFlags.Read);
            builder.UseTexture(resourceData.activeDepthTexture, AccessFlags.Read);
            builder.SetRenderAttachment(shaftTexHandle, 0, AccessFlags.Write);
            
            passData.SrcTextureHandle = srcTextureHandle;
            passData.ShaftTextureHandle = shaftTexHandle;
            passData.material = _lightShaftMaterial;
            
            var cam = cameraData.camera;
            passData.SrcTextureHandle = srcTextureHandle;
            passData.camToWorld = cam.cameraToWorldMatrix;
            passData.maxIterations = component.MaxIterations.value;
            passData.maxDistance = component.MaxDistance.value;
            passData.minDistance = component.MinDistance.value;
            passData.intensity = component.Intensity.value;
            passData.rayColor        = component.RayColor.value;
            passData.decay           = component.Decay.value;
            passData.raySpread       = component.RaySpread.value;
            passData.jitterStrength  = component.JitterStrength.value;
            passData.noiseScale      = component.NoiseScale.value;
            passData.blueNoiseTex    = component.BlueNoiseTex.value;         
            passData.falloffPower   = component.FalloffPower.value;
            passData.occlusionStrength = component.OcclusionStrength.value;

            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
            {
                var material = data.material;
                material.SetMatrix(CamToWorldId, data.camToWorld);
                material.SetInt(MaxIterationsId, data.maxIterations);
                material.SetFloat(MaxDistanceId, data.maxDistance);
                material.SetFloat(MinDistanceId, data.minDistance);
                material.SetColor(RayColorId, data.rayColor);
                material.SetFloat(DecayId, data.decay);
                material.SetFloat(RaySpreadId, data.raySpread);
                material.SetFloat(JitterStrengthId, data.jitterStrength);
                material.SetFloat(NoiseScaleId, data.noiseScale);
                material.SetFloat(FalloffPowerId, data.falloffPower);
                material.SetFloat(OcclusionStrengthId, data.occlusionStrength);
                if (data.blueNoiseTex != null)
                {
                    material.SetTexture(BlueNoiseTexId, data.blueNoiseTex);
                }

                ExecutePass(data.SrcTextureHandle, material, ctx, 0);
            });
        }
        
        // -------------------------
        // Pass 1: Combine（元絵 + shaftTex を合成して dst に出力）
        // -------------------------
        using (var builder = renderGraph.AddRasterRenderPass("Light Shaft - Combine", out PassData pass1))
        {
            builder.UseTexture(srcTextureHandle, AccessFlags.Read); // _BlitTexture 用
            builder.UseTexture(shaftTexHandle, AccessFlags.Read); // _LightShaftTempTex 用
            builder.SetRenderAttachment(dstTextureHandle, 0, AccessFlags.Write);


            pass1.SrcTextureHandle = srcTextureHandle; // 元絵
            pass1.ShaftTextureHandle = shaftTexHandle; // SunShaft 出力
            pass1.material = _lightShaftMaterial;
            pass1.intensity = component.Intensity.value; // 必要なら Combine でも使用


            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
            {
                var material = data.material;
                    
                // Combine パスで参照される一時テクスチャをバインド
                material.SetTexture(LightShaftTempId, data.ShaftTextureHandle);
                material.SetFloat(IntensityId, data.intensity);
                ExecutePass(data.SrcTextureHandle, material, ctx, 1); // pass 1: Combine
            });
        }  
    }
}
