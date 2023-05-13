using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugController : MonoBehaviour
{
    #region 定数

    private const float k = 0.05f;

    #endregion

    [SerializeField] private GameObject _debugRootObj = null;

    [SerializeField] private Button _debugAwakeButton = null;

    [SerializeField] private TextMeshProUGUI _fpsAvgText = null;
    [SerializeField] private TextMeshProUGUI _fpsSpText = null;
    
    [SerializeField] private TextMeshProUGUI _screenSizeText = null;

    private bool _isShow = false;
    private float _avgTime = 0f;
    private float _sqSpike;

    private void Awake()
    {
        _debugAwakeButton.onClick.AddListener(OnClickDebugButton);
        OnOffDebug();
    }

    private void Update()
    {
        var deltaTime = Time.deltaTime;
        //平均FPS表示更新
        _avgTime *= 1f - k;
        _avgTime += deltaTime * k;
        _fpsAvgText.text = string.Format("avg: {0}",(1f / _avgTime).ToString());
        
        //最大FPS更新
        _sqSpike *= 1f - k;
        _sqSpike += (deltaTime * deltaTime) * k;
        _fpsSpText.text = string.Format("sp: {0}", Mathf.Sqrt(_sqSpike).ToString());
        
        //ScreenSize
        _screenSizeText.text = string.Format("ScreenSize: width {0} height {1}(Origin {2}x{3})", 
            GlaphicsSettingsUtil.GetResolution().x.ToString(), GlaphicsSettingsUtil.GetResolution().y.ToString(),
            Screen.width.ToString(), Screen.height.ToString());
    }

    /// <summary>
    /// クリックボタン
    /// </summary>
    private void OnClickDebugButton()
    {
        _isShow = !_isShow;
        OnOffDebug();
    }

    /// <summary>
    /// デバッグ切り替え
    /// </summary>
    private void OnOffDebug()
    {
        if (!_isShow)
        {
            _debugRootObj.gameObject.SetActive(false);
        }
        else
        {
            _debugRootObj.gameObject.SetActive(true);
        }
    }
}