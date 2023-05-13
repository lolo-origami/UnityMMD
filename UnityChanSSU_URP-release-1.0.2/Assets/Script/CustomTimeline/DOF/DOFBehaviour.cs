using UnityEngine;
using UnityEngine.Playables;

/// <summary>
/// ScriptPlayableの定義
/// </summary>
[System.Serializable]
public class DOFBehaviour : PlayableBehaviour
{
    // PlayableBehaviour内にクリップ情報を保持する変数を定義しておく
    [Range(40f, 60f)] public float StartValue = 0f;
    [Range(40f, 60f)] public float EndValue = 0f;
    public bool Override = false;
    //TODO Curveも？
}
