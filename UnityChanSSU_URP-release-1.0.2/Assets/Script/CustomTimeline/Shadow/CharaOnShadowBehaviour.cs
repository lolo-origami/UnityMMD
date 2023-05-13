using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// ScriptPlayableの定義
/// </summary>
[System.Serializable]
public class CharaOnShadowBehaviour : PlayableBehaviour
{
    // PlayableBehaviour内にクリップ情報を保持する変数を定義しておく
    public bool IsShadowOn = true;
}
