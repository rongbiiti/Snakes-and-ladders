using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// ID設定用(Debug)
/// </summary>
public class DebugSetID : MonoBehaviour
{
    /// <summary>
    /// 決定ボタン
    /// </summary>
    [SerializeField] private Button m_OkButton = default;

    /// <summary>
    /// RoomID入力用
    /// </summary>
    [SerializeField] private InputField m_RoomIDField = default;

    /// <summary>
    /// UserID入力用
    /// </summary>
    [SerializeField] private InputField m_UserIDField = default;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        GameInfo.MyUserID = Random.Range(1111, 10000).ToString();

        m_RoomIDField.text = KeyData.GameKey;
        m_UserIDField.text = GameInfo.MyUserID;
        m_OkButton.onClick.AddListener(() => OnClick_OkButton());
    }

    /// <summary>
    /// Okボタン押下時呼び出されるメソッド
    /// </summary>
    private void OnClick_OkButton()
    {
        KeyData.GameKey   = m_RoomIDField.text;
        GameInfo.MyUserID = m_UserIDField.text;
    }
}