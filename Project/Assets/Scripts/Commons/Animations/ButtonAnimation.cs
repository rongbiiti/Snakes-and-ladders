using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// ボタンアニメーション
/// </summary>
public class ButtonAnimation : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    /// <summary>
    /// サイズ変更アニメーション再生に掛かる時間
    /// </summary>
    private float ChangeAnimationTime = 0.2f;

    /// <summary>
    /// RectTransformのキャッシュ
    /// </summary>
    private RectTransform m_RectTransform = null;

    /// <summary>
    /// ボタンのデフォルトサイズ
    /// </summary>
    private Vector3 m_DefalutSize = Vector3.one;

    /// <summary>
    /// 拡大したサイズ
    /// </summary>
    private Vector3 m_ZoomSize = Vector3.one * 1.2f;

    /// <summary>
    /// Tween
    /// </summary>
    private Tween m_Tween = default;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        m_RectTransform = GetComponent<RectTransform>();
        m_RectTransform.localScale = m_DefalutSize;
    }

    /// <summary>
    /// ボタンにポインタが乗った瞬間呼び出されるメソッド
    /// </summary>
    /// <param name="eventData"></param>

    public void OnPointerEnter(PointerEventData eventData)
    {
        ChangeSizeAnimation(m_ZoomSize, ChangeAnimationTime);
    }

    /// <summary>
    /// ボタンにポインタが離れた瞬間呼び出されるメソッド
    /// </summary>
    /// <param name="eventData"></param>
    public void OnPointerExit(PointerEventData eventData)
    {
        ChangeSizeAnimation(m_DefalutSize, ChangeAnimationTime);
    }

    /// <summary>
    /// サイズを変更するアニメーション
    /// </summary>
    /// <param name="size">目標のサイズ</param>
    /// <param name="time">時間</param>
    public void ChangeSizeAnimation(Vector2 targetSize, float time)
    {
        // 現在再生中のアニメーション終了
        // 再度アニメーション再生（ゲームオブジェクト破棄時アニメーション終了）
        m_Tween.Kill();
        m_Tween = m_RectTransform.DOScale(targetSize, time).SetLink(this.gameObject);
    }

}
