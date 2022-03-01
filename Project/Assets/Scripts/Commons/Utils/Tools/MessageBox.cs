using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;

/// <summary>
/// メッセージのタイプ
/// </summary>
public enum MessageType
{
    YesNo,
    Ok,
    MessageOnly,
};

/// <summary>
/// メッセージボックスクラス
/// </summary>
public class MessageBox : MonoBehaviour
{
    /// <summary>
    /// Yesボタン
    /// </summary>
    [SerializeField]
    private Button _yesButton = default;

    /// <summary>
    /// Noボタン
    /// </summary>
    [SerializeField]
    private Button _noButton = default;

    /// <summary>
    /// OKボタン
    /// </summary>
    [SerializeField]
    private Button _okButton = default;

    /// <summary>
    /// 主題テキスト
    /// </summary>
    [SerializeField]
    private Text _subjectText = default;

    /// <summary>
    /// メッセージテキスト
    /// </summary>
    [SerializeField]
    private Text _messageText = default;

    /// <summary>
    /// 「YesButton」押下時、発行されるイベント
    /// </summary>
    private UnityAction _yesEvent = default;

    /// <summary>
    /// 「NoButton」押下時、発行されるイベント
    /// </summary>
    private UnityAction _noEvent = default;

    /// <summary>
    /// [OkButton]押下時、発行されるイベント
    /// </summary>
    private UnityAction _okEvent = default;

    /// <summary>
    /// ボタンが押されているか？
    /// </summary>
    /// <remarks>
    /// TRUE:  押されている
    /// FALSE: 押されていない
    /// </remarks>
    private bool _isPushedButton = false;

    /// <summary>
    /// オブジェクト表示時
    /// </summary>
    private void OnEnable()
    {
        _yesButton.onClick.AddListener(() => OnClick_YesButton());
        _noButton.onClick.AddListener(() => OnClick_NoButton());
        _okButton.onClick.AddListener(() => OnClick_OkButton());
    }

    /// <summary>
    /// メッセージ初期化　Yes/Noボタン表示
    /// </summary>
    /// <param name="subject">主題</param>
    /// <param name="message">メッセージ</param>
    /// <param name="yesEvent">Yesボタン押下時、実行されるメソッド</param>
    /// <param name="noEvent">Noボタン押下時、実行されるメソッド</param>
    public void Initialize_YesNo(string subject, string message, UnityAction yesEvent, UnityAction noEvent)
    {
        _subjectText.text = subject;
        _messageText.text = message;
        _yesEvent = yesEvent;
        _noEvent = noEvent;
        SetMessageType(MessageType.YesNo);
    }

    /// <summary>
    /// メッセージ初期化　Yes/Noボタン表示 (主題無し)
    /// </summary>
    /// <param name="subject">主題</param>
    /// <param name="message">メッセージ</param>
    /// <param name="yesEvent">Yesボタン押下時、実行されるメソッド</param>
    /// <param name="noEvent">Noボタン押下時、実行されるメソッド</param>
    public void Initialize_YesNo(string message, UnityAction yesEvent, UnityAction noEvent)
    {
        _subjectText.text = string.Empty;
        _messageText.text = message;
        _yesEvent = yesEvent;
        _noEvent = noEvent;
        SetMessageType(MessageType.YesNo);
    }

    /// <summary>
    /// メッセージ初期化 Okボタン表示
    /// </summary>
    /// <param name="subject">主題</param>
    /// <param name="message">メッセージ</param>
    /// <param name="okEvent">Okボタン押下時、実行されるメソッド</param>
    public void Initialize_Ok(string subject, string message, UnityAction okEvent)
    {
        _subjectText.text = subject;
        _messageText.text = message;
        _okEvent = okEvent;
        SetMessageType(MessageType.Ok);
    }

    /// <summary>
    /// メッセージ初期化 Okボタン表示（主題無し）
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="okEvent">Okボタン押下時、実行されるメソッド</param>
    public void Initialize_Ok(string message, UnityAction okEvent)
    {
        _subjectText.text = string.Empty;
        _messageText.text = message;
        _okEvent = okEvent;
        SetMessageType(MessageType.Ok);
    }

