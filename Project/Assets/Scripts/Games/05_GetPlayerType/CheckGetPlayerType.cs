using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CheckGetPlayerType : MonoBehaviour
{
    private IEnumerator Start()
    {
        // 指定秒数待機...
        yield return new WaitForSeconds(0.1f);

        // WebGLで起動されたら...
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // 講師か生徒か取得
            yield return CoGetPlayerType();
        }
        // タイトルシーンへ遷移
        SceneFadeManager.I.Load(SceneName.Title, 0f);

    }

    /// <summary>
    /// プレイヤータイプ（講師か生徒）取得
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoGetPlayerType()
    {
        // ゲームが実行されている環境がWebGLの場合なら...
        // PlayerType取得
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (GetClieParameters.m_PlayerType != null)
            {
                GameInfo.MyPlayerType = (PlayerType)Enum.Parse(typeof(PlayerType), GetClieParameters.m_PlayerType);
            }
            else
            {
                // PlayerType取得失敗UI表示

                // PlayerType取得エラーメッセージ
                GameObject msgBox = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
                msgBox.GetComponent<MessageBox>().Initialize_Ok("Communication error", $"Failed to get Player Type\nReturn to the title.", () =>
                {
                    SceneFadeManager.I.Load(SceneName.GetPlayerType);
                });
                while (true) { yield return null; }
            }
        }
        yield break;
    }
}
