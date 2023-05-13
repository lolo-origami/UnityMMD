using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine;
using VolumetricLights;

/// <summary>
/// 実際の挙動
/// </summary>
public class VolumeLightMixer : PlayableBehaviour
{
    public VolumetricLight TargetVolumetricLight = null;
    public PlayableDirector Director { get; set; }
    public TimelineClip[] Clips { get; set; }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        var time = Director.time; // Timeline全体の現在の時間
        for (int i = 0; i < Clips.Length; i++)
        {
            var clip = Clips[i];
            var clipAsset = clip.asset as VolumeLightClip; // クリップのアセット
            var behaviour = clipAsset.behaviour; // クリップが持つBehaviour
            var weight = playable.GetInputWeight(i);
            var clipProgress = (float)((time - clip.start) / clip.duration); // クリップの進行率
            if (0 < clipProgress && clipProgress <= 1)
            {
                var densityValue = Mathf.Lerp(behaviour.StartDensity, behaviour.EndDensity, clipProgress) * weight;
                TargetVolumetricLight.density = densityValue;
                
                var brightnessValue = Mathf.Lerp(behaviour.StartBrightness, behaviour.EndBrightness, clipProgress) * weight;
                TargetVolumetricLight.brightness = brightnessValue;
                
                var rangeFallValue = Mathf.Lerp(behaviour.StartRangeFallOff, behaviour.EndRangeFallOff, clipProgress) * weight;
                TargetVolumetricLight.rangeFallOff = rangeFallValue;
                
                var diffusionIntensityValue = Mathf.Lerp(behaviour.StartDiffusionIntensity, behaviour.EndDiffusionIntensity, clipProgress) * weight;
                TargetVolumetricLight.diffusionIntensity = diffusionIntensityValue;
                
                TargetVolumetricLight.shadowTranslucency = behaviour.Transluency;
            }
        }
    }
}