    /// <summary>
    /// メッセージ初期化 メッセージのみ表示
    /// </summary>
    /// <param name="subject">主題</param>
    /// <param name="message">メッセージ</param>
    ///     /// <param name="destroyTime">メッセージを削除する時間</param>
    public void Initialize_MessageOnly(string subject, string message, float destroyTime)
    {
        _subjectText.text = subject;
        _messageText.text = message;
        SetMessageType(MessageType.MessageOnly);
        StartCoroutine(CloseWindow(destroyTime));
    }

    /// <summary>
    /// メッセージ初期化 メッセージのみ表示（主題無し）
    /// </summary>
    /// <param name="message">メッセージ</param>
    /// <param name="destroyTime">メッセージを削除する時間</param>
    public void Initialize_MessageOnly(string message, float destroyTime)
    {
        _subjectText.text = string.Empty;
        _messageText.text = message;
        SetMessageType(MessageType.MessageOnly);
        StartCoroutine(CloseWindow(destroyTime));
    }

    /// <summary>
    /// メッセージ初期化 メッセージのみ表示（主題無し、破棄無し）
    /// </summary>
    /// <param name="message">メッセージ</param>
    public void Initialize_MessageOnly(string message)
    {
        _subjectText.text = string.Empty;
        _messageText.text = message;
        SetMessageType(MessageType.MessageOnly);
    }

    /// <summary>
    /// メッセージ初期化 メッセージのみ表示（破棄無し）
    /// </summary>
    /// <param name="message">メッセージ</param>
    public void Initialize_MessageOnly(string subject, string message)
    {
        _subjectText.text = subject;
        _messageText.text = message;
        SetMessageType(MessageType.MessageOnly);
    }

    /// <summary>
    /// メッセージのタイプを設定
    /// </summary>
    /// <remarks>
    /// ボタンの表示/非表示を行う
    /// </remarks>
    /// <param name="type"></param>
    private void SetMessageType(MessageType type)
    {
        switch (type)
        {
            case MessageType.MessageOnly:
                _yesButton.gameObject.SetActive(false);
                _noButton.gameObject.SetActive(false);
                _okButton.gameObject.SetActive(false);
                break;
            case MessageType.Ok:
                _yesButton.gameObject.SetActive(false);
                _noButton.gameObject.SetActive(false);
                _okButton.gameObject.SetActive(true);
                break;
            case MessageType.YesNo:
                _yesButton.gameObject.SetActive(true);
                _noButton.gameObject.SetActive(true);
                _okButton.gameObject.SetActive(false);
                break;
        }
    }

    /// <summary>
    /// 「YesButton」押下時、実行するメソッド
    /// </summary>
    public void OnClick_YesButton()
    {
        if (_isPushedButton) { return; }
        _isPushedButton = true;

        if (_yesEvent != null)
        {
            _yesEvent.Invoke();
        }

        AudioManager.I.PlaySe(AudioKey.ButtonSE);
        StartCoroutine(CloseWindow(0.1f));
    }

    /// <summary>
    /// 「NoButton」押下時、実行するメソッド
    /// </summary>
    public void OnClick_NoButton()
    {
        if (_isPushedButton) { return; }
        _isPushedButton = true;

        if (_noEvent != null)
        {
            _noEvent.Invoke();
        }

        AudioManager.I.PlaySe(AudioKey.ButtonSE);
        StartCoroutine(CloseWindow(0.1f));
    }

    /// <summary>
    /// 「OkButton」押下時、実行するメソッド
    /// </summary>
    public void OnClick_OkButton()
    {
        if (_isPushedButton) { return; }
        _isPushedButton = true;

        if (_okEvent != null)
        {
            _okEvent.Invoke();
        }

        AudioManager.I.PlaySe(AudioKey.ButtonSE);
        StartCoroutine(CloseWindow(0.1f));
    }

    /// <summary>
    /// メッセージを閉じる
    /// </summary>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    private IEnumerator CloseWindow(float waitTime)
    {
        yield return new WaitForSecondsRealtime(waitTime);
        Destroy(this.gameObject);
    }
}