using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Linq;

[TrackClipType(typeof(RimLightControlClip))]
[TrackBindingType(typeof(Material))]
public class RimLightControlTrack : TrackAsset
{
    public string targetRendererName;
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
    {
        var mixer = ScriptPlayable<RimLightControlMixer>.Create(graph, inputCount);
        var behaviour = mixer.GetBehaviour();
        
        mixer.GetBehaviour().Clips = GetClips().ToArray();
        var director = go.GetComponent<PlayableDirector>();
        mixer.GetBehaviour().Director = director;
        
        Material trackBinding = director.GetGenericBinding(this) as Material;
        if (trackBinding != null)
        {
            behaviour.TargetMat = trackBinding;
            
            //MMDだと何故かPlaying時にInstance化されるので
            if (Application.isPlaying)
            {
                var targetRenderer = GameObject.Find(targetRendererName).GetComponent<Renderer>();
                foreach (var mat in targetRenderer.materials)
                {
                    if (mat.name.Contains(trackBinding.name))
                    {
                        behaviour.TargetMat = mat;
                    }
                }
            }
        }

        return mixer;
    }
}
