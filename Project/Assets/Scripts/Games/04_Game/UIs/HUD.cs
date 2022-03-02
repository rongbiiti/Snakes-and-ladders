using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ユーザー情報
/// </summary>
[Serializable]
public class UserInfo
{
    /// <summary>
    /// そのプレイヤーのターンの時表示する枠
    /// </summary>
    [Header("そのプレイヤーのターンの時表示する枠")]
    [SerializeField] private Image m_TurnBorderImage;
    public Image TurnBorderImage {
        get => m_TurnBorderImage;
    }

    
}


/// <summary>
/// HUDクラス
/// </summary>
public class HUD : MonoBehaviour
{
    [Header("ユーザー情報")]
    [SerializeField] private UserInfo[] m_Users = null;
    public UserInfo[] Users => m_Users;

    [Header("メッセージ画像表示用UI")]
    [SerializeField] private Image m_MessageImageUI = null;

    [Header("ターンの画像リスト")]
    [SerializeField] private Sprite[] m_TurnList = null;

    /// <summary>
    /// ターンを設定
    /// </summary>
    /// <param name="turn"></param>
    public void SetTurn(Turn turn)
    {
        if (GameInfo.MyTurn == turn)
        {
            m_MessageImageUI.sprite = m_TurnList[0];
            m_Users[0].TurnBorderImage.gameObject.SetActive(true);
            m_Users[1].TurnBorderImage.gameObject.SetActive(false);
        }
        else if (GameInfo.OpponentTurn == turn)
        {
            m_MessageImageUI.sprite = m_TurnList[1];
            m_Users[0].TurnBorderImage.gameObject.SetActive(false);
            m_Users[1].TurnBorderImage.gameObject.SetActive(true);
        }
    }
}
