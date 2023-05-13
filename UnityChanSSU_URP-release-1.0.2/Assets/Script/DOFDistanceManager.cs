using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DOFDistanceManager : MonoBehaviour
{
    private const float BASE_POS_DISTANCE = 2.25f;
    private const float BASE_FOCAS_DISTANCE = 1.1f;
    private const float ALLIMENT_VALUE = 0.05f;
    
    [SerializeField]
    public Transform TargetTransform = null;
    
    [SerializeField]
    public Transform CameraTransform = null;
    
    [SerializeField]
    private VolumeProfile _profile = null;

    private DepthOfField _dof = null;

    private void Initialize()
    {
        for (int i = 0; i < _profile.components.Count; i++)
        {
            _dof = _profile.components[i] as DepthOfField;
            if(_dof != null)
                return;
        }
    }
    
    public void UpdateDistance()
    {
        if (_dof == null)
        {
            Initialize();
        }
        
        //通常はデフォルトの計算値
        var distance = Vector3.Distance(TargetTransform.position, CameraTransform.position);
        Debug.Log("Distance= " + distance);

        var plusLength = (distance - BASE_POS_DISTANCE) * ALLIMENT_VALUE;
        _dof.focusDistance.value = BASE_FOCAS_DISTANCE + plusLength;
    }

    public void UpdateDistance(float startValue, float endValue, float clipProgress, float weight)
    {
        _dof.focusDistance.value = Mathf.Lerp(startValue, endValue, clipProgress) * weight;
    }
}
