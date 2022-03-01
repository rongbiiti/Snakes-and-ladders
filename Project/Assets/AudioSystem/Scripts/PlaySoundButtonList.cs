using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 音を再生させるボタンのリスト
/// </summary>
public class PlaySoundButtonList : MonoBehaviour
{
    [SerializeField] private Button m_TestBgm1Button = null;
    [SerializeField] private Button m_TestBgm2Button = null;
    [SerializeField] private Button m_TestSe1Button  = null;
    [SerializeField] private Button m_TestSe2Button  = null;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        m_TestBgm1Button.onClick.AddListener(() => OnClick_TestBgm1Button());
        m_TestBgm2Button.onClick.AddListener(() => OnClick_TestBgm2Button());
        m_TestSe1Button.onClick.AddListener(() => OnClick_TestSe1Button());
        m_TestSe2Button.onClick.AddListener(() => OnClick_TestSe2Button());
    }

    /// <summary>
    /// TestBgm1再生
    /// </summary>
    private void OnClick_TestBgm1Button()
    {
        //AudioManager.I.PlayBgm(AudioKey.TestBgm1);
    }

    /// <summary>
    /// TestBgm2再生
    /// </summary>
    private void OnClick_TestBgm2Button()
    {
        //AudioManager.I.PlayBgm(AudioKey.TestBgm2);
    }

    /// <summary>
    /// TestSe1再生
    /// </summary>
    private void OnClick_TestSe1Button()
    {
        //AudioManager.I.PlaySe(AudioKey.TestSe1);
    }

    /// <summary>
    /// TestSe2再生
    /// </summary>
    private void OnClick_TestSe2Button()
    {
        //AudioManager.I.PlaySe(AudioKey.TestSe2);
    }
}


