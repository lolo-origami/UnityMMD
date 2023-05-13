using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// ScriptPlayableの定義
/// </summary>
[System.Serializable]
public class RimLightControlBehaviour : PlayableBehaviour
{
    // PlayableBehaviour内にクリップ情報を保持する変数を定義しておく
    public bool EnableRim = false;
    public bool EnableDarkRim = false;
    
    [Range(0f, 10f)] public float RimSmoothValue = 1f;
    [Range(0f, 10f)] public float RimPowValue = 3f;
    
    [Range(0f, 10f)] public float DarkRimSmoothValue = 1f;
    [Range(0f, 10f)] public float DarkRimPowValue = 3f;
}
