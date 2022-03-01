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

    /// <summary>
    /// プレイヤーがいるマス目
    /// </summary>
    private int m_SquareNum = 0;
    public int SquareNum => m_SquareNum;

    /// <summary>
    /// プレイヤーマスX軸
    /// </summary>
    private int m_SquareX = 0;
    

    /// <summary>
    /// プレイヤーマスY軸
    /// </summary>
    private int m_SquareY = 0;

    /// <summary>
    /// プレイヤーがいるマス目を元にコマをボード上に表示
    /// </summary>
    /// <param name="SquareNum">マス目</param>
    public void SetPieceSquarePos(int SquareNum)
    {
        m_SquareNum = SquareNum;

        m_SquareY = GameData.Height - (m_SquareNum - 1) / GameData.Width;

        // Yが偶数なら...
        if(m_SquareY % 2 == 0)
        {
            m_SquareX = 10 % (GameData.Height - 10 % m_SquareNum);
        }
        // Yが奇数なら...
        else
        {
            m_SquareX = 10 % m_SquareNum - 1;
        }
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
