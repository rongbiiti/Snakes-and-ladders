using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// FPSを表示するクラス
/// </summary>
public class FPSView : MonoBehaviour
{
    /// <summary>
    /// FPSを表示するテキスト
    /// </summary>
    [SerializeField] private Text _fpsText = default;

    /// <summary>
    /// 更新頻度
    /// </summary>
    [SerializeField ]private float _interval = 0.5f;

    /// <summary>
    /// カウント用フレーム
    /// </summary>
    private int _frame;

    /// <summary>
    /// 前の時間
    /// </summary>
    private float _oldTime;

    /// <summary>
    /// フレームレート
    /// </summary>
    private float _frameRate;

    /// <summary>
    /// Awake
    /// </summary>
    private void Awake()
    {
        _oldTime = Time.realtimeSinceStartup;
    }

    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        _frame++;
        var time = Time.realtimeSinceStartup - _oldTime;
        if (time < _interval) { return; }
        _frameRate    = _frame / time;
        _fpsText.text = $" FPS: {_frameRate.ToString("F2")}";
        _oldTime      = Time.realtimeSinceStartup;
        _frame        = 0;
    }
}