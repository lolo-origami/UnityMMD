using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// ScriptPlayableの定義
/// </summary>
[System.Serializable]
public class VolumeLightBehaviour : PlayableBehaviour
{
    // PlayableBehaviour内にクリップ情報を保持する変数を定義しておく
    [Range(0.25f,1f)] public float StartDensity = 1f;
    [Range(0.25f,1f)] public float EndDensity = 1f;
    
    [Range(0.1f,1f)] public float StartBrightness = 0.5f;
    [Range(0.1f,1f)] public float EndBrightness = 0.5f;
    
    [Range(0.25f,1.5f)] public float StartRangeFallOff = 1.2f;
    [Range(0.25f,1.5f)] public float EndRangeFallOff = 1.2f;
    
    [Range(2.5f,15f)] public float StartDiffusionIntensity = 12.5f;
    [Range(2.5f,15f)] public float EndDiffusionIntensity = 12.5f;
    
    public bool Transluency = true;
}
