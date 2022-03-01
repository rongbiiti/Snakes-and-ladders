using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 音設定UIを表示するボタン
/// </summary>
public class SoundSettingButton : MonoBehaviour
{
    [Header("音設定UI")]
    [SerializeField] private SoundSettingUI m_SoundSettingUI = null;

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        // ボタン登録
        this.gameObject.GetComponent<Button>().onClick.AddListener(() => OnClick_SoundSettingButton());
    }

    /// <summary>
    /// 音設定UI表示するボタン押下時呼び出されるメソッド
    /// </summary>
    private void OnClick_SoundSettingButton()
    {
        AudioManager.I.PlaySe(AudioKey.ButtonSE);
        m_SoundSettingUI.gameObject.SetActive(true);
    }
}
