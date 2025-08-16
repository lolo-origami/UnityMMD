using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;

public static class RenderGraphPostProcessUtils
{
    /// <summary>
    /// シェーダーを使わずに、指定されたテクスチャの内容を現在のレンダーターゲットにコピーする汎用Blit
    /// </summary>
    /// <param name="cmd">Blitを実行するRasterCommandBuffer</param>
    /// <param name="srcTextureHandle">コピー元のテクスチャハンドル</param>
    /// <param name="profilerTag">プロファイラに表示するタグ名</param>
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

    // 必要に応じて、他の汎用的なBlit処理を追加できます。
    // 例えば、シェーダーを使った汎用Blitなど
    // public static void BlitWithMaterial(RasterCommandBuffer cmd, TextureHandle srcTextureHandle, Material material, int passIndex, string profilerTag = "Generic Blit With Material")
    // {
    //     using (new ProfilingScope(cmd, new ProfilingSampler(profilerTag)))
    //     {
    //         Blitter.BlitTexture(cmd, srcTextureHandle, new Vector4(1, 1, 0, 0), material, passIndex);
    //     }
    // }
}