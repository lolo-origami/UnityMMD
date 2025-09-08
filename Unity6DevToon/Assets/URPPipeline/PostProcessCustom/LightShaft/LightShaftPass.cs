using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using static LLPostProcessBufferManager; 

public class LightShaftPass : LLPostProcessPassBase
{
    private static readonly int LightShaftTempId = UnityEngine.Shader.PropertyToID("_LightShaftTempTex");
    
    private class PassData
    {
        public Material material;
        public TextureHandle SrcTextureHandle;
        public Vector3 camWorldSpace;
        public Matrix4x4 frustum;
        public Matrix4x4 camToWorld;
        public int maxIterations;
        public float maxDistance;
        public float minDistance;
        public float intensity;
    }

    private Material _lightShaftMaterial;
    public Material LightShaftMaterial => _lightShaftMaterial;
    private Shader _lightShaftShader;

    public LightShaftPass(RenderPassEvent renderPassEvent, Shader shader)
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
        TextureHandle dstTextureHandle = GetDstHandle(renderGraph, frameData, srcTextureHandle);
        
        // Pass 実行
        using (var builder = renderGraph.AddRasterRenderPass("Light Shaft Pass", out PassData passData))
        {
            builder.UseTexture(srcTextureHandle, AccessFlags.Read);
            builder.UseTexture(resourceData.activeDepthTexture, AccessFlags.Read);
            builder.SetRenderAttachment(dstTextureHandle, 0, AccessFlags.Write);
            
            passData.SrcTextureHandle = srcTextureHandle;
            passData.material = _lightShaftMaterial;
            var cam = cameraData.camera;
            passData.frustum = FrustumCorners(cam);
            passData.camToWorld = cam.cameraToWorldMatrix;
            passData.camWorldSpace = cam.transform.position;
            passData.maxIterations = component.MaxIterations.value;
            passData.maxDistance = component.MaxDistance.value;
            passData.minDistance = component.MinDistance.value;
            passData.intensity = component.Intensity.value;


            builder.SetRenderFunc((PassData data, RasterGraphContext ctx) =>
            {
                var material = data.material;
                material.SetMatrix("_CamFrustum", data.frustum);
                material.SetMatrix("_CamToWorld", data.camToWorld);
                material.SetVector("_CamWorldSpace", data.camWorldSpace);
                material.SetInt("_MaxIterations", data.maxIterations);
                material.SetFloat("_MaxDistance", data.maxDistance);
                material.SetFloat("_MinDistance", data.minDistance);
                material.SetFloat("_Intensity", data.intensity);
                ExecutePass(data.SrcTextureHandle, material, ctx, 0);
            });
        }

        /*if (_isLast)
        {
            using (var builder = renderGraph.AddRasterRenderPass("Final Copy Pass (LightShaft)", out PassData passData))
            {
                builder.UseTexture(dstTextureHandle, AccessFlags.Read);
                builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);
                passData.SrcTextureHandle = dstTextureHandle;
                passData.material = null;
                builder.SetRenderFunc((PassData data, RasterGraphContext ctx) => ExecutePass(data.SrcTextureHandle, null, ctx));
            }
        }*/
        
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
}
