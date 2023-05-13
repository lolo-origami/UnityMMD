using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;

[TrackClipType(typeof(CharaOnShadowClip))]
[TrackBindingType(typeof(GameObject))]
public class CharaOnShadowTrack : TrackAsset
{
    public GameObject[] gameObjectArray;
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var mixer = ScriptPlayable<CharaOnShadowMixer>.Create(graph, inputCount);
        var behaviour = mixer.GetBehaviour();
        
        mixer.GetBehaviour().Clips = GetClips().ToArray();
        var director = go.GetComponent<PlayableDirector>();
        mixer.GetBehaviour().Director = director;
        
        GameObject trackBinding = director.GetGenericBinding(this) as GameObject;
        if (trackBinding != null)
        { 
            var targetRenderer = trackBinding.GetComponent<Renderer>();
            behaviour.TargetMat.AddRange(targetRenderer.sharedMaterials);
        }

        return mixer;
    }
}
