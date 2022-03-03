using System.Collections;
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
    /// 蛇の頭から尻尾へ移動するときにかける時間
    /// </summary>
    [Header("蛇の頭から尻尾へ移動するときにかける時間")]
    [SerializeField] private float m_SnakeSquareMoveTime = 1.5f;

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
    /// マスを指定して、X、Y、ローカル座標を合わせる
    /// </summary>
    /// <param name="squareNum"></param>
    public void SetLocalPosition(int squareNum)
    {
        m_SquareNumber = squareNum;

        if(squareNum == 0)
        {
            return;
        }

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
        Debug.Log("Y : " + m_SquareY);

        // Yが偶数なら...
        if (m_SquareY % 2 == 0)
        {
            m_SquareX = (GameData.Height - (squareNum % 10)) % 10;
            Debug.Log("Yは偶数 X : " + m_SquareX);
        }
        // Yが奇数なら...
        else
        {
            m_SquareX = (squareNum % 10) - 1 ;
            Debug.Log("Yは奇数 X : " + m_SquareX);
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
        float posY = m_StartPos.y + m_Offset.y * (GameData.Height - squareY - 1);
        Vector2 newPos = new Vector2(posX, posY);

        return newPos;
    }

    /// <summary>
    /// 一手目限定のコマ移動
    /// 梯子を移動するときにも使う
    /// </summary>
    /// <param name="destination">移動先のマス目</param>
    /// <returns></returns>
    public IEnumerator CoFirstPieceMove(int destination)
    {
        m_SquareNumber = destination;

        // まずXYのマス目を計算
        CalcSquareXY(destination);

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
            
            yield return CoPieceMoveUp();
        }

        // これ以上進めなくなったらbreak
        if (movePoint <= 0)
        {
            m_SquareNumber += moveSquare;
            yield break;
        }

        // 進める分だけ横方向へ移動
        // Yが偶数なら…
        if (m_SquareY % 2 == 0)
        {
            // 左方向へ移動、Xを引く
            int tmpX = m_SquareX;
            tmpX -= movePoint;
            movePoint -= Mathf.Abs(m_SquareX - tmpX);

            // 左端を超えないようにする
            if (tmpX < 0)
            {
                movePoint = Mathf.Abs(tmpX);
                tmpX = 0;
            }
            m_SquareX = tmpX;

            yield return CoPieceMoveHorizontal(m_SquareX, m_SquareY);
        }
        // Yが奇数なら…
        else
        {
            // 右方向へ移動、Xに足す
            int tmpX = m_SquareX;
            tmpX += movePoint;
            movePoint -= Mathf.Abs(m_SquareX - tmpX);

            // 右端を超えないようにする
            if ((GameData.Width - 1) < tmpX)
            {
                movePoint = tmpX - (GameData.Width - 1);
                tmpX = (GameData.Width - 1);
            }
            m_SquareX = tmpX;

            yield return CoPieceMoveHorizontal(m_SquareX, m_SquareY);
        }

        // これ以上進めなくなったらbreak
        if (movePoint <= 0)
        {
            m_SquareNumber += moveSquare;
            yield break;
        }

        // 左上のマスに着いてたらbreak
        if (m_SquareY == 0 && m_SquareX == 0) yield break;

        // まだmovePointが余ってたら †自分を呼ぶ†
        m_SquareNumber += moveSquare - movePoint;
        yield return CoPieceMove(m_SquareNumber, movePoint);
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

    /// <summary>
    /// 蛇の頭から尻尾へ移動する演出
    /// </summary>
    /// <param name="destination">移動先のマス</param>
    /// <returns></returns>
    public IEnumerator CoPieceMove_SnakeSquare(int destination)
    {
        // 値を格納しておく
        m_SquareNumber = destination;

        // 小さくなる（蛇に吸い込まれるイメージ）
        yield return transform.DOScale(
            0f,
            m_SnakeSquareMoveTime / 2f
            ).WaitForCompletion();

        // 移動先のマスに転移
        SetLocalPosition(destination);

        // 大きくなる
        yield return transform.DOScale(
            1f,
            m_SnakeSquareMoveTime / 2f
            ).SetEase(Ease.OutBounce).WaitForCompletion();

    }

   
}
