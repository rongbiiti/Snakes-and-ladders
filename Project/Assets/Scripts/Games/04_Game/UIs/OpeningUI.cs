using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using DG.Tweening;

/// <summary>
/// オープニング演出UI
/// </summary>
public class OpeningUI : MonoBehaviour
{
    [Header("画像")]
    [SerializeField] private Image m_Image = default;

    /// <summary>
    /// 演出処理
    /// </summary>
    /// <param name="fadeInTime"></param>
    /// <param name="fadeOutTime"></param>
    /// <returns></returns>
    public IEnumerator CoProduction(float fadeInTime, float fadeOutTime)
    {
        // 初期化
        this.gameObject.SetActive(true);
        m_Image.color = new Color(1f, 1f, 1f, 0f);
        m_Image.gameObject.transform.localPosition = new Vector3(0, 300, 0);

        m_Image.transform.DOLocalMoveY(0f, fadeInTime);
        yield return new WaitForSeconds(0.1f);
        yield return m_Image.DOFade(1f, fadeInTime).WaitForCompletion();

        // ゲームスタートSE再生
        AudioManager.I.PlaySe(AudioKey.GameStartSE);

        yield return new WaitForSeconds(1f);
        yield return m_Image.DOFade(0f, fadeOutTime).WaitForCompletion();
        this.gameObject.SetActive(false);
    }
}