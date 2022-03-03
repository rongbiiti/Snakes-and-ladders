using System.Collections;
using UnityEngine.UI;
using UnityEngine;

/// <summary>
/// DebugListクラス
/// </summary>
public class DebugList : UWRHelper
{
    [Header("FPS表示用UI")]
    [SerializeField] private GameObject m_FPSView = default;

    [Header("ScreenSize表示用UI")]
    [SerializeField] private GameObject m_ScreenSizeView = default;

    [Header("デバッグコンソール")]
    [SerializeField] private GameObject m_IngameDebugConsole = null;

    [Header("ゲームデータ表示用")]
    [SerializeField] private GameObject m_GameDataView = null;

    [Header("デバッグボタン（m_DebugListButton表示用）")]
    [SerializeField] private Button m_DebugButton = default;

    [Header("デバッグ用ボタンが列挙されているUI")]
    [SerializeField] private GameObject m_DebugListButton = default;

    [Header("FPS/スクリーンサイズ表示用ボタン")]
    [SerializeField] private Button m_FpsAndScreenSizeButton = default;

    [Header("ゲームデータ削除用ボタン")]
    [SerializeField] private Button m_DeleteGameDataButton = default;

    [Header("コンソール表示用ボタン")]
    [SerializeField] private Button m_DisplayConsoleButton = null;

    [Header("ゲームデータ表示用ボタン")]
    [SerializeField] private Button m_DisplayGameDataButton = null;

    [SerializeField, Header("URLドロップダウンリスト")]
    private Dropdown m_URLDropDown = null;

    [SerializeField, Header("サイコロの出目操作リスト")]
    private Dropdown m_DiceNumberController = null;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        // 各ボタン登録
        m_DebugButton.onClick.AddListener(() => OnClick_DebugButton());
        m_FpsAndScreenSizeButton.onClick.AddListener(() => OnClick_FpsAndScreenSizeButton());
        m_DeleteGameDataButton.onClick.AddListener(() => StartCoroutine(CoAllDeleteData()));
        m_DisplayConsoleButton.onClick.AddListener(() => OnClick_DisplayConsoleButton());
        m_DisplayGameDataButton.onClick.AddListener(() => OnClick_DisplayGameDataButton());
        m_URLDropDown.onValueChanged.AddListener((int value) => OnValueChanged_URLDropDown(value));
        m_DiceNumberController.onValueChanged.AddListener((int value) => OnValueChanged_DiceNumberController(value));
        m_URLDropDown.value = (int)GameInfo.URLType;
    }

    /// <summary>
    /// デバッグボタン押下時、デバッグボタンリスト表示
    /// </summary>
    private void OnClick_DebugButton()
    {
        m_DebugListButton.SetActive(!m_DebugListButton.activeSelf);
    }

    /// <summary>
    /// 全てのデータを削除
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoAllDeleteData()
    {
        SetIntaractableButton(false);

        var msgBox = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
        msgBox.GetComponent<MessageBox>().Initialize_MessageOnly("Delete", "Delete GameData");

        // ゲームデータ削除
        var uwr = CreateDeletUrl(KeyData.GameKey);
        yield return WaitForRequest(uwr);

        //Debug.Log("全てのデータ削除");

        Destroy(msgBox);
        SetIntaractableButton(true);
    }

    /// <summary>
    /// FPS/スクリーンサイズ 表示/非表示
    /// </summary>
    private void OnClick_FpsAndScreenSizeButton()
    {
        SetIntaractableButton(false);

        m_FPSView.SetActive(!m_FPSView.gameObject.activeSelf);
        m_ScreenSizeView.SetActive(!m_ScreenSizeView.gameObject.activeSelf);

        if(m_FPSView.gameObject.activeSelf)
        {
            m_FpsAndScreenSizeButton.GetComponent<Image>().color = Color.red;
        }
        else
        {
            m_FpsAndScreenSizeButton.GetComponent<Image>().color = Color.white;
        }

        SetIntaractableButton(true);
    }


    /// <summary>
    /// デバッグコンソール　表示/非表示
    /// </summary>
    private void OnClick_DisplayConsoleButton()
    {
        SetIntaractableButton(false);

        m_IngameDebugConsole.SetActive(!m_IngameDebugConsole.activeSelf);

        if (m_IngameDebugConsole.activeSelf)
        {
            m_DisplayConsoleButton.GetComponent<Image>().color = Color.red;
        }
        else
        {
            m_DisplayConsoleButton.GetComponent<Image>().color = Color.white;
        }

        SetIntaractableButton(true);
    }

    /// <summary>
    /// ゲームデータ　表示/非表示
    /// </summary>
    private void OnClick_DisplayGameDataButton()
    {
        SetIntaractableButton(false);

        m_GameDataView.SetActive(!m_GameDataView.activeSelf);

        if (m_GameDataView.activeSelf)
        {
            m_DisplayGameDataButton.GetComponent<Image>().color = Color.red;
        }
        else
        {
            m_DisplayGameDataButton.GetComponent<Image>().color = Color.white;
        }

        SetIntaractableButton(true);
    }

    /// <summary>
    /// 環境変更（URL変更）
    /// </summary>
    /// <param name="value"></param>
    private void OnValueChanged_URLDropDown(int value)
    {
        var type = (URLType)value;
        GameInfo.URLType = type;
        Debug.Log("環境" + GameInfo.URLType);
    }

    /// <summary>
    /// サイコロの出目操作（不正）
    /// </summary>
    /// <param name="value"></param>
    private void OnValueChanged_DiceNumberController(int value)
    {
        GameInfo.ControllDiceNumber = value;

        if(value == 0)
        {
            Debug.Log("次から通常通りのダイスロール");
        }
        else
        {
            Debug.Log("次のダイスロールで " + value + " が出る");
        }
        
    }


    /// <summary>
    /// ボタン反応　有り/無し
    /// </summary>
    /// <param name="enabled"></param>
    private void SetIntaractableButton(bool enabled)
    {
        m_DeleteGameDataButton.interactable   = enabled;
        m_FpsAndScreenSizeButton.interactable = enabled;
        m_DisplayConsoleButton.interactable   = enabled;
        m_DisplayGameDataButton.interactable  = enabled;
    }
}
