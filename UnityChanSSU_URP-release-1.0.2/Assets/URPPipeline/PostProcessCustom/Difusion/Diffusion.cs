using System.Drawing;
using UnityEngine;
using UnityEngine.Rendering;
using Color = UnityEngine.Color;

public class Diffusion : VolumeComponent
{
    public FloatParameter Contrast = new FloatParameter(1.0f);
    public ClampedFloatParameter Intensity = new ClampedFloatParameter(0f, 0f, 1f);

    public bool IsActive => Intensity.value > 0f;
}
