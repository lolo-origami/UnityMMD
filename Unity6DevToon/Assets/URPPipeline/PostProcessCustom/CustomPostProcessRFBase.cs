using UnityEngine;
using UnityEngine.Rendering.Universal;

public abstract class CustomPostProcessRFBase : ScriptableRendererFeature
{
    // ①インデックスを保持するためのフィールド
    protected static int _index = -1;

    // ①Create時にインデックスを増やす
    protected void AddIndex()
    {
        _index++;
    }
    
    // Dispose時にインデックスを減らす
    protected void DecIndex()
    {
        _index--;
        if (_index < 0)
        {
            _index = -1;
        }
    }
}