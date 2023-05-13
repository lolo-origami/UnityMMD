using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DepthFogPass : CustomPostProcessingPass<DepthFog>
{
    private static readonly int _rampTexId = UnityEngine.Shader.PropertyToID("_RampTex");
    private static readonly int _intensityId = UnityEngine.Shader.PropertyToID("_Intensity");
    private static readonly int _fogColor = UnityEngine.Shader.PropertyToID("_FogColor");
    protected override string RenderTag => "DepthFog";

    protected override void BeforeRender(CommandBuffer commandBuffer, ref RenderingData renderingData)
    {
        Material.SetTexture(_rampTexId, Component.rampTexture.value);
        Material.SetFloat(_intensityId, Component.intensity.value);
        Material.SetColor(_fogColor, Component.fogColor.value);
    }

    protected override bool IsActive()
    {
        return Component.IsActive;
    }

    public DepthFogPass(RenderPassEvent renderPassEvent, Shader shader) : base(renderPassEvent, shader)
    {
    }
}