// CustomPostProcessManager.cs

using System.Collections.Generic;
using UnityEngine;

public class LLPostProcessRFManager
{
    private static LLPostProcessRFManager _instance;
    public static LLPostProcessRFManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = new LLPostProcessRFManager();
            }
            return _instance;
        }
    }

    // 登録されたCustomPostProcessRenderFeatureを保持するリスト
    private readonly List<LLPostProcessRFBase> _registeredFeatureList = new List<LLPostProcessRFBase>();
    private int _cachedFrame = -1;
    private int _cachedFirst = -1;
    private int _cachedLast = -1;    

    public void RegisterFeature(LLPostProcessRFBase feature)
    {
        if (!_registeredFeatureList.Contains(feature))
        {
            _registeredFeatureList.Add(feature);
        }
    }

    public void UnregisterFeature(LLPostProcessRFBase feature)
    {
        if (_registeredFeatureList.Contains(feature))
        {
            _registeredFeatureList.Remove(feature);
        }
    }

    // 指定されたRenderFeatureのインデックスを返す
    public int GetFeatureIndex(LLPostProcessRFBase feature)
    {
        return _registeredFeatureList.IndexOf(feature);
    }
    
    public  int  GetLastActiveIndexThisFrame(ref UnityEngine.Rendering.Universal.RenderingData renderingData)
    {
        int f = Time.renderedFrameCount;
        if (_cachedFrame == f)
            return _cachedLast; // ★ 同フレームはキャッシュ


        int first = int.MaxValue;
        int last = -1;
        for (int i = 0; i < _registeredFeatureList.Count; i++)
        {
            var feat = _registeredFeatureList[i];
            if (!feat.IsActiveCheck()) continue; // ☑オフ除外
            if (feat is ILLPostProcessFeature p && p.IsActiveThisFrame(ref renderingData))
            {
                if (i < first) first = i;
                if (i > last) last = i;
            }
        }


        _cachedFrame = f;
        _cachedFirst = (first == int.MaxValue) ? -1 : first;
        _cachedLast = last;
        return _cachedLast;
    }
    
    public IReadOnlyList<LLPostProcessRFBase> FeatureList => _registeredFeatureList;
}