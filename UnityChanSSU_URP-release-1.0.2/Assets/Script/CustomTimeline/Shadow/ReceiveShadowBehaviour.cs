using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// ScriptPlayableの定義
/// </summary>
[System.Serializable]
public class ReceiveShadowBehaviour : PlayableBehaviour
{
    // PlayableBehaviour内にクリップ情報を保持する変数を定義しておく
    public bool IsOn = true;
    public bool IsOverRideArea = false;
    
    [Range(0f, 1f)] public float StartShadowAreaValue = 0.45f;
    [Range(0f, 1f)] public float EndShadowAreaValue = 0.45f;
}
