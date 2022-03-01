using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

/// <summary>
/// Room管理クラス
/// </summary>
public class RoomManager : UWRHelper
{
    /// <summary>
    /// User1
    /// </summary>
    private const string User1 = "User1";

    /// <summary>
    /// User2
    /// </summary>
    private const string User2 = "User2";

    [Header("ユーザーデータ取得UI")]
    [SerializeField] private GameObject m_GetUserDataUI = null;

    [Header("ユーザーデータ取得失敗UI")]
    [SerializeField] private GameObject m_FailedUserDataUI = null;

    [Header("接続待ちUI")]
    [SerializeField] private GameObject m_WaitingConnectUI = null;

    [Header("ターン選択待ちUI")]
    [SerializeField] private GameObject m_WaitingSelectTurnUI = null; 

    [Header("先行/後攻を設定するUI")]
    [SerializeField] private SelectTurnUI m_SelectTurnUI = null;

    [Header("ユーザーの名前を表示するテキスト")]
    [SerializeField] private Text m_UserNameText = null;

    /// <summary>
    /// コルーチン
    /// </summary>
    private IEnumerator m_Coroutine = null;

    /// <summary>
    /// Start
    /// </summary>
    protected void Start()
    {
        AudioManager.I.PlayBgm(AudioKey.MainBgm, 0.5f);
        StartMatching();
    }

    /// <summary>
    /// マッチング開始
    /// </summary>
    private void StartMatching()
    {
        m_GetUserDataUI.SetActive(true);
        StopMatching();
        m_Coroutine = CoInitialize();
        StartCoroutine(CoInitialize());
    }

    /// <summary>
    /// マッチング中止
    /// </summary>
    private void StopMatching()
    {
        if (m_Coroutine != null)
        {
            StopCoroutine(m_Coroutine);
            m_Coroutine = null;
        }
    }

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoInitialize()
    {
        // シーン遷移が完了するまで待機...
        yield return new WaitUntil(() => !SceneFadeManager.I.IsFading);

        // ゲームKey取得
        yield return CoGetGameKey();

        // ユーザーID取得
        yield return CoGetUserID();

        // ゲームを前回の状態から開始するか調べる
        // 前回の状態から開始しない場合はゲームデータを削除している。
        yield return CoCheckRestart();

        // 入室処理
        yield return CoEntryRoom();

        // ゲームシーンへ遷移
        yield return CoGoToGameScene();
    }

    /// <summary>
    /// GameKey取得
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoGetGameKey()
    {
        //// UserID取得エラーメッセージ
        //GameObject msgBox = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
        //msgBox.GetComponent<MessageBox>().Initialize_Ok("Communication error", $"Failed to get user ID\nReturn to the title.", () =>
        //{
        //    SceneFadeManager.I.Load(SceneName.Title);
        //    StopMatching();
        //});
        //while (true) { yield return null; }

        // ゲームが実行されている環境がWebGLの場合なら...
        // GameKey取得
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (GetClieParameters.m_GameKey != null)
            {
                KeyData.GameKey = GetClieParameters.m_GameKey;
            }
            else
            {
                // RoomID取得エラーメッセージ
                GameObject msgBox = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
                msgBox.GetComponent<MessageBox>().Initialize_Ok("Communication error", $"Failed to get progresses ID\nReturn to the title.", () =>
                {
                    SceneFadeManager.I.Load(SceneName.Title);
                    StopMatching();
                });
                while (true) { yield return null; }
            }
        }

