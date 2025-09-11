using UnityEngine;
using UnityEngine.Rendering;


public class SSRaymarchFog : VolumeComponent
{
    public IntParameter MaxIterations = new IntParameter(64);
    public FloatParameter MaxDistance = new FloatParameter(50f);
    public FloatParameter MinDistance = new FloatParameter(0.1f);
    public ClampedFloatParameter Intensity = new ClampedFloatParameter(1.0f, 0f, 10f);
        
    //0 だと完全無効化したいときに早期 return 可能
    public bool IsActive => Intensity.value > 0f;
}