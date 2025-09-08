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
        int last = -1;

        for (int i = 0; i < _registeredFeatureList.Count; i++)
        {
            var feat = _registeredFeatureList[i];
            if (!feat.IsActiveCheck()) continue; // ☑ オフのものは無視
            if (feat is ILLPostProcessFeature p && p.IsActiveThisFrame(ref renderingData))
            {
                last = i; // アクティブなもののうち一番後ろを更新
            }
        }

        return last;
    }
    
    public IReadOnlyList<LLPostProcessRFBase> FeatureList => _registeredFeatureList;
}