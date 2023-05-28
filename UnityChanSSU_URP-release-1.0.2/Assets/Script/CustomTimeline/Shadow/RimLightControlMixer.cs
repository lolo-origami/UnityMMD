using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;
using UnityEngine;

/// <summary>
/// 実際の挙動
/// </summary>
public class RimLightControlMixer : PlayableBehaviour
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
            var clipAsset = clip.asset as RimLightControlClip; // クリップのアセット
            var behaviour = clipAsset.behaviour; // クリップが持つBehaviour
            var weight = playable.GetInputWeight(i);
            var clipProgress = (float)((time - clip.start) / clip.duration); // クリップの進行率
            if (0 <= clipProgress && clipProgress <= 1)
            {
                TargetMat.SetFloat("_EnableRim", behaviour.EnableRim == false ? 0 : 1);
                if (behaviour.EnableRim)
                {
                    var rimSmoothValue = behaviour.RimSmoothValue * weight;
                    TargetMat.SetFloat("_RimSmooth", rimSmoothValue);
                    var rimPowValue = behaviour.RimPowValue * weight;
                    TargetMat.SetFloat("_RimPow", rimPowValue);
                }
                
                TargetMat.SetFloat("_EnableRimDS", behaviour.EnableDarkRim == false ? 0 : 1);
                if (behaviour.EnableDarkRim)
                {
                    var darkRimSmoothValue = behaviour.DarkRimSmoothValue * weight;
                    TargetMat.SetFloat("_DarkSideRimSmooth", darkRimSmoothValue);
                    var darkRimPowValue = behaviour.DarkRimPowValue * weight;
                    TargetMat.SetFloat("_DarkSideRimPow", darkRimPowValue);
                }
            }
        }
    }
}
