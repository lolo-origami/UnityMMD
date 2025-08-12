using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class LightShaftPass : CustomPostProcessingPass<LightShaft>
{
    private static readonly int FlareVectorId = UnityEngine.Shader.PropertyToID("_FlareVector");
    private static readonly int FlareColorId = UnityEngine.Shader.PropertyToID("_FlareColor");
    private static readonly int ParaVectorId = UnityEngine.Shader.PropertyToID("_ParaVector");
    private static readonly int ParaColorId = UnityEngine.Shader.PropertyToID("_ParaColor");
    
    private static readonly int LightShaftTempId = UnityEngine.Shader.PropertyToID("_LightShaftTempTex");
    
    protected override string RenderTag => "LightShaft";

    public LightShaftPass(RenderPassEvent renderPassEvent, Shader shader) : base(renderPassEvent, shader)
    {
    }
    
    /// <summary>
    /// Textureとfloatの設定をマテリアルに送る
    /// </summary>
    /// <param name="commandBuffer"></param>
    /// <param name="renderingData"></param>
    protected override void BeforeRender(CommandBuffer commandBuffer, ref RenderingData renderingData)
    {
        ref var cameraData = ref renderingData.cameraData;
        var camera = cameraData.camera;
            
        Material.SetMatrix("_CamFrustum", FrustumCorners(camera));
        Material.SetMatrix("_CamToWorld", camera.cameraToWorldMatrix);
        Material.SetVector("_CamWorldSpace", camera.transform.position);
        Material.SetInt("_MaxIterations", Component.MaxIterations.value);
        Material.SetFloat("_MaxDistance", Component.MaxDistance.value);
        Material.SetFloat("_MinDistance", Component.MinDistance.value);
        Material.SetFloat("_Intensity", Component.Intensity.value);
    }

    protected override void Render(CommandBuffer commandBuffer, ref RenderingData renderingData, RenderTargetIdentifier source, RenderTargetIdentifier dest)
    {
        ref var cameraData = ref renderingData.cameraData;
            
        commandBuffer.GetTemporaryRT(LightShaftTempId,cameraData.camera.scaledPixelWidth / 4, cameraData.camera.scaledPixelHeight / 4);
            
        // LightShaft生成
        commandBuffer.Blit(null, LightShaftTempId, Material, 0);
        commandBuffer.SetGlobalTexture(LightShaftTempId, new RenderTargetIdentifier(LightShaftTempId));
        commandBuffer.Blit(source, dest, Material, 1);
            
        commandBuffer.ReleaseTemporaryRT(LightShaftTempId);
    }
    
    private Matrix4x4 FrustumCorners(Camera cam)
    {
        Transform camtr = cam.transform;

        Vector3[] frustumCorners = new Vector3[4];
        //四錐体のサイズを求める
        cam.CalculateFrustumCorners(new Rect(0, 0, 1, 1), cam.farClipPlane, cam.stereoActiveEye, frustumCorners);

        Matrix4x4 frustumVectorsArray = Matrix4x4.identity;
            
        frustumVectorsArray.SetRow(0,  camtr.TransformVector(frustumCorners[0]));
        frustumVectorsArray.SetRow(1, camtr.TransformVector(frustumCorners[3]));
        frustumVectorsArray.SetRow(2, camtr.TransformVector(frustumCorners[1]));
        frustumVectorsArray.SetRow(3, camtr.TransformVector(frustumCorners[2]));
            
        return frustumVectorsArray;
    }

    protected override bool IsActive()
    {
        return Component.IsActive;
    }
}
