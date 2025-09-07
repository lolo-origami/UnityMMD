using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FlareRenderFeature : CustomPostProcessRFBase
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
    }

    public Settings settings = new Settings();
    private FlarePass _pass;

    protected override void OnCreate()
    {
        this.name = "Flare";
        _pass = new FlarePass(settings.renderPassEvent, settings.shader);
    }

    public override bool IsActiveThisFrame(ref RenderingData renderingData)
    {
        if (!renderingData.cameraData.postProcessEnabled) return false;
        var comp = VolumeManager.instance.stack.GetComponent<Flare>();
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
        CoreUtils.Destroy(_pass?.FlareMaterial);
    }
}