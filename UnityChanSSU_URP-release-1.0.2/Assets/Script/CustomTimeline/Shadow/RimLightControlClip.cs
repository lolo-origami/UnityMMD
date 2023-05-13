using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

/// <summary>
/// Trackに設定するclip
/// </summary>
public class RimLightControlClip : PlayableAsset, ITimelineClipAsset
{
    // 必ずpublic（レコードボタンが表示されない）でBehaviourを持たせる
    public RimLightControlBehaviour behaviour = new RimLightControlBehaviour();
    public override Playable CreatePlayable(PlayableGraph graph, GameObject go)
    {
        return ScriptPlayable<RimLightControlBehaviour>.Create(graph);
    }
    
    // このクリップの特徴を定義
    public ClipCaps clipCaps {
        get {
            // タイムスケール変更に対応
            return ClipCaps.SpeedMultiplier;
        }
    }
}
