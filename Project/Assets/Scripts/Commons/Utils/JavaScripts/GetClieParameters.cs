using UnityEngine;

/// <summary>
/// パラメータ取得
/// UserID GameKey取得
/// </summary>
public class GetClieParameters : MonoBehaviour
{
    /// <summary>
    /// 取得したUserID
    /// </summary>
    public static string m_UserId = null;

    /// <summary>
    /// 取得したGameKey
    /// </summary>
    public static string m_GameKey = null;

    /// <summary>
    /// 取得したPlayerType
    /// </summary>
    public static string m_PlayerType = null;

    /// <summary>
    /// Start
    /// </summary>
    void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            Application.ExternalEval("SendMessage('" + this.transform.root.name + "', 'RecieveText', window.location.search)");
            //Debug.LogError("Application.ExternalEval" + "が呼び出されました");
        }
    }

    /// <summary>
    /// 受信テキスト
    /// </summary>
    /// <param name="text"></param>
    void RecieveText(string text)
    {
        GetQueryString(text);
    }

    /// <summary>
    /// パラメーターを分解するための関数
    /// 今回専用にチューニングしたもの
    /// </summary>
    /// <returns>The query string.</returns>
    string GetQueryString(string text)
    {
        string[] getParams = text.Substring(1).Split('&');
        int len = getParams.Length;

        for (int i = 0; i < len; ++i)
        {
            string[] param = getParams[i].Split('=');

            if (param[0] == "userId")
            {
                m_UserId = param[1];
                Debug.Log("userId:" + param[1]);
            }

            if (param[0] == "progressesId")
            {
                m_GameKey = param[1];
                Debug.Log("progressesId" + param[1]);
            }

            if (param[0] == "isTeacher")
            {
                m_PlayerType = param[1];
                Debug.Log("isTeacher" + param[1]);
            }
        }

        return "";
    }
}