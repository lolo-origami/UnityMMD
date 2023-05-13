using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

/// <summary>
/// ScriptPlayableの定義
/// </summary>
[System.Serializable]
public class MeshRendererBehaviour : PlayableBehaviour
{
    // PlayableBehaviour内にクリップ情報を保持する変数を定義しておく
    public ShadowCastingMode ShadowCastingMode;
}
