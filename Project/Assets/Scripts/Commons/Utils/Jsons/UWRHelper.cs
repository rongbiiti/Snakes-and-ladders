using System;
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

/// <summary>
///　通信する際に継承して使う補助関数群
///　UnityWebRequest通信を行う
/// </summary>
public class UWRHelper : MonoBehaviour
{
    /// <summary>
    /// データ同期時に待機するデフォルトの時間
    /// </summary>
    protected static float DefaultSyncSecond = 0.5f;

    /// <summary>
    /// タイムリミットのデフォルト時間
    /// </summary>
    protected static float DefalutTimeLimitSecond = 180f;

    /// <summary>
    /// タイムリミット用コルーチン
    /// </summary>
    private IEnumerator m_Coroutine = null;

    /// <summary>
    /// データ取得用URL
    /// </summary>
    /// <param name="key">取得してくるデータのキーを入れる</param>
    /// <returns>(通信用)データ</returns>
    public UnityWebRequest CreateGetUrl(string key)
    {
        UnityWebRequest uwr = default;

        switch (GameInfo.URLType)
        {
            case URLType.Develop:
                uwr = UnityWebRequest.Get(KeyValueHelper.CreateGetUrl_Develop(key));
                break;
            case URLType.Info:
                uwr = UnityWebRequest.Get(KeyValueHelper.CreateGetUrl_Info(key));
                break;
            case URLType.Quadra:
                uwr = UnityWebRequest.Get(KeyValueHelper.CreateGetUrl_Quadra(key));
                break;
            case URLType.StudyCompas:
                uwr = UnityWebRequest.Get(KeyValueHelper.CreateGetUrl_StudyCompass(key));
                break;
        }
        return uwr;
    }

    /// <summary>
    /// データ追加＆更新用URL
    /// </summary>
    /// <param name="key">変更したいデータのキーを入れる</param>
    /// <returns>(通信用)データ</returns>
    public UnityWebRequest CreateSetUrl(string key, string text)
    {
        UnityWebRequest uwr = default;

        switch (GameInfo.URLType)
        {
            case URLType.Develop:
                WWWForm form = new WWWForm();
                form.AddField("key", key);
                form.AddField("value", text);
                uwr = UnityWebRequest.Post(KeyValueHelper.CreateUpdateUrl_Develop(), form);
                break;
            case URLType.Info:
                WWWForm formInfo = new WWWForm();
                formInfo.AddField("key", key);
                formInfo.AddField("value", text);
                uwr = UnityWebRequest.Post(KeyValueHelper.CreateUpdateUrl_Info(), formInfo);
                break;
            case URLType.Quadra:
                uwr = UnityWebRequest.Get(KeyValueHelper.CreateUpdateUrl_Quadra(key, text));
                break;
            // 本番環境
            case URLType.StudyCompas:
                WWWForm formStudy = new WWWForm();
                formStudy.AddField("key", key);
                formStudy.AddField("value", text);
                uwr = UnityWebRequest.Post(KeyValueHelper.CreateUpdateUrl_StudyCompass(), formStudy);
                break;
        }

        return uwr;
    }

    /// <summary>
    /// データの削除用URL
    /// </summary>
    /// <param name="key">>変更したいデータのキー<</param>
    /// <returns></returns>
    public UnityWebRequest CreateDeletUrl(string key)
    {
        UnityWebRequest uwr = default;

        switch (GameInfo.URLType)
        {
            case URLType.Develop:
                WWWForm form = new WWWForm();
                form.AddField("key", key);
                uwr = UnityWebRequest.Post(KeyValueHelper.CreateDeleteUrl_Develop(), form);
                break;
            case URLType.Info:
                WWWForm formInfo = new WWWForm();
                formInfo.AddField("key", key);
                uwr = UnityWebRequest.Post(KeyValueHelper.CreateDeleteUrl_Info(), formInfo);
                break;
            case URLType.Quadra:
                uwr = UnityWebRequest.Get(KeyValueHelper.CreateDeleteUrl_Quadra(key));
                break;
            case URLType.StudyCompas:
                WWWForm formStudy = new WWWForm();
                formStudy.AddField("key", key);
                uwr = UnityWebRequest.Post(KeyValueHelper.CreateDeleteUrl_StudyCompass(), formStudy);
                break;
        }

        return uwr;
    }


