using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Board
{
    [Header("ボードの画像")]
    public Sprite m_BoardTexture;

    /// <summary>
    /// 蛇の顔と尻尾
    /// </summary>
    [Serializable]
    public class SneakSquare
    {
        [Header("蛇の頭と尻尾のマス")]
        public int[] m_SneakSquare;
    }

    /// <summary>
    /// 梯子の根本と頂上
    /// </summary>
    [Serializable]
    public class LaddersSquare
    {
        [Header("梯子の根元と頂上のマス")]
        public int[] m_LaddersSquare;
    }

    /// <summary>
    /// 蛇の顔と尻尾の組み合わせの配列
    /// </summary>
    [Header("蛇の顔と尻尾の組み合わせの配列")]
    public SneakSquare[] m_SneakSquares;

    /// <summary>
    /// 梯子の根元と頂上の組み合わせの配列
    /// </summary>
    [Header("梯子の根元と頂上の組み合わせの配列")]
    public LaddersSquare[] m_LaddersSquares;

    /// <summary>
    /// ミッションマスの配列
    /// </summary>
    [Header("ミッションマス")]
    public int[] m_MissionSquares;

}
