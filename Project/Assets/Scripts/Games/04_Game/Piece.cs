﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Piece : MonoBehaviour
{
    /// <summary>
    /// マスが1のときの座標
    /// </summary>
    [Header("マスが1のときの座標")]
    [SerializeField] private Vector2 m_StartPos;

    /// <summary>
    /// オフセット
    /// </summary>
    [Header("オフセット")]
    [SerializeField] private Vector2 m_Offset;

    /// <summary>
    /// 上へ移動するときにかける時間
    /// </summary>
    [Header("上へ移動するときにかける時間")]
    [SerializeField] private float m_MoveUpTime = 0.25f;

    /// <summary>
    /// 横方向へ移動するときにかける時間
    /// </summary>
    [Header("横方向へ移動するときにかける時間")]
    [SerializeField] private float m_MoveHorizontalTime = 0.5f;

    /// <summary>
    /// 今いるマス
    /// </summary>
    private int m_SquareNumber = 0;
    public int SquareNumber => m_SquareNumber;

    /// <summary>
    /// プレイヤーマスX軸
    /// </summary>
    private int m_SquareX = 0;

    /// <summary>
    /// プレイヤーマスY軸
    /// </summary>
    private int m_SquareY = 0;

    /// <summary>
    /// 再起動時のセットアップで呼ばれる
    /// </summary>
    /// <param name="squareNum"></param>
    public void Acquired(int squareNum)
    {
        m_SquareNumber = squareNum;

        CalcSquareXY(squareNum);

        float posX = m_StartPos.x + m_Offset.x * m_SquareX;
        float posY = m_StartPos.y + m_Offset.y * (GameData.Height - m_SquareY - 1);
        Vector2 newPos = new Vector2(posX, posY);

        transform.localPosition = newPos;
    }

    /// <summary>
    /// マス目を元に、ボード上でのXY座標を計算
    /// </summary>
    /// <param name="squareNum">今いるマス</param>
    public void CalcSquareXY(int squareNum)
    {
        // Y軸
        m_SquareY = (GameData.Height - 1) - (squareNum - 1) / GameData.Width;
        Debug.Log("Y軸 : " + m_SquareY);

        // Yが偶数なら...
        if (m_SquareY % 2 == 0)
        {
            m_SquareX = (GameData.Height - (squareNum % 10)) % 10;
            Debug.Log("Yは偶数でX軸 : " + m_SquareX);
        }
        // Yが奇数なら...
        else
        {
            m_SquareX = (squareNum % 10) - 1 ;
            Debug.Log("Yは奇数でX軸 : " + m_SquareX);
        }

    }

    /// <summary>
    /// Canvas上の座標を返す
    /// </summary>
    /// <param name="squareX">X軸</param>
    /// <param name="squareY">Y軸</param>
    /// <returns></returns>
    private Vector2 CalcBoardPos(int squareX, int squareY)
    {
        float posX = m_StartPos.x + m_Offset.x * squareX;
        float posY = m_StartPos.y + m_Offset.y * (GameData.Height - m_SquareY - 1);
        Vector2 newPos = new Vector2(posX, posY);

        return newPos;
    }

    /// <summary>
    /// 一手目限定のコマ移動
    /// </summary>
    /// <param name="squareNum"></param>
    /// <returns></returns>
    public IEnumerator CoFirstPieceMove(int squareNum)
    {
        // まずXYのマス目を計算
        CalcSquareXY(squareNum);

        yield return transform.DOLocalMove(CalcBoardPos(m_SquareX, m_SquareY), 0.5f).WaitForCompletion();
    }

    /// <summary>
    /// コマをジグザグに移動させる
    /// </summary>
    /// <param name="squareNum">今いるマス</param>
    /// <param name="moveSquare">このマス分進む</param>
    /// <returns></returns>
    public IEnumerator CoPieceMove(int squareNum, int moveSquare)
    {
        int movePoint = moveSquare;

        // まず今のY軸が偶数かつX軸が左端か、Y軸が奇数かつX軸が右端かチェック
        if (CheckNowSquareBoardEdge(m_SquareX, m_SquareY))
        {
            --movePoint;
            --m_SquareY;
            Debug.Log("1マス上に" + movePoint);
            yield return CoPieceMoveUp();
        }

        // これ以上進めなくなったらbreak
        if (movePoint <= 0) yield break;

        // 進める分だけ横方向へ移動
        // Yが偶数なら…
        if (m_SquareY % 2 == 0)
        {
            // X左へ
            int tmpX = m_SquareX;
            tmpX -= movePoint;
            movePoint -= Mathf.Abs(m_SquareX - tmpX);

            // 左端を超えないようにする
            if (tmpX < 0)
            {
                movePoint = Mathf.Abs(tmpX);
                tmpX = 0;
            }
            Debug.Log("Y偶数なので左へ" + movePoint);
            m_SquareX = tmpX;

            yield return CoPieceMoveHorizontal(m_SquareX, m_SquareY);
        }
        // Yが奇数なら…
        else
        {
            // X右へ
            int tmpX = m_SquareX;
            tmpX += movePoint;
            movePoint -= Mathf.Abs(m_SquareX - tmpX);

            // 右端を超えないようにする
            if (GameData.Width - 1 < tmpX)
            {
                movePoint = tmpX - (GameData.Width - 1);
                tmpX = (GameData.Width - 1);
            }
            Debug.Log("Y奇数なので右へ" + movePoint);
            m_SquareX = tmpX;
            yield return CoPieceMoveHorizontal(m_SquareX, m_SquareY);
        }

        // これ以上進めなくなったらbreak
        if (movePoint <= 0) yield break;

        // まだmovePointが余ってたら †自分を呼ぶ†
        Debug.Log("Point余ってるので再帰処理" + movePoint);
        yield return CoPieceMove(squareNum, movePoint);
    }

    /// <summary>
    /// コマ上方向へ移動
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoPieceMoveUp()
    {
        yield return transform.DOLocalMoveY(CalcBoardPos(m_SquareX, m_SquareY).y, m_MoveUpTime).WaitForCompletion();
    }

    /// <summary>
    /// Y軸が偶数かつX軸が左端か、Y軸が奇数かつX軸が右端かチェック
    /// </summary>
    /// <param name="squareX">X軸</param>
    /// <param name="squareY">Y軸</param>
    /// <returns></returns>
    private bool CheckNowSquareBoardEdge(int squareX, int squareY)
    {
        // Y軸が偶数なら…
        if(squareY % 2 == 0)
        {
            // X軸が左端かチェック
            if(squareX == 0)
            {
                return true;
            }
            
        }
        // Y軸が奇数なら…
        else
        {
            // X軸が右端かチェック
            if(squareX == GameData.Width - 1)
            {
                return true;
            }
            
        }

        return false;
    }

    /// <summary>
    /// 横方向へコマ移動
    /// </summary>
    /// <param name="moveSquareX">移動先のX軸</param>
    /// <param name="squareY">今いるY軸</param>
    /// <returns></returns>
    private IEnumerator CoPieceMoveHorizontal(int moveSquareX, int squareY)
    {
        yield return transform.DOLocalMoveX(CalcBoardPos(moveSquareX, squareY).x, m_MoveHorizontalTime).WaitForCompletion();
    }
}
