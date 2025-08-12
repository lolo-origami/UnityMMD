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
    public static void BlitToCurrentRenderTarget(RasterCommandBuffer cmd, TextureHandle srcTextureHandle, string profilerTag = "Generic Blit To Screen")
    {
        // プロファイリングスコープで処理時間を計測
        using (new ProfilingScope(cmd, new ProfilingSampler(profilerTag)))
        {
            // Blitter.BlitTextureを使って、指定されたテクスチャの内容を現在のレンダーターゲットにコピーする
            // このオーバーロードは、シェーダーを使わない単純なコピーに適しているよ
            Blitter.BlitTexture(cmd, srcTextureHandle, new Vector4(1, 1, 0, 0), 0, false);
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