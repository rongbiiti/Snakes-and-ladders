using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PopUpText : MonoBehaviour
{
    [SerializeField] private Image m_Image;

    private void Start()
    {
        m_Image.color = Color.clear;
    }

    public IEnumerator CoSnakePopUp(Vector3 pos)
    {
        transform.localPosition = pos;
        m_Image.color = Color.white;
        transform.localRotation = Quaternion.Euler(0, 0, -90);
        transform.localScale = Vector3.zero;

        yield return transform.DOScale(1f, 1f);
        yield return transform.DORotate(Vector3.zero, 1.5f).SetEase(Ease.OutBounce).WaitForCompletion();

        yield return new WaitForSeconds(0.5f);

        m_Image.color = Color.clear;

    }

    public IEnumerator CoLadderPopUp(Vector3 pos)
    {
        transform.localPosition = pos;
        m_Image.enabled = true;
        Color col = Color.white;
        m_Image.color = Color.clear;

        yield return DOTween.ToAlpha(
            () => new Color(1, 1, 1, 0),
            color => m_Image.color = color,
            1f,
            0.75f
            );

        yield return transform.DOLocalMoveY(10f, 1f).WaitForCompletion();
        yield return new WaitForSeconds(0.5f);

        m_Image.color = Color.clear;
    }
}
