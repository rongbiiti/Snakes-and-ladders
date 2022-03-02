using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BoardImage : MonoBehaviour
{
    /// <summary>
    /// 自身のImageコンポーネント
    /// </summary>
    [SerializeField] private Image m_Image;
    public Image Image => m_Image;

    /// <summary>
    /// ボード画像をフェードイン表示
    /// </summary>
    /// <param name="fadeTime"></param>
    /// <returns></returns>
    public IEnumerator CoFadeIn(Sprite sprite, float fadeTime)
    {
        m_Image.sprite = sprite;
        m_Image.enabled = true;
        Color col = m_Image.color;
        m_Image.color = Color.clear;

        yield return  DOTween.ToAlpha(
            () => new Color(1,1,1,0),
            color => m_Image.color = color,
            1f,
            fadeTime
            ).WaitForCompletion();
    }
}
