using UnityEngine.UI;
using UnityEngine;
using System.Collections;

/// <summary>
/// ゲームデータを表示する
/// </summary>
public class GameDataView : UWRHelper
{
    [Header("ゲームデータ表示用テキスト")]
    [SerializeField] private Text m_gameDataText = null;

    /// <summary>
    /// コルーチン
    /// </summary>
    private IEnumerator m_Coroutine = null;

    /// <summary>
    /// 表示
    /// </summary>
    private void OnEnable()
    {
        StartCoroutine();
    }

    /// <summary>
    /// 非表示
    /// </summary>
    private void OnDisable()
    {
        EndCoroutine();
    }

    /// <summary>
    /// コルーチン開始
    /// </summary>
    private void StartCoroutine()
    {
        EndCoroutine();
        m_Coroutine = CoGameDataView();
        StartCoroutine(m_Coroutine);
    }

    /// <summary>
    /// コルーチンを終了する
    /// </summary>
    private void EndCoroutine()
    {
        if (m_Coroutine != null)
        {
            StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }
    }

    /// <summary>
    /// サーバー情報のゲームデータを表示
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoGameDataView()
    {
        while (true)
        {
            var uwr = CreateGetUrl(KeyData.GameKey);
            yield return WaitForRequest(uwr);

            if (CheckKey(uwr))
            {
                // Snakes and laddersではなく別のゲームデータがサーバー上に格納されていたら...
                if (CheckDifferenceGameData(uwr))
                {
                    //Debug.Log("Snakes and laddersではなく別のゲームデータがサーバー上にあります。");
                    m_gameDataText.text = "There is another game data on the server\n not Snakes and ladders.";
                }
                // 正常なゲームデータがサーバー上に格納されていたら...
                else
                {
                    GameData gameData = GameData.FromJsonConvert(JsonNode.GetValue(uwr.downloadHandler.text));
                    m_gameDataText.text = gameData.GetStr();
                    m_gameDataText.text += $"MyRoomID: {KeyData.GameKey}\n";
                    m_gameDataText.text += $"MyUserID: {GameInfo.MyUserID}\n";
                    m_gameDataText.text += $"MyTurn: {GameInfo.MyTurn}\n";
                }
            }
            else
            {
                m_gameDataText.text = "No GameData.";
            }

            yield return new WaitForSeconds(DefaultSyncSecond);
        }
    }
}
