using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ユーザー情報
/// </summary>
[Serializable]
public class UserInfo
{
    [Header("プレイヤーがいるマス目表示用Text")]
    [SerializeField] private Text m_SquareNumText = null;

    /// <summary>
    /// プレイヤーがいるマス目
    /// </summary>
    private int m_SquareNum = 0;
    public int PairNum => m_SquareNum;

    /// <summary>
    /// プレイヤーがいるマス目をTextに表示する
    /// </summary>
    /// <param name="SquareNum">追加する数値</param>
    public void SetSquareNumText(int SquareNum)
    {
        m_SquareNum += SquareNum;
        m_SquareNumText.text = m_SquareNum.ToString();
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
        }
        else if (GameInfo.OpponentTurn == turn)
        {
            m_MessageImageUI.sprite = m_TurnList[1];
        }
    }
}
