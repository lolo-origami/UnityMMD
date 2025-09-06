using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

public static class RenderGraphPostProcessUtils
{
    /// <summary>
    /// 一時テクスチャの参照を保持するためのクラス
    /// </summary>
    public class CustomPostProcessTextureHandleDictionary : ContextItem
    {
        public Dictionary<int, TextureHandle> textures = new Dictionary<int, TextureHandle>();

        public override void Reset()
        {
            textures.Clear();
        }
    }
    
    /// <summary>
    /// 一時テクスチャを作成してTextureを返す関数
    /// </summary>
    /// <param name="renderGraph">RenderGraphのインスタンス</param>
    /// <param name="frameData">ContextContainerのインスタンス</param>
    /// <param name="sourceTexture">ソーステクスチャ</param>
    /// <param name="textureName">作成するテクスチャの名前</param>
    /// <returns>一時テクスチャのTextureHandle</returns>
    public static TextureHandle CreateTemporaryTexture(RenderGraph renderGraph, ContextContainer frameData, TextureHandle sourceTexture, int index)
    {
        // sourceTextureを元に一時テクスチャを作成
        var textureDesc = renderGraph.GetTextureDesc(sourceTexture);
        textureDesc.name = string.Format("_TmepRT{0}", index) ;
        TextureHandle tempTextureHandle = renderGraph.CreateTexture(textureDesc);

        // 作成したテクスチャの参照をFrameDataに登録
        var textureDictionary = frameData.GetOrCreate<CustomPostProcessTextureHandleDictionary>();
        textureDictionary.textures[index] = tempTextureHandle;    
        
        return tempTextureHandle;
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
        var textureDictionary = frameData.GetOrCreate<CustomPostProcessTextureHandleDictionary>();

        // 最初のパスの場合、カメラのカラーターゲットを返す
        if (index == 0)
        {
            return resourceData.activeColorTexture;
        }

        // ひとつ前のインデックスのテクスチャを返す
        if (textureDictionary.textures.ContainsKey(index - 1))
        {
            return textureDictionary.textures[index - 1];
        }
        
        // テクスチャが見つからない場合、安全策としてカメラのカラーターゲットを返す
        return resourceData.activeColorTexture;
    }    
    
    /// <summary>
    /// パスの実行
    /// </summary>
    /// <param name="srcHandle"></param>
    /// <param name="material"></param>
    /// <param name="graphContext"></param>
    public static void ExecutePass(TextureHandle srcHandle, Material material, RasterGraphContext graphContext)
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
            Blitter.BlitTexture(cmd, srcHandle, new Vector4(1, 1, 0, 0), material, 0);
        }
    }
}