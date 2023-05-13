using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;
using VolumetricLights;

[TrackClipType(typeof(VolumeLightClip))]
[TrackBindingType(typeof(VolumetricLight))]
public class VolumeLightTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var mixer = ScriptPlayable<VolumeLightMixer>.Create(graph, inputCount);
        var behaviour = mixer.GetBehaviour();
        
        mixer.GetBehaviour().Clips = GetClips().ToArray();
        var director = go.GetComponent<PlayableDirector>();
        mixer.GetBehaviour().Director = director;
        
        VolumetricLight trackBinding = director.GetGenericBinding(this) as VolumetricLight;
        if (trackBinding != null)
        {
            behaviour.TargetVolumetricLight = trackBinding;
        }

        return mixer;
    }
}
