using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;

public class Dice : MonoBehaviour
{
    /// <summary>
    /// サイコロの各面の画像
    /// </summary>
    [Header("サイコロの各面の画像")]
    [SerializeField] private Sprite[] m_DiceImages;

    /// <summary>
    /// Image
    /// </summary>
    [Header("Imageコンポーネント")]
    [SerializeField] private Image m_Image;

    /// <summary>
    /// イベントトリガー
    /// </summary>
    [Header("イベントトリガー")]
    [SerializeField] private EventTrigger m_EventTrigger = default;

    /// <summary>
    /// サイコロを振るアニメーションにかける時間
    /// </summary>
    [Header("サイコロを振るアニメーションにかける時間")]
    [SerializeField] private float m_DiceRollAnimTime = 2f;

    /// <summary>
    /// サイコロが止まったあと出目を表示する時間
    /// </summary>
    [Header("サイコロが止まったあと出目を表示する時間")]
    [SerializeField] private float m_DiceNumberDisplayTime = 2f;

    public EventTrigger EventTrigger {
        get => m_EventTrigger;
        set => m_EventTrigger = value;
    }

    private int m_DiceNumber = -1;
    public int DiceNumber {
        get => m_DiceNumber;
        set => m_DiceNumber = value;
    }

    /// <summary>
    /// サイコロを振るアニメーション
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoDiceRollAnimation(int diceNumber)
    {
        // サイコロ抽選結果をセット
        m_DiceNumber = diceNumber;

        // サイコロが止まるまで画像をランダムに変え続ける
        StartCoroutine(CoDiceImageRandomChangeAnim());

        // サイコロジャンプ
        yield return transform.DOLocalJump(
            transform.localPosition,
            250,
            1,
            m_DiceRollAnimTime / 2f
            ).WaitForCompletion();

        //　地面についたあとすこしバウンド
        yield return transform.DOLocalMoveY(
            transform.localPosition.y,
            m_DiceRollAnimTime / 2f
            ).SetEase(Ease.OutBounce).WaitForCompletion();

        

        m_Image.sprite = m_DiceImages[m_DiceNumber - 1];

        yield return new WaitForSeconds(m_DiceNumberDisplayTime);

        gameObject.SetActive(false);
    }

    /// <summary>
    /// サイコロが止まるまでImageをランダムに変え続ける
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoDiceImageRandomChangeAnim()
    {
        float waitTime = 0;

        do
        {
            m_Image.sprite = m_DiceImages[Random.Range(0, m_DiceImages.Length)];
            waitTime += 0.1f;
            yield return new WaitForSeconds(0.1f);

        } while (waitTime < m_DiceRollAnimTime - 0.1f);
    }

    #region EventTrigger

    public void OnPointerDown()
    {
        GameManager.I.OnClick_Dice();
    }

    #endregion
}
