using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlaphicsSettingsUtil
{
    private enum VirtualResolution
    {
        Horizontal, //横
        Vertical //縦
    }
    
    private const int WIDTH_BASE_3D = 1280;
    private const int HEIGHT_MOBILE_3D = 720;
    public enum ScreenMode
    {
        StandAlone,
        Mobile
    }

    private static Vector2Int _resolution; 

    /// <summary>
    /// 縦横基準決定
    /// </summary>
    /// <returns></returns>
    private static VirtualResolution GetVirtualResolutionXY()
    {
        var originalAspect = Screen.height / Screen.width;
        var screenVirtualResolutionH = Mathf.RoundToInt(WIDTH_BASE_3D * originalAspect);
        return screenVirtualResolutionH > HEIGHT_MOBILE_3D ? VirtualResolution.Horizontal : VirtualResolution.Vertical;
    }
    
    /// <summary>
    /// 解像度取得
    /// </summary>
    public static Vector2Int GetResolution()
    {
        _resolution.x = Screen.width;
        _resolution.y = Screen.height;

        var defaultResolution = new Vector2Int(WIDTH_BASE_3D, HEIGHT_MOBILE_3D);

        var widthBasedRatio = Mathf.Min(1f, defaultResolution.x / (float)Screen.width);
        var heightBasedRatio = Mathf.Min(1f, defaultResolution.y / (float)Screen.height);

        var ratio = GetVirtualResolutionXY() == VirtualResolution.Vertical ? heightBasedRatio : widthBasedRatio;

        _resolution.x = Mathf.RoundToInt(_resolution.x * ratio);
        _resolution.y = Mathf.RoundToInt(_resolution.y * ratio);

        return _resolution;
    }

    /// <summary>
    /// 解像度指定
    /// </summary>
    public static void ApplySetResolution()
    {
        Screen.SetResolution(GetResolution().x, GetResolution().y, true);
    }


}
