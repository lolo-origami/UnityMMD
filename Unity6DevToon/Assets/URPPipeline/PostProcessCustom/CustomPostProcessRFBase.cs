using UnityEngine;
using UnityEngine.Rendering.Universal;

public interface ICustomPostProcessFeature
{
    bool IsActiveThisFrame(ref RenderingData renderingData); // ★ そのカメラ/フレームで有効か
}

public abstract class CustomPostProcessRFBase : ScriptableRendererFeature, ICustomPostProcessFeature
{
    [System.Serializable]
    public class Settings
    {
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        public Shader shader;
        public bool IsSave = false;
        public string SaveName = "";
        public bool IsRestore = false;
        public string RestoreName = "";
    }
    
    public Settings settings = new Settings();
    
    public override void Create()
    {
        CustomPostProcessManager.Instance.RegisterFeature(this);
        OnCreate();
    }


    protected virtual void OnCreate() { }


    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);
        CustomPostProcessManager.Instance.UnregisterFeature(this);
        OnDispose(disposing);
    }
    
    protected virtual void OnDispose(bool disposing) { }
    
    public abstract bool IsActiveThisFrame(ref RenderingData renderingData);
    
    protected int FeatureIndex() => CustomPostProcessManager.Instance.GetFeatureIndex(this);
    
    public bool IsActiveCheck()
    {
        var pi = typeof(ScriptableRendererFeature).GetProperty(
            "isActive",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
        return (bool)(pi?.GetValue(this) ?? true);
    }    
}