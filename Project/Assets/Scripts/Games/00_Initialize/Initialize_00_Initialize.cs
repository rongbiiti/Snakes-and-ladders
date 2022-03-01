using UnityEngine;
using UnityEditor;

/// <summary>
/// 00_Initializeシーンの初期化
/// </summary>
public class Initialize_00_Initialize : MonoBehaviour
{
    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        // WebGLで設定するとエラーが発生するためフレームレート設定しない。
#if !UNITY_WEBGL
        // フレームレート設定
        QualitySettings.vSyncCount  = 0;
        Application.targetFrameRate = GameInfo.FrameRate;
#endif
    }
}