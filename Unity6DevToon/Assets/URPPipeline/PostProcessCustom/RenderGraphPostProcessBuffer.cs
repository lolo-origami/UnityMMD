using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public static class RenderGraphPostProcessBuffer
{
    /// <summary>
    /// フレーム中のみ有効なチェーン／名前付き一時バッファの管理。
    /// </summary>
    public class FrameTempBuffers : ContextItem
    {
        public bool IsCreateChainTempBuffer;
        public TextureHandle chainTempBuffer; // 直近のチェーン用 TempBuffer
        public Dictionary<string, TextureHandle> namedBufferDicionary = new(); // 名前付き保存
        public override void Reset()
        {
            IsCreateChainTempBuffer = false;
            chainTempBuffer = TextureHandle.nullHandle;
            namedBufferDicionary.Clear();
        }
        
    }
    
    /// <summary>
    /// 作成されたBufferがあれば返す、無ければカメラ
    /// </summary>
    /// <param name="frameData"></param>
    /// <param name="resourceData"></param>
    /// <returns></returns>
    public static TextureHandle GetChainBufferOrCamera(ContextContainer frameData, UniversalResourceData resourceData)
    {
        var store = frameData.GetOrCreate<FrameTempBuffers>();
        
        return store.IsCreateChainTempBuffer ? 
            store.chainTempBuffer : resourceData.activeColorTexture;
    }
    
    /// <summary>
    /// ChainBufferがあれば返す、無ければ作る
    /// </summary>
    /// <param name="renderGraph"></param>
    /// <param name="frameData"></param>
    /// <param name="refTexture"></param>
    /// <param name="debugName"></param>
    /// <returns></returns>
    public static TextureHandle GetChainBuffer(RenderGraph renderGraph, ContextContainer frameData, TextureHandle refTexture)
    {
        //あるものを返す
        var store = frameData.GetOrCreate<FrameTempBuffers>();
        if (store.IsCreateChainTempBuffer)
        {
            return store.chainTempBuffer;
        }

        //無ければ作る
        var desc = renderGraph.GetTextureDesc(refTexture);
        desc.msaaSamples = MSAASamples.None;
        desc.depthBufferBits = 0;
        desc.name = $"_TempRT";
        store.chainTempBuffer = renderGraph.CreateTexture(desc);
        
        store.IsCreateChainTempBuffer = true;
        
        return store.chainTempBuffer;
    }
    
    /// <summary>
    /// 名前付きのTempBufferを作成して保存
    /// </summary>
    /// <param name="frameData"></param>
    /// <param name="name"></param>
    /// <param name="handle"></param>
    public static void CreateNamedBuffer(RenderGraph renderGraph, ContextContainer frameData, TextureHandle refTexture, string name)
    {
        var desc = renderGraph.GetTextureDesc(refTexture);
        desc.msaaSamples = MSAASamples.None;
        desc.depthBufferBits = 0;
        desc.name = $"_TempRT_{name}";
        
        var store = frameData.GetOrCreate<FrameTempBuffers>();
        store.namedBufferDicionary[name] = renderGraph.CreateTexture(desc);
    }
    
    public static TextureHandle CreateTempBufferNamedChainTexture(RenderGraph renderGraph, ContextContainer frameData, TextureHandle refTexture, string debugName)
    {
        var store = frameData.GetOrCreate<FrameTempBuffers>();
        var desc = renderGraph.GetTextureDesc(refTexture);
        desc.msaaSamples = MSAASamples.None;
        desc.depthBufferBits = 0;
        desc.name = $"_TempRT_{debugName}";
        store.chainTempBuffer = renderGraph.CreateTexture(desc);
        return store.chainTempBuffer;
    }    

    
    /// <summary>
    /// SaveされたBufferの取得
    /// </summary>
    /// <param name="frameData"></param>
    /// <param name="name"></param>
    /// <param name="handle"></param>
    /// <returns></returns>
    public static bool TryGetSavedBuffer(ContextContainer frameData, string name, out TextureHandle handle)
    {
        var store = frameData.GetOrCreate<FrameTempBuffers>();
        if (!string.IsNullOrEmpty(name) && store.namedBufferDicionary.TryGetValue(name, out var h) && h.IsValid())
        {
            handle = h; return true;
        }
        handle = default; return false;
    }


    /// <summary>
    /// TempBufferをSetする
    /// </summary>
    /// <param name="frameData"></param>
    /// <param name="handle"></param>
    public static void SetChainBuffer(ContextContainer frameData, TextureHandle handle)
    {
        var store = frameData.GetOrCreate<FrameTempBuffers>();
        store.chainTempBuffer = handle;
    }
    /// <summary>
    /// パスの実行
    /// </summary>
    /// <param name="srcHandle"></param>
    /// <param name="material"></param>
    /// <param name="graphContext"></param>
    public static void ExecutePass(TextureHandle srcHandle, Material material, RasterGraphContext graphContext, int passIndex = 0)
    {
        RasterCommandBuffer cmd = graphContext.cmd;
        if (material == null)
        {
            //コピー
            Blitter.BlitTexture(cmd, srcHandle, new Vector4(1, 1, 0, 0), 0, false);
        }
        else
        {
            //フルスクリーンエフェクトをかけて書き込む
            Blitter.BlitTexture(cmd, srcHandle, new Vector4(1, 1, 0, 0), material, passIndex);
        }
    }
}