using System.Drawing;
using UnityEngine.Rendering;
using Color = UnityEngine.Color;

public class DepthFog : VolumeComponent
{
    //public TextureParameter rampTexture = new TextureParameter(null);

    public ClampedFloatParameter intensity = new ClampedFloatParameter(0f, 0f, 100f);
    
    public ColorParameter fogColor = new ColorParameter(Color.white, false, true, false);
    public bool IsActive => intensity.value > 0f;
}