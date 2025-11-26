using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RendererUtils;
using UnityEngine.Rendering.Universal;    
using UnityEngine.Rendering.RenderGraphModule;

class LLShadowStencilPass : ScriptableRenderPass
{
    private LLShadowStencilFeature.Settings _settings;
    
    static readonly ShaderTagId _tagReceive   = new ShaderTagId("_StencilReceiver");
    static readonly ShaderTagId _tagProjector = new ShaderTagId("_StencilProjector");
    
    public LLShadowStencilPass(LLShadowStencilFeature.Settings settings)
    {
        this._settings = settings;
        renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
    }

    class PassDataStencil
    {
        public RendererListHandle RendererList;
    }

    // RenderGraph にパスを登録する
    public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
    {
        var cameraData = frameData.Get<UniversalCameraData>();
        var renderData = frameData.Get<UniversalRenderingData>();
        var resourceData = frameData.Get<UniversalResourceData>();
        
        // ============ RendererList 作成 ===============
        var receiveListDesc = new RendererListDesc(
            _tagReceive,
            renderData.cullResults,
            cameraData.camera)
        {
            sortingCriteria = SortingCriteria.CommonOpaque,
            renderQueueRange = RenderQueueRange.opaque,
            layerMask = _settings.ReceiveLayer
        };
        
        var projListDesc = new RendererListDesc(
            _tagProjector,
            renderData.cullResults,
            cameraData.camera)
        {
            sortingCriteria = SortingCriteria.CommonOpaque,
            renderQueueRange = RenderQueueRange.opaque,
            layerMask = _settings.ReceiveLayer
        };

        // ============ パス登録 ===============
        // ステンシルが書き込まれた部分だけ描く
        using (var builder = renderGraph.AddRasterRenderPass("StencilProjector", out PassDataStencil passData))
        {
            passData.RendererList = renderGraph.CreateRendererList(projListDesc);
            builder.UseRendererList(passData.RendererList);
            
            builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);
            builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Read);

            // Stencil影を落とす部分を描く
            builder.SetRenderFunc((PassDataStencil data, RasterGraphContext ctx) =>
            {
                // ステンシルだけを描いて範囲を確定
                ctx.cmd.DrawRendererList(data.RendererList);
            });
        }
        
        // シェーダー側で、現在のColorバッファと色をブレンドして最終色にする
        using (var builder = renderGraph.AddRasterRenderPass("StencilReceive", out PassDataStencil passData))
        {
            // RendererList を生成
            passData.RendererList = renderGraph.CreateRendererList(receiveListDesc);
            
            builder.UseRendererList(passData.RendererList);

            // 出力 attachment
            builder.SetRenderAttachment(resourceData.activeColorTexture, 0, AccessFlags.Write);
            builder.SetRenderAttachmentDepth(resourceData.activeDepthTexture, AccessFlags.Read);
            
            // 描画関数
            builder.SetRenderFunc((PassDataStencil data, RasterGraphContext ctx) =>
            {
                ctx.cmd.DrawRendererList(data.RendererList);
            });
        }
    }
}