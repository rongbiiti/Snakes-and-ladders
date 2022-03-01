using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

/// <summary>
/// 音設定UI
/// </summary>
public class SoundSettingUI : MonoBehaviour
{
    [Header("Bgm音量設定用ボタン"), Tooltip("Bgm On/Offを設定出来る")]
    [SerializeField] private Button m_BgmVolumeButton = null;

    [Header("Se音量設定用ボタン"), Tooltip("Se On/Offを設定出来る")]
    [SerializeField] private Button m_SeVolumeButton = null;

    [Header("Bgm音量調整用スライダー")]
    [SerializeField] private Slider m_BgmVolumeSlider = null;

    [Header("Se音量調整用スライダー")]
    [SerializeField] private Slider m_SeVolumeSlider = null;

    [Header("決定ボタン")]
    [SerializeField] private Button m_OkButton = null;

    [Header("サウンドOn/Offに使用するスプライト")]
    [SerializeField] private List<Sprite> m_SoundSpriteList = null;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        // ボタン登録
        m_BgmVolumeButton.onClick.AddListener(() => OnClick_BgmVolumeButton());
        m_SeVolumeButton.onClick.AddListener(() => OnClick_SeVolumeButton());
        m_OkButton.onClick.AddListener(() => OnClick_OkButton());

        // スライダー登録
        m_BgmVolumeSlider.onValueChanged.AddListener((float value) => OnValueChange_BgmVolumeSlider(value));
        m_SeVolumeSlider.onValueChanged.AddListener((float value) => OnValueChange_SeVolumeSlider(value));

        CheckSoundSetting();
    }

    /// <summary>
    /// 音設定の状態を調べる
    /// </summary>
    private void CheckSoundSetting()
    {
        // 現在の音量の値をスライダーに反映
        m_BgmVolumeSlider.value = m_BgmVolumeSlider.maxValue * AudioManager.I.GetBgmVolumeMag();
        m_SeVolumeSlider.value  = m_SeVolumeSlider.maxValue * AudioManager.I.GetSeVolumeMag();

        // 現在の音量の状態をボタンの画像に反映
        m_BgmVolumeButton.GetComponent<Image>().sprite = m_SoundSpriteList[(int)AudioManager.I.GetBgmFlg()];
        m_SeVolumeButton.GetComponent<Image>().sprite  = m_SoundSpriteList[(int)AudioManager.I.GetSeFlg()];
    }

    /// <summary>
    /// Bgm音量設定用ボタン押下時呼び出されるメソッド
    /// Bgm On/Off設定
    /// </summary>
    private void OnClick_BgmVolumeButton()
    {
        var currentBgmFlg = AudioManager.I.GetBgmFlg();

        switch(currentBgmFlg)
        {
            case SoundFlg.ON:
                AudioManager.I.SetBgmFlg(SoundFlg.OFF);
                break;
            case SoundFlg.OFF:
                AudioManager.I.SetBgmFlg(SoundFlg.ON);
                AudioManager.I.PlayBgm(AudioManager.I.CurrentBgMKey, 0.5f);
                break;
        }

        // 画像を切り替える
        m_BgmVolumeButton.GetComponent<Image>().sprite = m_SoundSpriteList[(int)AudioManager.I.GetBgmFlg()];
    }

    /// <summary>
    /// Se音量設定用ボタン押下時呼び出されるメソッド
    /// Se On/Off設定
    /// </summary>
    private void OnClick_SeVolumeButton()
    {
        var currentSeFlg = AudioManager.I.GetSeFlg();

        switch (currentSeFlg)
        {
            case SoundFlg.ON:
                AudioManager.I.SetSeFlg(SoundFlg.OFF);
                break;
            case SoundFlg.OFF:
                AudioManager.I.SetSeFlg(SoundFlg.ON);
                break;
        }

        // 画像を切り替える
        m_SeVolumeButton.GetComponent<Image>().sprite = m_SoundSpriteList[(int)AudioManager.I.GetSeFlg()];
    }

    /// <summary>
    /// Bgm音量調整時に呼び出されるメソッド
    /// </summary>
    private void OnValueChange_BgmVolumeSlider(float value)
    {
        AudioManager.I.SetBgmVolumeMag(value / m_BgmVolumeSlider.maxValue);
        Debug.Log("BGM音量倍率" + AudioManager.I.GetBgmVolumeMag());
    }

    /// <summary>
    /// Se音量調整時に呼び出されるメソッド
    /// </summary>
    private void OnValueChange_SeVolumeSlider(float value)
    {
        AudioManager.I.SetSeVolumeMag(value / m_SeVolumeSlider.maxValue);
        Debug.Log("SE音量倍率" + AudioManager.I.GetSeVolumeMag());
    }

    /// <summary>
    /// 決定ボタン押下時呼び出されるメソッド
    /// 音設定UIを非表示
    /// </summary>
    private void OnClick_OkButton()
    {
        AudioManager.I.PlaySe(AudioKey.ButtonSE);
        this.gameObject.SetActive(false);
    }
}
