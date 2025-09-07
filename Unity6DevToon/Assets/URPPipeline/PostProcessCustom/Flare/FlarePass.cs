using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;
using static RenderGraphPostProcessUtils;

public class FlarePass : ScriptableRenderPass
{
    private static readonly int _cameraMainTextureId = Shader.PropertyToID("_MainTex");
    private static readonly int FlareVectorId = UnityEngine.Shader.PropertyToID("_FlareVector");
    private static readonly int FlareColorId = UnityEngine.Shader.PropertyToID("_FlareColor");
    private static readonly int ParaVectorId = UnityEngine.Shader.PropertyToID("_ParaVector");
    private static readonly int ParaColorId = UnityEngine.Shader.PropertyToID("_ParaColor");

    private class PassData
    {
        public Material flareMaterial;
        public Vector2Parameter flarePosition;
        public FloatParameter flareSize;
        public ColorParameter flareColor;
        public Vector2Parameter paraPosition;
        public FloatParameter paraSize;
        public ColorParameter paraColor;
        public TextureHandle srcTextureHandle;
    }

    private readonly Material _flareMaterial;
    private int _index;
    private int _maxindex;

    public Material FlareMaterial => _flareMaterial;

    public FlarePass(RenderPassEvent renderPassEvent, Shader shader)
    {
        this.renderPassEvent = renderPassEvent;
        if (shader != null) _flareMaterial = CoreUtils.CreateEngineMaterial(shader);
    }

    public void SetFrameOrder(int index, int maxIndex)
    {
        _index = index; _maxindex = maxIndex;
    }
    
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        if (_flareMaterial == null || _index < 0)
        {
            return;
        }
        
        var resourceData = frameData.Get<UniversalResourceData>();
        var cameraData = frameData.Get<UniversalCameraData>();
        var volumeStack = VolumeManager.instance.stack;
        var comp = volumeStack.GetComponent<Flare>();

        if (!cameraData.postProcessEnabled || comp == null || !comp.IsActive)
        {
            return;
        }

        // 1:一時的なレンダーテクスチャを作成
        TextureHandle srcTextureHandle = GetTemporaryTexture(frameData, _index, resourceData);
        TextureHandle dstTextureHandle = CreateTemporaryTexture(renderGraph, frameData, srcTextureHandle, _index);
        
        // 2:カメラのカラーバッファをマテリアルを適用しながら一時的なレンダーテクスチャーに書き込む
        using (IRasterRenderGraphBuilder builder = renderGraph.AddRasterRenderPass("Apply Flare Pass", out PassData passData))
        {
            builder.UseTexture(srcTextureHandle, AccessFlags.Read);
            builder.SetRenderAttachment(dstTextureHandle, 0, AccessFlags.Write);
            passData.srcTextureHandle = srcTextureHandle;
            passData.flareMaterial = _flareMaterial;
            passData.flarePosition = comp.flarePosition;
            passData.flareSize = comp.flareSize;
            passData.flareColor = comp.flareColor;
            passData.paraPosition = comp.paraPosition;
            passData.paraSize = comp.paraSize;
            passData.paraColor = comp.paraColor;

            builder.SetRenderFunc((PassData data, RasterGraphContext graphContext) =>
            {
                data.flareMaterial.SetTexture(_cameraMainTextureId, passData.srcTextureHandle);
                data.flareMaterial.SetVector(FlareVectorId, new Vector4(data.flarePosition.value.x, data.flarePosition.value.y, 1f / data.flareSize.value));
                data.flareMaterial.SetColor(FlareColorId, data.flareColor.value);
                data.flareMaterial.SetVector(ParaVectorId, new Vector4(data.paraPosition.value.x, data.paraPosition.value.y, 1f / data.paraSize.value));
                data.flareMaterial.SetColor(ParaColorId, data.paraColor.value);
                ExecutePass(data.srcTextureHandle, data.flareMaterial, graphContext);
            });
            //renderGraph.AddBlitPass(blitParameters, "Radial Blur Effect");
        }
        
        if (_index == _maxindex) // ★ 最後だけカメラへコピー
        {
            using (var builder = renderGraph.AddRasterRenderPass("Final Copy Pass (Flare)", out PassData pd))
            {
                builder.UseTexture(dstTextureHandle, AccessFlags.Read);
                builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);
                pd.srcTextureHandle = dstTextureHandle;
                pd.flareMaterial = null;
                builder.SetRenderFunc((PassData data, RasterGraphContext ctx) => ExecutePass(data.srcTextureHandle, null, ctx));
            }
        }
    }
}