using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Timeline;
using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 実際の挙動
/// </summary>
public class CharaOnShadowMixer : PlayableBehaviour
{
    public List<Material> TargetMat = new List<Material>();
    public PlayableDirector Director { get; set; }
    public TimelineClip[] Clips { get; set; }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var time = Director.time; // Timeline全体の現在の時間
        for (int i = 0; i < Clips.Length; i++)
        {
            var clip = Clips[i];
            var clipAsset = clip.asset as CharaOnShadowClip; // クリップのアセット
            var behaviour = clipAsset.behaviour; // クリップが持つBehaviour
            var clipProgress = (float)((time - clip.start) / clip.duration); // クリップの進行率
            if (0 <= clipProgress && clipProgress <= 1)
            {
                foreach (var mat in TargetMat)
                {
                    if (behaviour.IsShadowOn)
                    {
                        mat.EnableKeyword("ENABLE_CHARA_ON_SHADOW");
                    }
                    else
                    {
                        mat.DisableKeyword("ENABLE_CHARA_ON_SHADOW");
                    }
                }
            }
        }
    }
}
