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

    public override void Create()
    {
        AddIndex();
        this.name = "DepthFog";
        _pass = new DepthFogPass(settings.renderPassEvent, settings.shader, _index);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(_pass);
    }

    protected override void Dispose(bool disposing)
    {
        DecIndex();
        CoreUtils.Destroy(_pass?.DepthFogMaterial);
    }
}