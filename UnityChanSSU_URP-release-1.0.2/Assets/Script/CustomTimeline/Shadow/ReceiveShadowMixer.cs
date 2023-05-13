using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;
using UnityEngine;

/// <summary>
/// 実際の挙動
/// </summary>
public class ReceiveShadowMixer : PlayableBehaviour
{
    public Material TargetMat = null;
    public PlayableDirector Director { get; set; }
    public TimelineClip[] Clips { get; set; }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var time = Director.time; // Timeline全体の現在の時間
        for (int i = 0; i < Clips.Length; i++)
        {
            var clip = Clips[i];
            var clipAsset = clip.asset as ReceiveShadowClip; // クリップのアセット
            var behaviour = clipAsset.behaviour; // クリップが持つBehaviour
            var weight = playable.GetInputWeight(i);
            var clipProgress = (float)((time - clip.start) / clip.duration); // クリップの進行率
            if (0 <= clipProgress && clipProgress <= 1)
            {
                if (behaviour.IsOverRideArea)
                {
                    var shadowAreaValue = Mathf.Lerp(behaviour.StartShadowAreaValue, behaviour.EndShadowAreaValue, clipProgress) * weight;
                    TargetMat.SetFloat("_ShadowArea", shadowAreaValue);
                }

                TargetMat.SetFloat("_ReceiveShadows", behaviour.IsOn == false ? 0 : 1);
            }
        }
    }
}
