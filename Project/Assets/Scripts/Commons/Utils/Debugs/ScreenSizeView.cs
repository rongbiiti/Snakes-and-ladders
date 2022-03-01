using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 画面サイズを表示するクラス
/// </summary>
public class ScreenSizeView : MonoBehaviour
{
    /// <summary>
    /// 画面サイズを表示するテキスト
    /// </summary>
    [SerializeField]
    private Text _screenSizeText = default;


    /// <summary>
    /// 表示時
    /// </summary>
    private void OnEnable()
    {
        _screenSizeText.text = $" Size: {Screen.width} x {Screen.height}";
    }
}
