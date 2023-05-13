using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;
using Color = UnityEngine.Color;

public class LightShaft : VolumeComponent
{
    public IntParameter MaxIterations = new IntParameter(64);
    public FloatParameter MaxDistance = new FloatParameter(12f);
    public FloatParameter MinDistance = new FloatParameter(0.4f);

    public ClampedFloatParameter Intensity = new ClampedFloatParameter(0f, 0f, 1f);
    
    public bool IsActive => Intensity.value > 0f;
}