    /// <summary>
    /// 勝敗数追加＆更新用URL
    /// </summary>
    /// <param name="usrId">ユーザーID</param>
    /// <param name="roomId">ルームID</param>
    /// <param name="winCount">勝利数</param>
    /// <param name="loseCount">敗北数</param>
    /// <param name="applicationName">アプリケーションの名前</param>
    /// <returns></returns>
    public UnityWebRequest ResultSetUrl(int usrId, int roomId, int winCount, int loseCount, string applicationName)
    {
        WWWForm form = new WWWForm();
        form.AddField("uid", usrId);
        form.AddField("roomId", roomId);
        form.AddField("winCount", winCount);
        form.AddField("loseCount", loseCount);
        form.AddField("appName", applicationName);
        UnityWebRequest request = UnityWebRequest.Post(KeyValueHelper.ResultSetUrl(), form);
        return request;
    }

    /// <summary>
    /// 通信用コルーチン
    /// </summary>
    /// <param name="request">CreateGetUrlなどで作ったUnityWebRequestデータを入れる</param>
    /// <returns></returns>
    protected IEnumerator WaitForRequest(UnityWebRequest request)
    {
        // タイムアウト設定
        //request.timeout = 10;

        // 通信待ち...
        yield return request.SendWebRequest();

        // HTTPステータスコードがエラーを示していたら...
        if (request.isHttpError)
        {
            GameObject msgBox = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
            msgBox.GetComponent<MessageBox>().Initialize_Ok("Communication Error", $"Response Code : {request.responseCode}\nReturn to the matching screen.", () => SceneFadeManager.I.Load(SceneName.Room));
            while (true) { yield return null; }
        }
        // UnityWebRequestのシステムエラーが発生したら...
        // 具体的には、DNSを解決できない、リダイレクトエラー、タイムアウトなどがこれに当たります。
        else if (request.isNetworkError)
        {
            // タイムアウトしていたら...
            if (request.error == "Request timeout")
            {
                GameObject msgBox = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
                msgBox.GetComponent<MessageBox>().Initialize_Ok("Communication Error", $"No response.\nReturn to the matching screen.", () => SceneFadeManager.I.Load(SceneName.Room));
                while (true) { yield return null; }
            }
            else
            {
                GameObject msgBox = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
                msgBox.GetComponent<MessageBox>().Initialize_Ok("Communication Error", $"Unable to connect to the network.\nReturn to the matching screen.", () => SceneFadeManager.I.Load(SceneName.Room));
                while (true) { yield return null; }
            }
        }
    }

    /// <summary>
    /// レスポンスデータが正常なのか確認
    /// </summary>
    /// <param name="www">レスポンスデータ</param>
    /// <returns>true : 正常, false : エラー</returns>
    protected bool CheckKey(UnityWebRequest request)
    {
        if (request.downloadHandler == null)
        {
            return false;
        }

        //byte[] results = request.downloadHandler.data;
        //Debug.Log(convs(results));
        switch (GameInfo.URLType)
        {
            case URLType.Develop:
            case URLType.Info:
            case URLType.StudyCompas:
                return request.downloadHandler.text.IndexOf("success") != -1;
            case URLType.Quadra:
                return request.downloadHandler.text.IndexOf("key") != -1;
        }

        Debug.LogError("CheckKeyメソッドでエラーが発生しました");
        return default;
    }

    /// <summary>
    /// サーバー上のゲームデータと現在のゲームデータに違いがあるか調べる。
    /// TRUE: ある　FALSE: ない
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    protected bool CheckDifferenceGameData(UnityWebRequest request)
    {
        // サーバー上にSnaeks and laddersのデータがあればこの処理を通る
        if (request.downloadHandler.text.Contains(GameInfo.ApplicationName))
        {
            //Debug.Log("データを復帰させます");
            return false;
        }

        //Debug.Log("サーバー上のゲームデータと現在のゲームデータに違いがありました。\nゲームデータを削除します");
        return true;
    }

    /// <summary>
    /// データログ出力
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public string convs(byte[] data)
    {
        return new String(Array.ConvertAll(data, x => (char)x));
    }

    /// <summary>
    /// 制限時間チェック開始
    /// </summary>
    /// <param name="second">秒数</param>
    protected void StartCheckTimeLimit(float second)
    {
        EndCoroutine();
        m_Coroutine = CoCheckTimeLimit(second);
        StartCoroutine(m_Coroutine);
    }

    /// <summary>
    /// 制限時間チェック終了
    /// </summary>
    protected void EndCheckTimeLimit()
    {
        EndCoroutine();
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
    /// 制限時間を過ぎていないか調べる
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoCheckTimeLimit(float second)
    {
        yield return new WaitForSeconds(second);
        GameObject msgBox = (GameObject)Instantiate((GameObject)Resources.Load("Prefabs/MessageBox"));
        msgBox.GetComponent<MessageBox>().Initialize_Ok("Time out", $"There is no response from the opponent.\nReturn to the title.", () =>
        {
            SceneFadeManager.I.Load(SceneName.Title);
        });
        while (true) { yield return null; }
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

        Debug.Log("ゲームデータ削除");
    }
}
