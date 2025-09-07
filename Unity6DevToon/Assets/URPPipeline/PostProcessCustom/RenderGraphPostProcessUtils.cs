using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public static class RenderGraphPostProcessUtils
{
    /// <summary>
    /// フレーム内チェーンの Temp を保持（ContextContainer にぶら下げるので自動的にフレームスコープ）
    /// </summary>
    public class CustomPostProcessTextureHandleDictionary : ContextItem
    {
        public Dictionary<int, TextureHandle> textures = new Dictionary<int, TextureHandle>();
        public override void Reset() => textures.Clear();
    }
    
    /// <summary>
    /// 一時テクスチャを作成してTextureを返す関数
    /// </summary>
    /// <param name="renderGraph">RenderGraphのインスタンス</param>
    /// <param name="frameData">ContextContainerのインスタンス</param>
    /// <param name="sourceTexture">ソーステクスチャ</param>
    /// <param name="textureName">作成するテクスチャの名前</param>
    /// <returns>一時テクスチャのTextureHandle</returns>
    public static TextureHandle CreateTemporaryTexture(RenderGraph renderGraph, ContextContainer frameData, TextureHandle sourceTexture, int index, string name)
    {
        var desc = renderGraph.GetTextureDesc(sourceTexture);

        // 1時バッファは非MSAA・カラーのみ
        desc.msaaSamples    = MSAASamples.None;             // ★ MSAA禁止（Resolve/Discard問題の根本回避）
        desc.depthBufferBits= 0;
        desc.name           = $"_TempRT{name}";

        var temp = renderGraph.CreateTexture(desc);

        var dict = frameData.GetOrCreate<CustomPostProcessTextureHandleDictionary>();
        dict.textures[index] = temp;         // 同indexを上書き登録OK

        return temp;
    }
    
    /// <summary>
    /// ひとつ前のTempTextureを返す関数、無ければカメラのColorターゲットを返す
    /// </summary>
    /// <param name="renderGraph">RenderGraphのインスタンス</param>
    /// <param name="frameData">ContextContainerのインスタンス</param>
    /// <param name="sourceTexture">ソーステクスチャ</param>
    /// <param name="textureName">作成するテクスチャの名前</param>
    /// <returns>一時テクスチャのTextureHandle</returns>
    public static TextureHandle GetTemporaryTexture(ContextContainer frameData, int index, UniversalResourceData resourceData)
    {
        var dict = frameData.GetOrCreate<CustomPostProcessTextureHandleDictionary>();
        if (index == 0) 
            return resourceData.activeColorTexture;
        for (int i = index - 1; i >= 0; i--)
        {
            if (dict.textures.TryGetValue(i, out var prev) && prev.IsValid())
                return prev;
        }
        
        return resourceData.activeColorTexture;
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