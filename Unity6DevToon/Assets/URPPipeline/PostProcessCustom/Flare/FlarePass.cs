using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class FlarePass : CustomPostProcessingPass<Flare>
{
    private static readonly int FlareVectorId = UnityEngine.Shader.PropertyToID("_FlareVector");
    private static readonly int FlareColorId = UnityEngine.Shader.PropertyToID("_FlareColor");
    private static readonly int ParaVectorId = UnityEngine.Shader.PropertyToID("_ParaVector");
    private static readonly int ParaColorId = UnityEngine.Shader.PropertyToID("_ParaColor");
        
    protected override string RenderTag => "Flare";

    public FlarePass(RenderPassEvent renderPassEvent, Shader shader) : base(renderPassEvent, shader)
    {
    }
    
    /// <summary>
    /// Textureとfloatの設定をマテリアルに送る
    /// </summary>
    /// <param name="commandBuffer"></param>
    /// <param name="renderingData"></param>
    protected override void BeforeRender(CommandBuffer commandBuffer, ref RenderingData renderingData)
    {
        Material.SetVector(FlareVectorId, new Vector4(Component.flarePosition.value.x, Component.flarePosition.value.y, 1f / Component.flareSize.value));
        Material.SetColor(FlareColorId, Component.flareColor.value);
        Material.SetVector(ParaVectorId, new Vector4(Component.paraPosition.value.x, Component.paraPosition.value.y, 1f / Component.paraSize.value));
        Material.SetColor(ParaColorId, Component.paraColor.value);
    }

    protected override bool IsActive()
    {
        return Component.IsActive;
    }
}
