using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// ゲームデータを同期するクラス
/// </summary>
public class GameDataSync : UWRHelper
{
    /// <summary>
    /// コルーチン
    /// </summary>
    private IEnumerator m_Coroutine = null;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        // 通信チェックを開始。
        // ゲームが開始された終了する。
        StartCheckConnecting();
    }

    /// <summary>
    /// <summary>
    /// サーバーと接続されているかチェックする
    /// ゲーム同期を開始するタイミングでこのコルーチンを終了する
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoCheckConnecting()
    {
        // 無限ループ
        while (true)
        {
            var uwr = CreateGetUrl(KeyData.GameKey);
            yield return WaitForRequest(uwr);

            // ゲームデータが無ければエラーメッセージを表示
            if (!CheckKey(uwr))
            {
                var msgBox = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
                msgBox.GetComponent<MessageBox>().Initialize_Ok("Communication Error", $"Your opponent has exited the game. \nComing back to the matching screen.", () => SceneFadeManager.I.Load(SceneName.Room));
                while (true) { yield return null; }
            }
            yield return new WaitForSeconds(DefaultSyncSecond);
        }
    }

    /// <summary>
    /// 通信チェックを開始する。
    /// </summary>
    private void StartCheckConnecting()
    {
        EndCoroutine();
        m_Coroutine = CoCheckConnecting();
        StartCoroutine(m_Coroutine);
    }

    /// <summary>
    /// ゲーム同期を開始する
    /// </summary>
    public void StartGameSync()
    {
        EndCoroutine();
        m_Coroutine = CoGameDataSync();
        StartCoroutine(m_Coroutine);
    }

    /// <summary>
    /// リザルト画面チェックする。
    /// </summary>
    public void StartCheckResult()
    {
        EndCoroutine();
        m_Coroutine = CoCheckResult();
        StartCoroutine(m_Coroutine);
    }

    /// <summary>
    /// ゲーム同期を停止する
    /// </summary>
    public void StopGameSync()
    {
        EndCoroutine();
    }
    
    /// <summary>
    /// コルーチンを終了する
    /// </summary>
    public void EndCoroutine()
    {
        if(m_Coroutine != null)
        {
            StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }
    }

    /// <summary>
    /// ゲームデータを同期させる
    /// 指定秒数間隔で呼び出されるメソッド
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoGameDataSync()
    {
        // 無限ループ
        // ゲームデータを同期させる
        while(true)
        {
            var uwr = CreateGetUrl(KeyData.GameKey);
            yield return WaitForRequest(uwr);

            if(CheckKey(uwr))
            {
                GameData gameData = GameData.FromJsonConvert(JsonNode.GetValue(uwr.downloadHandler.text));

                // ゲームデータ同期できるか？
                if (CheckUpdateGameData(gameData))
                {
                    //// ゲームデータ画面同期
                    //GameInfo.Game = gameData;
                    yield return GameManager.I.CoScreenSync(gameData);
                }
            }
            else
            {
                // 対戦相手が見つからなければエラーメッセージを表示
                var msgBox = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
                msgBox.GetComponent<MessageBox>().Initialize_Ok("Communication Error", $"Your opponent has exited the game. \nComing back to the matching screen.", () => SceneFadeManager.I.Load(SceneName.Room));
                while (true) { yield return null; }
            }
            yield return new WaitForSeconds(DefaultSyncSecond);
        }
    }

    /// <summary>
    /// リザルト画面中のデータを調べる
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoCheckResult()
    {
        // 無限ループ
        while (true)
        {
            var uwr = CreateGetUrl(KeyData.GameKey);
            yield return WaitForRequest(uwr);

            // ゲームデータが無ければ、対戦相手がリトライしたという事
            // 自身もリトライするために、Roomシーンに遷移
            if (!CheckKey(uwr))
            {
                SceneFadeManager.I.Load(SceneName.Room);
                yield break;
            }

            yield return new WaitForSeconds(DefaultSyncSecond);
        }
    }

    /// <summary>
    /// ゲームデータを更新できるか調べる
    /// </summary>
    /// <param name="gameData"></param>
    /// <returns>TRUE: 更新できる FALSE: 更新できない</returns>
    public static bool CheckUpdateGameData(GameData gameData)
    {
        //// ターンが設定されていなければ、更新しない
        //if (gameData.Turn == Turn.None)
        //{
        //    //Debug.Log("更新無し: ターンが設定されていない");
        //    return false;
        //}

        //// 自分のターンであれば、更新しない
        //if (gameData.Turn == GameInfo.MyTurn)
        //{
        //    //Debug.Log("更新無し: 自分のターン中");
        //    return false;
        //}

        //// 遷移タイプがなにか設定されていれば、更新しない
        //if(gameData.TransitionType != TransitionType.None)
        //{
        //    Debug.Log("更新無し: 遷移中です");
        //    return false;
        //}

        return true;
    }

    /// <summary>
    /// ゲームデータを更新する
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoUpdateGameData(GameData gameData)
    {
        var uwr = CreateSetUrl(KeyData.GameKey, GameData.ToJsonConvert(gameData));
        yield return WaitForRequest(uwr);
        //yield return new WaitForSeconds(DefaultSyncSecond);
    }

    /// <summary>
    /// ゲームデータを削除
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoDeleteGameData()
    {
        var uwr = CreateDeletUrl(KeyData.GameKey);
        yield return WaitForRequest(uwr);
    }

    /// <summary>
    /// 初期化時にゲームデータを同期させる
    /// User2側で呼び出される
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoSetUpDataSync_User2()
    {
        // ゲームデータ内容を同期するまでループ
        while (true)
        {
            var uwr = CreateGetUrl(KeyData.GameKey);
            yield return WaitForRequest(uwr);

            if(CheckKey(uwr))
            {
                GameData gameData = GameData.FromJsonConvert(JsonNode.GetValue(uwr.downloadHandler.text));

                // ボードデータが選出されていたら
                // データを格納し処理を抜ける
                if(gameData.BoardNum != -1)
                {
                    GameInfo.Game = gameData;
                    break;
                }
                
            }

            yield return new WaitForSeconds(DefaultSyncSecond);
        }
    }

    /// <summary>
    /// 生徒が講師のOKボタン押下を待機中に呼び出される
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoGetMissionFlg_Student()
    {
        while(true)
        {
            var uwr = CreateGetUrl(KeyData.GameKey);
            yield return WaitForRequest(uwr);

            if (CheckKey(uwr))
            {
                GameData gameData = GameData.FromJsonConvert(JsonNode.GetValue(uwr.downloadHandler.text));

                // ミッション中フラグが折れていたら
                // データを格納し処理を抜ける
                if (!gameData.MissionFlg)
                {
                    GameInfo.Game = gameData;
                    break;
                }

            }

            yield return new WaitForSeconds(DefaultSyncSecond);
        }
    }

    /// <summary>
    /// リザルト画面　両プレイヤー到達するまで待機する処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoWaitResult()
    {
        // 先に結果画面に到達したプレイヤーは、
        // 相手プレイヤーが結果画面に到達するまで待機するようにしている。
        while(true)
        {
            var uwr = CreateGetUrl(KeyData.GameKey);
            yield return WaitForRequest(uwr);

            if (CheckKey(uwr))
            {
                GameData gameData = GameData.FromJsonConvert(JsonNode.GetValue(uwr.downloadHandler.text));

                if (gameData.Turn != GameInfo.MyTurn)
                {
                    GameInfo.Game.Turn = Turn.Result;
                    yield return CoUpdateGameData(GameInfo.Game);
                    yield break;
                }
            }

            yield return new WaitForSeconds(DefaultSyncSecond);
        }
    }

    /// <summary>
    /// 勝敗データ送信
    /// </summary>
    /// <param name="userId">ユーザーID</param>
    /// <param name="roomId">ルームID</param>
    /// <param name="winCount">勝利回数</param>
    /// <param name="loseCount">敗北回数</param>
    /// <param name="ApplicationName">アプリケーションの名前</param>
    /// <returns></returns>
    public IEnumerator CoUpdateResult(string userId, string roomId, int winCount, int loseCount, string applicationName)
    {
        var result = ResultSetUrl(int.Parse(userId), int.Parse(roomId), winCount, loseCount, applicationName);
        yield return WaitForRequest(result);

        //Debug.LogError("勝敗結果" + result.downloadHandler.text);
    }
}