        yield break;
    }

    /// <summary>
    /// ユーザーID取得
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoGetUserID()
    {
        //// ランダムにUserID取得
        //int value = UnityEngine.Random.Range(0, 10000);
        //GameInfo.MyUserID = value.ToString("0000");

        // ゲームが実行されている環境がWebGLの場合なら...
        // UserID取得
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (GetClieParameters.m_UserId != null)
            {
                // ユーザーデータ取得完了
                // 対戦相手接続待ちUI表示
                m_GetUserDataUI.SetActive(false);
                m_WaitingConnectUI.SetActive(true);

                GameInfo.MyUserID = GetClieParameters.m_UserId;
            }
            else
            {
                // ユーザーデータ取得失敗UI表示
                m_GetUserDataUI.SetActive(false);
                m_FailedUserDataUI.SetActive(true);

                // UserID取得エラーメッセージ
                GameObject msgBox = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
                msgBox.GetComponent<MessageBox>().Initialize_Ok("Communication error", $"Failed to get user ID\nReturn to the title.", () =>
                {
                    SceneFadeManager.I.Load(SceneName.Title);
                    StopMatching();
                });
                while (true) { yield return null; }
            }
        }
        else
        {
            // ユーザーデータ取得完了
            // 対戦相手接続待ちUI表示
            m_GetUserDataUI.SetActive(false);
            m_WaitingConnectUI.SetActive(true);
        }

        yield break;
    }

    /// <summary>
    /// ルーム入室
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoEntryRoom()
    {
        yield return CoCheckGame();
        Debug.Log("ゲームチェック完了");

        // ここでゲームデータが削除されたら
        // タイトルに戻るように設定している。
        StartCoroutine(CoCheckDeleteGame());

        yield return CoEntryUser();
        Debug.Log("ユーザ登録完了");
        yield return CoMatching();
        Debug.Log("マッチング完了");
        yield return CoSetGameInfoGame();
        Debug.Log("ゲームデータ取得完了");
        yield return CoSelectTurn();
        Debug.Log("先行/後攻選択完了");
    }


    /// <summary>
    /// ゲームチェック
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoCheckGame()
    {
        var uwr = CreateGetUrl(KeyData.GameKey);
        yield return WaitForRequest(uwr);

        // ゲームデータ作成済みならこの処理を抜ける
        if (CheckKey(uwr)) { yield break; }

        // ゲームデータ作成
        yield return CoCreateGameData();
    }

    /// <summary>
    /// ゲームデータが消されていない調べる
    /// 消されていたら処理を中断し、タイトルに戻る
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoCheckDeleteGame()
    {
        // 無限ループ
        while (true)
        {
            var uwr = CreateGetUrl(KeyData.GameKey);
            yield return WaitForRequest(uwr);

            // ゲームデータが無ければエラーメッセージを表示
            if (!CheckKey(uwr))
            {
                GameObject msgBox = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
                msgBox.GetComponent<MessageBox>().Initialize_Ok("Time out", $"There is no response from the opponent.\nReturn to the title.", () =>
                {
                    SceneFadeManager.I.Load(SceneName.Title);
                    StopMatching();
                });
                while (true) { yield return null; }
            }
            yield return new WaitForSeconds(DefaultSyncSecond);
        }
    }

    /// <summary>
    /// ユーザー登録
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoEntryUser()
    {
        UnityWebRequest uwr = CreateGetUrl(KeyData.GameKey);
        yield return WaitForRequest(uwr);
        GameData gameData = GameData.FromJsonConvert(JsonNode.GetValue(uwr.downloadHandler.text));

        // 各UserにIDが設定されていなければ...
        // IDを設定、ルームデータ更新
        if (string.IsNullOrEmpty(gameData.UserID_01))
        {
            GameInfo.MyTurn = Turn.User01;
            m_UserNameText.text = User1;
            gameData.UserID_01 = GameInfo.MyUserID;
            // ゲームデータを削除するタイムリミットを24時間後に設定(#72対応)
            gameData.timeLimit = DateTime.Now.AddHours(24).ToString();
            yield return CoUpdateGameData(gameData);
        }
        else if (string.IsNullOrEmpty(gameData.UserID_02) && GameInfo.MyUserID != gameData.UserID_01)
        {
            GameInfo.MyTurn = Turn.User02;
            m_UserNameText.text = User2;
            gameData.UserID_02 = GameInfo.MyUserID;
            yield return CoUpdateGameData(gameData);
        }
        // 対戦相手とユーザーIDが同じならタイトルに戻る
        else
        {
            // エラー処理
            var msgBox = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
            msgBox.GetComponent<MessageBox>().Initialize_Ok("User Error", $"I'm trying to register the same user ID. \nBack to the title.", () => SceneFadeManager.I.Load(SceneName.Title));
            while (true) { yield return null; }
        }
    }

    /// <summary>
    /// マッチング処理
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoMatching()
    {
        // 無限ループ
        // ゲームデータを同期させる
        while (true)
        {
            var uwr = CreateGetUrl(KeyData.GameKey);
            yield return WaitForRequest(uwr);

            if (CheckKey(uwr))
            {
                GameData gameData = GameData.FromJsonConvert(JsonNode.GetValue(uwr.downloadHandler.text));

                // 重複入室対応用
                // User1に登録したはずのユーザー情報が上書きされていたら...
                // 再度User2として登録する。
                if (GameInfo.MyTurn == Turn.User01 && gameData.UserID_01 != GameInfo.MyUserID)
                {
                    if (string.IsNullOrEmpty(gameData.UserID_02) && GameInfo.MyUserID != gameData.UserID_01)
                    {
                        GameInfo.MyTurn = Turn.User02;
                        m_UserNameText.text = "User2";
                        gameData.UserID_02 = GameInfo.MyUserID;
                        Debug.Log("ユーザー情報が上書きされていました。再度User2として登録します。");
                        yield return CoUpdateGameData(gameData);
                    }
                    // 対戦相手とユーザーIDが同じならタイトルに戻る
                    else
                    {
                        // UserIDが同じと警告するエラーメッセージ
                        var msgBox = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
                        msgBox.GetComponent<MessageBox>().Initialize_Ok("User Error", $"I'm trying to register the same user ID. \nBack to the title.", () => SceneFadeManager.I.Load(SceneName.Title));
                        while (true) { yield return null; }
                    }
                }

                // 両ユーザーが登録されていたら...
                // 処理を抜ける
                if (!string.IsNullOrEmpty(gameData.UserID_01) && !string.IsNullOrEmpty(gameData.UserID_02))
                {
                    yield break;
                }
            }

            yield return new WaitForSeconds(DefaultSyncSecond);
        }
    }

    /// <summary>
    /// ゲーム情報のGameDataを設定
    /// データベース上のゲームデータを取得し、GameInfo.Gameに格納している。
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoSetGameInfoGame()
    {
        // ゲームデータを取得し格納する。
        while (true)
        {
            var uwr = CreateGetUrl(KeyData.GameKey);
            yield return WaitForRequest(uwr);

            if (CheckKey(uwr))
            {
                // データベース上のゲームデータ格納
                GameInfo.Game = GameData.FromJsonConvert(JsonNode.GetValue(uwr.downloadHandler.text));
                break;
            }
        }
    }

    /// <summary>
    /// 先行/後攻選択
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoSelectTurn()
    {
        // 接続待ちUI非表示
        m_WaitingConnectUI.SetActive(false);

        // User1で先行/後攻を設定
        // User2は設定されたデータを同期する
        switch (GameInfo.MyTurn)
        {
            case Turn.User01:
                // 制限時間を超えていないか調べる
                // もし超えていたら、この処理を終了してタイトル画面に遷移する
                StartCheckTimeLimit(DefalutTimeLimitSecond);
                yield return m_SelectTurnUI.CoSelect();
                // 制限時間チェック終了
                EndCheckTimeLimit();
                yield return CoUpdateGameData(GameInfo.Game);
                break;
            case Turn.User02:
                // 相手のターン選択を待つUI表示
                m_WaitingSelectTurnUI.SetActive(true);
                yield return CoSettingTurnSync();
                break;
        }
    }

    /// <summary>
    /// 設定されたターンを同期する
    /// User1側で先行/後攻が設定されるまで待機する
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoSettingTurnSync()
    {
        // 制限時間を超えていないか調べる
        // もし超えていたら、この処理を終了してタイトル画面に遷移する
        StartCheckTimeLimit(DefalutTimeLimitSecond);

        // ゲームデータ内容を同期するまでループ
        while (true)
        {
            var uwr = CreateGetUrl(KeyData.GameKey);
            yield return WaitForRequest(uwr);

            if (CheckKey(uwr))
            {
                GameData gameData = GameData.FromJsonConvert(JsonNode.GetValue(uwr.downloadHandler.text));

                // User1側で先行/後攻を設定していれば...
                // ゲームを同期させる
                if (gameData.Turn != Turn.None)
                {
                    GameInfo.Game = gameData;
                    break;
                }
            }

            yield return new WaitForSeconds(DefaultSyncSecond);
        }

        // 制限時間チェック終了
        EndCheckTimeLimit();
    }

    /// <summary>
    /// ゲームシーンへ遷移
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoGoToGameScene()
    {
        // ユーザー登録されていたらゲームシーン遷移
        // ユーザー登録されていなけばこのシーンを再度読み込み
        if (GameInfo.MyUserID != string.Empty)
        {
            SceneFadeManager.I.Load(SceneName.Game);
        }
        yield break;
    }

    /// <summary>
    /// ゲームデータ作成
    /// </summary>
    private IEnumerator CoCreateGameData()
    {
        GameData data = new GameData();
        data.RoomID = KeyData.GameKey;
        yield return CoCreateGameData(data);
        //Debug.Log("ゲームデータ作成");
    }

    /// <summary>
    /// ゲームデータ作成
    /// </summary>
    /// <param name="gameData"></param>
    /// <returns></returns>
    private IEnumerator CoCreateGameData(GameData gameData)
    {
        var uwr = CreateSetUrl(KeyData.GameKey, GameData.ToJsonConvert(gameData));
        yield return WaitForRequest(uwr);
    }

    /// <summary>
    /// ゲームデータ更新
    /// </summary>
    /// <param name="gameData"></param>
    /// <returns></returns>
    private IEnumerator CoUpdateGameData(GameData gameData)
    {
        var uwr = CreateSetUrl(KeyData.GameKey, GameData.ToJsonConvert(gameData));
        yield return WaitForRequest(uwr);

        if (!CheckKey(uwr))
        {
            // エラー処理
            var msgBox = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
            msgBox.GetComponent<MessageBox>().Initialize_Ok("Communication Error", $"No game information found. \nBack to the title.", () => SceneFadeManager.I.Load(SceneName.Title));
            while (true) { yield return null; }
        }
    }

    /// <summary>
    /// ゲームデータを削除
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoDeleteGameData()
    {
        // ゲームデータ削除
        var uwr = CreateDeletUrl(KeyData.GameKey);
        yield return WaitForRequest(uwr);
        //Debug.Log("ゲームデータ削除");
    }

    /// <summary>
    /// ゲームを再起動（ゲームを前回の状態からプレイ）するか調べる
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoCheckRestart()
    {
        var uwr = CreateGetUrl(KeyData.GameKey);
        yield return WaitForRequest(uwr);

        if (CheckKey(uwr))
        {
            // 神経衰弱ではなく別のゲームデータがサーバー上に格納されていたらゲームデータを削除する。
            if (CheckDifferenceGameData(uwr))
            {
                Debug.Log("Snakes and laddersではなく別のゲームデータがサーバー上にあったため、ゲームデータを削除します。");
                yield return CoDeleteGameData();
            }
            // 正常なゲームデータが格納されていたら...
            else
            {
                GameData gameData = GameData.FromJsonConvert(JsonNode.GetValue(uwr.downloadHandler.text));

                if(!string.IsNullOrEmpty(gameData.timeLimit))
                {
                    DateTime limit = DateTime.Parse(gameData.timeLimit);
                    // タイムリミットを超えていた場合はデータを削除(#72対応)
                    if(DateTime.Now > limit)
                    {
                        yield return CoDeleteGameData();
                        Debug.Log("24時間が立ちましたゲームデータを削除");
                        yield break;
                    }
                }

                // 両ユーザIDが設定されている
                if (!string.IsNullOrEmpty(gameData.UserID_01) && !string.IsNullOrEmpty(gameData.UserID_02))
                {
                    // 使用されているゲームデータのRoomIDが前回使用したRoomIdと一致して
                    // 先行/後攻が設定されていれば...
                    if (KeyData.GameKey == gameData.RoomID && gameData.Turn != Turn.None)
                    {
                        // 前回使用したUserIdがUserID_01と一致したなら前回の状態から
                        // User1としてゲームに復帰する
                        if (GameInfo.MyUserID == gameData.UserID_01)
                        {
                            //Debug.Log("User1としてゲームに復帰します。");
                            GameInfo.MyTurn = Turn.User01;
                            m_UserNameText.text = User1;
                            GameInfo.IsRestart = true;
                            yield return CoSetGameInfoGame();
                            yield return CoGoToGameScene();
                            while (true) { yield return null; }
                        }
                        // 前回使用したUserIdがUserID_02と一致したなら前回の状態から
                        // User2としてゲームに復帰する
                        else if (GameInfo.MyUserID == gameData.UserID_02)
                        {
                            //Debug.Log("User2としてゲームに復帰します。");
                            GameInfo.MyTurn = Turn.User02;
                            m_UserNameText.text = User2;
                            GameInfo.IsRestart = true;
                            yield return CoSetGameInfoGame();
                            yield return CoGoToGameScene();
                            while (true) { yield return null; }
                        }
                    }

                    // それ以外ならゲームデータを削除
                    yield return CoDeleteGameData();
                    yield return new WaitForSeconds(DefaultSyncSecond);
                }
            }
        }
    }
}
