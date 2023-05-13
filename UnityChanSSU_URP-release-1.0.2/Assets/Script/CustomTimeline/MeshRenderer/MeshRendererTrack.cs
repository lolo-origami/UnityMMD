using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;

[TrackClipType(typeof(MeshRendererClip))]
[TrackBindingType(typeof(MeshRenderer))]
public class MeshRendererTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var mixer = ScriptPlayable<MeshRendererMixer>.Create(graph, inputCount);
        var behaviour = mixer.GetBehaviour();
        
        mixer.GetBehaviour().Clips = GetClips().ToArray();
        var director = go.GetComponent<PlayableDirector>();
        mixer.GetBehaviour().Director = director;
        
        MeshRenderer trackBinding = director.GetGenericBinding(this) as MeshRenderer;
        if (trackBinding != null)
        {
            behaviour.TargetRenderer = trackBinding;
        }

        return mixer;
    }
}
