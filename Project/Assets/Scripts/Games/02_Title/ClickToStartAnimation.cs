using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ClickToStartAnimation
/// </summary>
public class ClickToStartAnimation : MonoBehaviour
{
    [Header("画像")]
    [SerializeField] private Image m_TapToStartImage = default;

    [Header("点滅時間")]
    [SerializeField] private float m_FlashingTime = 1f;

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        // アニメーション再生
        m_TapToStartImage
            .DOFade(0f, m_FlashingTime)
            .SetEase(Ease.InCubic)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(this.gameObject);
    }
}
