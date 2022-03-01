using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 結果表示用UI
/// </summary>
public class ResultUI : MonoBehaviour
{
    [Header("ゲームデータ同期")]
    [SerializeField] private GameDataSync m_GameDataSync = null;

    [Header("勝利演出用UI")]
    [SerializeField] private GameObject m_WinUI = default;

    [Header("敗北演出用UI")]
    [SerializeField] private GameObject m_LoseUI = default;

    [Header("リトライボタン")]
    [SerializeField] private Button m_RetryButton = default;

    [Header("ゲーム終了ボタン")]
    [SerializeField] private Button m_GameEndButton = default;

    [Header("ゲーム終了UI")]
    [SerializeField] private GameObject m_GameEndUI = default;

    /// <summary>
    /// Start
    /// </summary>
    private IEnumerator Start()
    {
        // ボタン非表示
        m_GameEndUI.SetActive(false);
        m_RetryButton.gameObject.SetActive(false);
        m_GameEndButton.gameObject.SetActive(false);

        // ゲーム同期を止める。
        m_GameDataSync.StopGameSync();

        // 結果画面で対戦相手がリトライボタンを押したら
        // 自身も出るようにしている。
        m_GameDataSync.StartCheckResult();

        // 両プレイヤーがリザルト画面に遷移するまで待機する。
        yield return CoWaitResult();

        // ボタン登録、表示
        m_RetryButton.gameObject.SetActive(true);
        m_GameEndButton.gameObject.SetActive(true);
        m_RetryButton.onClick.AddListener(() => StartCoroutine(CoOnClick_RetryButton()));
        m_GameEndButton.onClick.AddListener(() => StartCoroutine(CoOnClick_GameEndButton()));
    }

    /// <summary>
    /// ボタン押下時、リトライ
    /// </summary>
    private IEnumerator CoOnClick_RetryButton()
    {
        SetIntaractableButton(false);
        AudioManager.I.PlaySe(AudioKey.ButtonSE);
        yield return m_GameDataSync.CoDeleteGameData();
        SceneFadeManager.I.Load(SceneName.Room);
    }

    /// <summary>
    /// ゲーム終了ボタン
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoOnClick_GameEndButton()
    {
        m_GameDataSync.StopGameSync();
        Debug.Log($"ゲーム終了\n結果 勝利:{GameInfo.WinCount} 敗北:{GameInfo.LoseCount}");
        SetIntaractableButton(false);
        AudioManager.I.PlaySe(AudioKey.ButtonSE);
        m_GameEndUI.SetActive(true);
        this.gameObject.SetActive(false);

        yield break;
    }

    /// <summary>
    /// ボタン反応 表示/非表示
    /// </summary>
    private void SetIntaractableButton(bool enabled)
    {
        m_RetryButton.interactable    = enabled;
        //m_GameEndButton.interactable  = enabled;
    }

    /// <summary>
    /// 勝利
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoWin()
    {
        ++GameInfo.WinCount;
        Debug.Log(
            "勝利回数:" + GameInfo.WinCount + "\n" +
            "敗北回数:" + GameInfo.LoseCount + "\n");
        AudioManager.I.PlaySe(AudioKey.WinSE);
        this.gameObject.SetActive(true);
        m_WinUI.SetActive(true);
        m_LoseUI.SetActive(false);

        // クアドラサーバーじゃなければ
        // 結果を送信
        if (GameInfo.URLType != URLType.Quadra)
        {
            yield return m_GameDataSync.CoUpdateResult(
                 userId: GameInfo.MyUserID,
                 roomId: KeyData.GameKey,
                 winCount: GameInfo.WinCount,
                 loseCount: GameInfo.LoseCount,
                 applicationName: GameInfo.ApplicationName);
        }
        yield break;
    }

    /// <summary>
    /// 敗北
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoLose()
    {
        ++GameInfo.LoseCount;
        Debug.Log(
            "勝利回数:" + GameInfo.WinCount + "\n" +
            "敗北回数:" + GameInfo.LoseCount + "\n");
        AudioManager.I.PlaySe(AudioKey.LoseSE);
        this.gameObject.SetActive(true);
        m_WinUI.SetActive(false);
        m_LoseUI.SetActive(true);

        // クアドラサーバーじゃなければ
        // 結果を送信
        if (GameInfo.URLType != URLType.Quadra)
        {
            yield return m_GameDataSync.CoUpdateResult(
                 userId: GameInfo.MyUserID,
                 roomId: KeyData.GameKey,
                 winCount: GameInfo.WinCount,
                 loseCount: GameInfo.LoseCount,
                 applicationName: GameInfo.ApplicationName);
        }
        yield break;
    }

    /// <summary>
    /// 両プレイヤーがリザルト画面に遷移するまで待機する。
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoWaitResult()
    {
        m_RetryButton.gameObject.SetActive(false);
        m_GameEndButton.gameObject.SetActive(false);
        yield return m_GameDataSync.CoWaitResult();
    }
}