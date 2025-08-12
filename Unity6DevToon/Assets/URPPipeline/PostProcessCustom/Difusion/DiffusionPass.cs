using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DiffusionPass : CustomPostProcessingPass<Diffusion>
{
    private static readonly int tempBlurBuffer1 = UnityEngine.Shader.PropertyToID("_TempBlurBuffer1");
    private static readonly int tempBlurBuffer2 = UnityEngine.Shader.PropertyToID("_TempBlurBuffer2");
        
    private static readonly int blurTexId = UnityEngine.Shader.PropertyToID("_BlurTex");
    private static readonly int contrastId = UnityEngine.Shader.PropertyToID("_Contrast");
    private static readonly int intensityId = UnityEngine.Shader.PropertyToID("_Intensity");
        
    protected override string RenderTag => "Diffusion";

    public DiffusionPass(RenderPassEvent renderPassEvent, Shader shader) : base(renderPassEvent, shader)
    {
    }
    
    /// <summary>
    /// Textureとfloatの設定をマテリアルに送る
    /// </summary>
    /// <param name="commandBuffer"></param>
    /// <param name="renderingData"></param>
    protected override void BeforeRender(CommandBuffer commandBuffer, ref RenderingData renderingData)
    {
        Material.SetFloat(contrastId, Component.Contrast.value);
        Material.SetFloat(intensityId, Component.Intensity.value);
    }

    protected override void Render(CommandBuffer commandBuffer, ref RenderingData renderingData, RenderTargetIdentifier source, RenderTargetIdentifier dest)
    {
        ref var cameraData = ref renderingData.cameraData;
        
        //シェーダーでぼかして縮小したバッファをコピーしていく
        commandBuffer.GetTemporaryRT(tempBlurBuffer1, cameraData.camera.scaledPixelWidth / 2, cameraData.camera.scaledPixelHeight / 2);
        commandBuffer.GetTemporaryRT(tempBlurBuffer2, cameraData.camera.scaledPixelWidth / 2, cameraData.camera.scaledPixelHeight / 2);
        
        //コントラスト調整
        commandBuffer.Blit(source, tempBlurBuffer1, Material, 0);
        //ブラー1
        commandBuffer.Blit(tempBlurBuffer1, tempBlurBuffer2, Material, 1);
        //ブラー2
        commandBuffer.Blit(tempBlurBuffer2, tempBlurBuffer1, Material, 2);
        
        //合成
        commandBuffer.SetGlobalTexture(blurTexId, tempBlurBuffer1);
        commandBuffer.Blit(source, dest, Material, 3);
            
        commandBuffer.ReleaseTemporaryRT(tempBlurBuffer1);
        commandBuffer.ReleaseTemporaryRT(tempBlurBuffer2);
    }

    protected override bool IsActive()
    {
        return Component.IsActive;
    }
}
