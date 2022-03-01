using UnityEngine;
using System.Collections;

// 【info環境】
// 作成/更新
// https://www.studycompass.info/cloudt/gamedata/update
// 取得
// https://www.studycompass.info/cloudt/gamedata/get
// 削除
// https://www.studycompass.info/cloudt/gamedata/delete

// 【開発環境】
// 作成/更新
// https://101.207.135.70:8090/cloudt/gamedata/update
// 取得
// https://101.207.135.70:8090/cloudt/gamedata/get
// 削除
// https://101.207.135.70:8090/cloudt/gamedata/delete


/// <summary>
/// WWW通信用URLを作るクラス
/// </summary>
public class KeyValueHelper
{
    /// <summary>
    /// BaseURL クアドラ環境
    /// </summary>
    private const string BaseUrl_Quadra = "http://www16306uf.sakura.ne.jp/Quiz/QuizIndex.php?proc=";

    /// <summary>
    /// BaseURL 開発環境用
    /// </summary>
    private const string BaseUrl_Develop = "https://101.207.135.70:8090/cloudt/gamedata/";

    /// <summary>
    /// BaseURL Info環境用
    /// </summary>
    private const string BaseUrl_Info = "https://www.studycompass.info/cloudt/gamedata/";

    /// <summary>
    /// BaseURL 本番環境用
    /// </summary>
    private const string BaseUrl_studyCompass = "https://www.studycompass.net/cloudt/gamedata/";

    /// <summary>
    /// 結果送信用URL Info環境用
    /// </summary>
    static string infoResultUrl = "https://www.studycompass.info/cloudt/game/result/save";

    /// <summary>
    /// 結果送信用URL develop環境用
    /// </summary>
    static string developResultUrl = "https://101.207.135.70:8090/cloudt/game/result/save";

    /// <summary>
    /// 結果送信用URL 本番環境用
    /// </summary>
    static string productionResultUrl = "https://www.studycompass.net/cloudt/game/result/save";


    #region クアドラ環境

    /// <summary>
    /// Key完全一致のレコード取得用URL作成
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string CreateGetUrl_Quadra(string key)
    {
        string url;
        url = BaseUrl_Quadra + "getValueWithKey&apriId=devQuiz&key=" + key;
        return url;
    }

    /// <summary>
    /// Keyと部分一致のレコード設定/更新URL作成
    /// </summary>
    /// <param name="key"></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string CreateUpdateUrl_Quadra(string key, string value)
    {
        string url;
        url = BaseUrl_Quadra + "setValueWithKey&apriId=devQuiz&key=" + key + "&value=" + value;
        return url;
    }

    /// <summary>
    /// Keyと部分一致のレコード削除
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string CreateDeleteUrl_Quadra(string key)
    {
        string url;
        url = BaseUrl_Quadra + "deleteValueWithKey&apriId=devQuiz&key=" + key;
        return url;
    }

    #endregion

    #region 本番用環境

    /// <summary>
    /// Key完全一致のレコード取得用URL作成
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string CreateGetUrl_Info(string key)
    {
        string url;
        url = BaseUrl_Info + "get?key=" + key;
        return url;
    }

    /// <summary>
    /// レコードを全て削除
    /// </summary>
    /// <returns></returns>
    public static string CreateDeleteUrl_Info()
    {
        string url;
        url = BaseUrl_Info + "delete";
        return url;
    }

    /// <summary>
    /// Keyと完全一致のレコードを保存
    /// </summary>
    /// <returns></returns>
    public static string CreateUpdateUrl_Info()
    {
        string url;
        url = BaseUrl_Info + "update";
        return url;
    }

    #endregion

    #region 開発環境

    /// <summary>
    /// Key完全一致のレコード取得用URL作成
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string CreateGetUrl_Develop(string key)
    {
        string url;
        url = BaseUrl_Develop + "get?key=" + key;
        return url;
    }

    /// <summary>
    /// レコードを全て削除
    /// </summary>
    /// <returns></returns>
    public static string CreateDeleteUrl_Develop()
    {
        string url;
        url = BaseUrl_Develop + "delete";
        return url;
    }

    /// <summary>
    /// Keyと完全一致のレコードを保存
    /// </summary>
    /// <returns></returns>
    public static string CreateUpdateUrl_Develop()
    {
        string url;
        url = BaseUrl_Develop + "update";
        return url;
    }

    #endregion
    #region studyCompass

    /// <summary>
    /// Key完全一致のレコード取得用URL作成
    /// </summary>
    /// <param name="key"></param>
    /// <returns></returns>
    public static string CreateGetUrl_StudyCompass(string key)
    {
        string url;
        url = BaseUrl_studyCompass + "get?key=" + key;
        return url;
    }

    /// <summary>
    /// レコードを全て削除
    /// </summary>
    /// <returns></returns>
    public static string CreateDeleteUrl_StudyCompass()
    {
        string url;
        url = BaseUrl_studyCompass + "delete";
        return url;
    }

    /// <summary>
    /// Keyと完全一致のレコードを保存
    /// </summary>
    /// <returns></returns>
    public static string CreateUpdateUrl_StudyCompass()
    {
        string url;
        url = BaseUrl_studyCompass + "update";
        return url;
    }

    #endregion


    /// <summary>
    /// 勝敗数を送信
    /// </summary>
    /// <returns></returns>
    public static string ResultSetUrl()
    {
        // 結果送信用APIも環境によって異なるためURLTypeごとに設定(#74対応)
        switch(GameInfo.URLType)
        {
            case URLType.Info:
                return infoResultUrl;
            case URLType.Develop:
                return developResultUrl;
            case URLType.StudyCompas:
                return productionResultUrl;
        }
        return null;
    }
}