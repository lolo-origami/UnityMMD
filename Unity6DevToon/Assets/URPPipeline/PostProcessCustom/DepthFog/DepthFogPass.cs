using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using static RenderGraphPostProcessUtils; // RenderGraphPostProcessUtils.cs は別途作成してください

public class DepthFogPass : ScriptableRenderPass
{
    // プロファイラで表示するタグ名
    private const string ApplyFogPassName = "Apply Depth Fog Pass";
    private const string CopyToScreenPassName = "Copy Fog To Screen Pass";

    // シェーダープロパティIDをここで定義
    private static readonly int _rampTexId = Shader.PropertyToID("_RampTex");
    private static readonly int _intensityId = Shader.PropertyToID("_Intensity");
    private static readonly int _fogColorId = Shader.PropertyToID("_FogColor");
    private static readonly int _cameraDepthTextureId = Shader.PropertyToID("_CameraDepthTexture");

    private class PassData
    {
        public TextureHandle srcTextureHandle;
        public TextureHandle depthTextureHandle;

        public Material depthFogMaterial;
        public ClampedFloatParameter intensity;
        public ColorParameter fogColor;
        public TextureParameter rampTexture;
    }

    private Material _depthFogMaterial;
    private Shader _depthFogShader;

    public Material DepthFogMaterial => _depthFogMaterial;

    public DepthFogPass(RenderPassEvent renderPassEvent, Shader shader)
    {
        this.renderPassEvent = renderPassEvent;
        _depthFogShader = shader;

        if (_depthFogShader != null)
        {
            _depthFogMaterial = CoreUtils.CreateEngineMaterial(_depthFogShader);
        }
    }
    
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (_depthFogMaterial == null)
        {
            return;
        }

        var resourceData = frameData.Get<UniversalResourceData>();
        var cameraData = frameData.Get<UniversalCameraData>();

        var volumeStack = VolumeManager.instance.stack;
        var depthFogComponent = volumeStack.GetComponent<DepthFog>();

        if (!cameraData.postProcessEnabled || depthFogComponent == null || !depthFogComponent.active)
        {
            return;
        }

        var cameraColorTextureHandle = resourceData.activeColorTexture;
        var cameraDepthTextureHandle = resourceData.activeDepthTexture;

        int w = cameraData.scaledWidth;
        int h = cameraData.scaledHeight;
        var tempFogRT = UniversalRenderer.CreateRenderGraphTexture(
            renderGraph,
            new RenderTextureDescriptor(w, h),
            "_TempDepthFogRT",
            true
        );

        using (var builder = renderGraph.AddRasterRenderPass<PassData>(ApplyFogPassName, out PassData passDataApplyFog))
        {
            builder.UseTexture(cameraColorTextureHandle, AccessFlags.Read);
            builder.UseTexture(cameraDepthTextureHandle, AccessFlags.Read);
            builder.SetRenderAttachment(tempFogRT, 0, AccessFlags.Write);
            

            passDataApplyFog.srcTextureHandle = cameraColorTextureHandle;
            passDataApplyFog.depthTextureHandle = cameraDepthTextureHandle;
            passDataApplyFog.depthFogMaterial = _depthFogMaterial;
            passDataApplyFog.intensity = depthFogComponent.intensity;
            passDataApplyFog.fogColor = depthFogComponent.fogColor;
            passDataApplyFog.rampTexture = depthFogComponent.rampTexture;

            builder.SetRenderFunc(
                (PassData data, RasterGraphContext context) => ExecuteApplyFogPass(data, context)
            );
        }
        
        using (var builder = renderGraph.AddRasterRenderPass<PassData>(CopyToScreenPassName, out PassData passDataCopyToScreen))
        {
            builder.UseTexture(tempFogRT, AccessFlags.Read);
            builder.SetRenderAttachment(cameraColorTextureHandle, 0, AccessFlags.Write);

            passDataCopyToScreen.srcTextureHandle = tempFogRT;
            passDataCopyToScreen.depthFogMaterial = null;

            builder.SetRenderFunc(
                (PassData data, RasterGraphContext context) =>
                {
                    BlitToCurrentRenderTarget(context.cmd, tempFogRT, CopyToScreenPassName);
                }
            );
        }        
    }
    
    private static void ExecuteApplyFogPass(PassData passData, RasterGraphContext graphContext)
    {
        RasterCommandBuffer cmd = graphContext.cmd;

        passData.depthFogMaterial.SetTexture(_rampTexId, passData.rampTexture.value);
        passData.depthFogMaterial.SetFloat(_intensityId, passData.intensity.value);
        passData.depthFogMaterial.SetColor(_fogColorId, passData.fogColor.value);
        passData.depthFogMaterial.SetTexture(_cameraDepthTextureId, passData.depthTextureHandle);

        Blitter.BlitTexture(cmd, passData.srcTextureHandle, new Vector4(1, 1, 0, 0), passData.depthFogMaterial, 0);
    }    
    
}