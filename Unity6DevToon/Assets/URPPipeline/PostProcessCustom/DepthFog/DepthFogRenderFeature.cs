using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public class DepthFogRenderFeature : CustomPostProcessRFBase
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }
    
    public Settings settings = new Settings();
    private DepthFogPass _pass;


    protected override void OnCreate()
    {
        _pass = new DepthFogPass(settings.renderPassEvent, settings.shader);
    }


    public override bool IsActiveThisFrame(ref RenderingData renderingData)
    {
        if (!renderingData.cameraData.postProcessEnabled) return false;
        var comp = VolumeManager.instance.stack.GetComponent<DepthFog>();
        return comp != null && comp.IsActive;
    }


    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (!IsActiveThisFrame(ref renderingData)) return;
        int idx = FeatureIndex();
        var (first, last) = CustomPostProcessManager.Instance.GetLastActiveIndexThisFrame(ref renderingData);
        if (_pass != null) _pass.SetFrameOrder(idx, last);
        renderer.EnqueuePass(_pass);
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        CoreUtils.Destroy(_pass?.DepthFogMaterial);
    }
}