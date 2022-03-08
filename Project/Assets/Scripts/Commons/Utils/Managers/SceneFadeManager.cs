using System;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

/// <summary>
/// シーン名
/// </summary>
public class SceneName
{
    /// <summary>
    /// 00_Initializeシーン名
    /// </summary>
    public const string Initialize = "00_Initialize";

    /// <summary>
    /// 01_Splashシーン名
    /// </summary>
    //public const string Splash = "01_Splash";

    /// <summary>
    /// 02_Titleシーン名
    /// </summary>
    public const string Title = "02_Title";

    /// <summary>
    /// 03_Roomシーン名
    /// </summary>
    public const string Room = "03_Room";

    /// <summary>
    /// 04_Gameシーン名
    /// </summary>
    public const string Game = "04_Game";

    /// <summary>
    /// 05_プレイヤータイプ取得シーン
    /// </summary>
    public const string GetPlayerType = "05_GetPlayerType";

    ///// <summary>
    ///// 99_Debugシーン名
    ///// </summary>
    //public const string Debug = "99_Debug";
}


/// <summary>
/// シーン管理クラス
/// </summary>
public class SceneFadeManager : SingletonMonoBehaviour<SceneFadeManager>
{
    /// <summary>
    /// フェード画像
    /// </summary>
    [SerializeField, Header("フェードの画像")]
    private Image _fadeImage = default;

    /// <summary>
    /// フェード中の透明度
    /// </summary>
    private float _fadeAlpha = 0.0f;

    /// <summary>
    /// フェード中かどうか？
    /// </summary>
    private bool _isFading = false;
    public  bool IsFading => _isFading;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        _fadeImage.raycastTarget = false;
        _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, 0f);
    }

    /// <summary>
    /// 画面遷移
    /// </summary>
    /// <param name="scene">シーン名</param>
    /// <param name="interval">暗転にかかる時間(秒)</param>
    /// <param name="ButtonRing">SEを鳴らすかどうか</param>
    public void Load(string sceneName, float interval = 0.5f, bool ButtonRing = false)
    {
        try
        {
            StartCoroutine(TransScene(sceneName, interval));
        }
        catch (Exception ex)
        {
            GameObject msgBox = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
            msgBox.GetComponent<MessageBox>().Initialize_Ok("Scene transition failed.\n" + ex.Message + "\n" + ex.StackTrace, null);
        }
    }

    /// <summary>
    /// シーン遷移用コルーチン 
    /// </summary>
    /// <param name='scene'>シーン名</param>
    /// <param name='interval'>暗転にかかる時間(秒)</param>
    private IEnumerator TransScene(string scene, float interval)
    {
        // 2度読み防止
        if (_isFading) { yield break; }

        // だんだん暗く 
        _isFading = true;
        _fadeImage.raycastTarget = true;
        float time = 0;
        while (_fadeAlpha < 1f)
        {
            _fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
            _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, _fadeAlpha);
            time += Time.deltaTime;
            yield return null;
        }

        // シーン非同期ロード
        yield return SceneManager.LoadSceneAsync(scene);

        // だんだん明るく 
        time = 0;
        while (_fadeAlpha > 0f)
        {
            _fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
            _fadeImage.color = new Color(_fadeImage.color.r, _fadeImage.color.g, _fadeImage.color.b, _fadeAlpha);
            time += Time.deltaTime;
            yield return null;
        }

        _fadeImage.raycastTarget = false;
        _isFading = false;
    }

}