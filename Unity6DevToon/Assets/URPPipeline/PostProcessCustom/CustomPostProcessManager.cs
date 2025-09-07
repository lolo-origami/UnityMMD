// CustomPostProcessManager.cs

using System.Collections.Generic;
using UnityEngine;

public class CustomPostProcessManager
{
    private static CustomPostProcessManager _instance;
    public static CustomPostProcessManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new CustomPostProcessManager();
            }
            return _instance;
        }
    }

    // 登録されたCustomPostProcessRenderFeatureを保持するリスト
    private readonly List<CustomPostProcessRFBase> _registeredFeatureList = new List<CustomPostProcessRFBase>();
    private int _cachedFrame = -1;
    private int _cachedFirst = -1;
    private int _cachedLast = -1;    

    public void RegisterFeature(CustomPostProcessRFBase feature)
    {
        if (!_registeredFeatureList.Contains(feature))
        {
            _registeredFeatureList.Add(feature);
        }
    }

    public void UnregisterFeature(CustomPostProcessRFBase feature)
    {
        if (_registeredFeatureList.Contains(feature))
        {
            _registeredFeatureList.Remove(feature);
        }
    }

    // 指定されたRenderFeatureのインデックスを返す
    public int GetFeatureIndex(CustomPostProcessRFBase feature)
    {
        return _registeredFeatureList.IndexOf(feature);
    }
    
    public (int first, int last) GetLastActiveIndexThisFrame(ref UnityEngine.Rendering.Universal.RenderingData renderingData)
    {
        int f = Time.renderedFrameCount;
        if (_cachedFrame == f)
            return (_cachedFirst, _cachedLast); // ★ 同フレームはキャッシュ


        int first = int.MaxValue;
        int last = -1;
        for (int i = 0; i < _registeredFeatureList.Count; i++)
        {
            var feat = _registeredFeatureList[i];
            if (!feat.IsActiveCheck()) continue; // ☑オフ除外
            if (feat is ICustomPostProcessFeature p && p.IsActiveThisFrame(ref renderingData))
            {
                if (i < first) first = i;
                if (i > last) last = i;
            }
        }


        _cachedFrame = f;
        _cachedFirst = (first == int.MaxValue) ? -1 : first;
        _cachedLast = last;
        return (_cachedFirst, _cachedLast);
    }
    
    public IReadOnlyList<CustomPostProcessRFBase> FeatureList => _registeredFeatureList;
}