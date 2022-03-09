using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using DG.Tweening;

public class MissionBox : MonoBehaviour
{
    [SerializeField]
    private Transform _UIParent;

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
        _okButton.onClick.AddListener(() => OnClick_OkButton());
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
    /// メッセージ初期化 Okボタン表示
    /// </summary>
    /// <param name="subject">主題</param>
    /// <param name="message">メッセージ</param>
    /// <param name="okEvent">Okボタン押下時、実行されるメソッド</param>
    public IEnumerator Initialize_Ok(string subject, string message, UnityAction okEvent)
    {        
        gameObject.SetActive(true);
        _subjectText.text = subject;
        _messageText.text = message;
        _okEvent = okEvent;
        SetMessageType(MessageType.Ok);

        _UIParent.localScale = Vector3.zero;
        yield return _UIParent.DOScale(1f, 0.2f).WaitForCompletion();
    }

    /// <summary>
    /// メッセージ初期化 メッセージのみ表示（破棄無し）
    /// </summary>
    /// <param name="message">メッセージ</param>
    public IEnumerator Initialize_MessageOnly(string subject, string message)
    {
        gameObject.SetActive(true);
        _subjectText.text = subject;
        _messageText.text = message;
        SetMessageType(MessageType.MessageOnly);

        _UIParent.localScale = Vector3.zero;
        yield return _UIParent.DOScale(1f, 0.2f).WaitForCompletion();
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
                _okButton.gameObject.SetActive(false);
                break;
            case MessageType.Ok:
                _okButton.gameObject.SetActive(true);
                break;
        }
    }

    /// <summary>
    /// メッセージを閉じる
    /// </summary>
    /// <param name="waitTime"></param>
    /// <returns></returns>
    public IEnumerator CloseWindow(float waitTime)
    {
        yield return _UIParent.DOScale(0f, 0.2f).WaitForCompletion();

        yield return new WaitForSecondsRealtime(waitTime);
        _isPushedButton = false;
        gameObject.SetActive(false);
    }
}
