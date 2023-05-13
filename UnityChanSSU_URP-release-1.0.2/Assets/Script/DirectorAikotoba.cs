using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class DirectorAikotoba : MonoBehaviour
{
    public GlaphicsSettingsUtil.ScreenMode ScreenType = GlaphicsSettingsUtil.ScreenMode.StandAlone;

    public DOFDistanceManager DistanceManager;
    
    void Awake()
    {
        if (ScreenType == GlaphicsSettingsUtil.ScreenMode.Mobile)
        {
            GlaphicsSettingsUtil.ApplySetResolution();
        }
        Application.targetFrameRate = 30;
    }
}
