using UnityEngine;

/// <summary>
/// ターン
/// </summary>
public enum Turn
{
    None = -1,
    User01,
    User02,
    Result,
};

/// <summary>
/// ゲームデータ
/// </summary>
[System.Serializable]
public class GameData
{
    /// <summary>
    /// 高さ
    /// </summary>
    public const int Height = 10;

    /// <summary>
    /// 幅
    /// </summary>
    public const int Width = 5;

    /// <summary>
    /// GameName
    /// </summary>
    [SerializeField] private string m_GameName = GameInfo.ApplicationName;
    public string GameName
    {
        get => m_GameName;
        set => m_GameName = value;
    }

    /// <summary>
    /// RoomID
    /// </summary>
    [SerializeField] private string m_RoomID = string.Empty;
    public string RoomID
    {
        get => m_RoomID;
        set => m_RoomID = value;
    }

    /// <summary>
    /// User1のPlayerType
    /// 講師なら1、生徒なら2。初期値が0で、そのままだとエラー
    /// </summary>
    [SerializeField] private PlayerType m_User1PlayerType = PlayerType.None;
    public PlayerType User1PlayerType
    {
        get => m_User1PlayerType;
        set => m_User1PlayerType = value;
    }

    /// <summary>
    /// User2のPlayerType
    /// 講師なら1、生徒なら2。初期値が0で、そのままだとエラー
    /// </summary>
    [SerializeField] private PlayerType m_User2PlayerType = PlayerType.None;
    public PlayerType User2PlayerType {
        get => m_User2PlayerType;
        set => m_User2PlayerType = value;
    }

    /// <summary>
    /// User01
    /// </summary>
    [SerializeField] private string m_UserID_01 = string.Empty;
    public string UserID_01
    {
        get => m_UserID_01;
        set => m_UserID_01 = value;
    }

    /// <summary>
    /// User02
    /// </summary>
    [SerializeField] private string m_UserID_02 = string.Empty;
    public string UserID_02
    {
        get => m_UserID_02;
        set => m_UserID_02 = value;
    }

    /// <summary>
    /// ボード番号
    /// </summary>
    [SerializeField] private int m_BoardNum = -1;
    public int BoardNum 
    {
        get => m_BoardNum;
        set => m_BoardNum = value;
    }

    /// <summary>
    /// ミッション中か
    /// </summary>
    [SerializeField] private bool m_MissionFlg = false;
    public bool MissionFlg 
    {
        get => m_MissionFlg;
        set => m_MissionFlg = value;
    }

    /// <summary>
    /// 進行中のミッション番号
    /// </summary>
    [SerializeField] private int m_MissionNumber = -1;
    public int MissionNumber 
    {
        get => m_MissionNumber;
        set => m_MissionNumber = value;
    }

    /// <summary>
    /// サイコロの出目
    /// -1はサイコロ振れるタイミングでない、0はサイコロ振り待ち、1～6は出た目
    /// </summary>
    [SerializeField] private int m_DiceNumber = 0;
    public int DiceNumber
    {
        get => m_DiceNumber;
        set => m_DiceNumber = value;
    }

    /// <summary>
    /// User1のいるマス
    /// </summary>
    [SerializeField] private int m_User1Square = 0;
    public int User1Square
    {
        get => m_User1Square;
        set => m_User1Square = value;
    }

    /// <summary>
    /// User2のいるマス
    /// </summary>
    [SerializeField] private int m_User2Square = 0;
    public int User2Square
    {
        get => m_User2Square;
        set => m_User2Square = value;
    }

    /// <summary>
    /// User1がサイコロを振った回数
    /// </summary>
    [SerializeField] private int m_User1MoveCount = 0;
    public int User1MoveCount 
    {
        get => m_User1MoveCount;
        set => m_User1MoveCount = value;
    }

    /// <summary>
    /// User2がサイコロを振った回数
    /// </summary>
    [SerializeField] private int m_User2MoveCount = 0;
    public int User2MoveCount {
        get => m_User2MoveCount;
        set => m_User2MoveCount = value;
    }

    /// <summary>
    /// ターン
    /// </summary>
    [SerializeField] private Turn m_Turn = Turn.None;
    public Turn Turn
    {
        get => m_Turn;
        set => m_Turn = value;
    }

    /// <summary>
    /// ゲームデータが削除されるまでの制限時間
    /// </summary>
    [SerializeField] private string m_TimeLimit = string.Empty;
    public string timeLimit
    {
        get => m_TimeLimit;
        set => m_TimeLimit = value;
    }

    /// <summary>
    /// コンストラクタ
    /// </summary>
    public GameData()
    {
        Initialize();
    }

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Initialize()
    {
        m_Turn = Turn.None;
    }

    /// <summary>
    /// Jsonデータからゲームデータに変換
    /// </summary>
    /// <param name="json">Jsonデータ</param>
    /// <returns>ゲームデータ</returns>
    public static GameData FromJsonConvert(JsonNode json)
    {
        SendGameData sendGameData = SendGameData.Convert(json);
        GameData gameData = new GameData();
        
        gameData.m_GameName         = sendGameData.GameName;
        gameData.m_RoomID           = sendGameData.RoomID;
        gameData.m_User1PlayerType  = (PlayerType)sendGameData.User1PlayerType;
        gameData.m_User2PlayerType  = (PlayerType)sendGameData.User2PlayerType;
        gameData.m_UserID_01        = sendGameData.UserID_01;
        gameData.m_UserID_02        = sendGameData.UserID_02;
        gameData.m_BoardNum         = sendGameData.BoardNum;
        gameData.m_MissionFlg       = sendGameData.MissionFlg;
        gameData.m_MissionNumber    = sendGameData.MissionNumber;
        gameData.m_DiceNumber       = sendGameData.DiceNumber;
        gameData.m_User1Square      = sendGameData.User1Square;
        gameData.m_User2Square      = sendGameData.User2Square;
        gameData.m_User1MoveCount   = sendGameData.User1MoveCount;
        gameData.m_User2MoveCount   = sendGameData.User2MoveCount;
        gameData.m_Turn             = (Turn)sendGameData.Turn;
        gameData.m_TimeLimit        = sendGameData.TimeLimit;

        return gameData;
    }

    /// <summary>
    /// GameデータからJsonデータに変換
    /// </summary>
    /// <param name="gameData"><ゲームデータ/param>
    /// <returns>Jsonデータ</returns>
    public static string ToJsonConvert(GameData gameData)
    {
        SendGameData sendGameData = SendGameData.CreateData(gameData);
        string json = JsonUtility.ToJson(sendGameData);
        json = "[" + json + "]";
        return json;
    }

    #region Debug

    /// <summary>
    /// ゲームデータの情報をstring型にして返す
    /// </summary>
    /// <returns></returns>
    public string GetStr()
    {
        string str =
            $"RoomID: {m_RoomID}\n" +
            $"User1PlayerType: {m_User1PlayerType}\n" +
            $"User2PlayerType: {m_User2PlayerType}\n" +
            $"User1: {m_UserID_01}\n" +
            $"User2: {m_UserID_02}\n" +
            $"BoardNum: {m_BoardNum}\n" +
            $"MissionFlg: {m_MissionFlg}\n" +
            $"MissionNum: {m_MissionNumber}\n" +
            $"DiceNumber: {m_DiceNumber}\n" +
            $"User1Square: {m_User1Square}\n" +
            $"User2Square: {m_User2Square}\n" +
            $"User1MoveCount: {m_User1MoveCount}\n" +
            $"User2MoveCount: {m_User2MoveCount}\n" +
            $"Turn: {m_Turn}\n" +
            $"TimeLimit: {m_TimeLimit}\n";

        return str;
    }
   
    #endregion
}


/// <summary>
/// 送信するゲームデータ
/// </summary>
[System.Serializable]
public class SendGameData
{
    /// <summary>
    /// GameName
    /// </summary>
    [SerializeField] private string m_GameName = GameInfo.ApplicationName;
    public string GameName => m_GameName;

    /// <summary>
    /// RoomID
    /// </summary>
    [SerializeField] private string m_RoomID = string.Empty;
    public string RoomID => m_RoomID;

    /// <summary>
    /// User1のPlayerType
    /// 講師なら1、生徒なら2。初期値が0で、そのままだとエラー
    /// </summary>
    [SerializeField] private int m_User1PlayerType = 0;
    public int User1PlayerType => m_User1PlayerType;

    /// <summary>
    /// User2のPlayerType
    /// 講師なら1、生徒なら2。初期値が0で、そのままだとエラー
    /// </summary>
    [SerializeField] private int m_User2PlayerType = 0;
    public int User2PlayerType => m_User2PlayerType;

    /// <summary>
    /// User01
    /// </summary>
    [SerializeField] private string m_UserID_01 = string.Empty;
    public string UserID_01 => m_UserID_01;

    /// <summary>
    /// User02
    /// </summary>
    [SerializeField] private string m_UserID_02 = string.Empty;
    public string UserID_02 => m_UserID_02;

    /// <summary>
    /// ボード番号
    /// </summary>
    [SerializeField] private int m_BoardNum = 0;
    public int BoardNum {
        get => m_BoardNum;
        set => m_BoardNum = value;
    }

    /// <summary>
    /// ミッション中か
    /// </summary>
    [SerializeField] private bool m_MissionFlg = false;
    public bool MissionFlg {
        get => m_MissionFlg;
        set => m_MissionFlg = value;
    }

    /// <summary>
    /// 進行中のミッション番号
    /// </summary>
    [SerializeField] private int m_MissionNumber = -1;
    public int MissionNumber {
        get => m_MissionNumber;
        set => m_MissionNumber = value;
    }

    /// <summary>
    /// サイコロの出目
    /// </summary>
    [SerializeField] private int m_DiceNumber = 0;
    public int DiceNumber {
        get => m_DiceNumber;
        set => m_DiceNumber = value;
    }

    /// <summary>
    /// User1のいるマス
    /// </summary>
    [SerializeField] private int m_User1Square = 0;
    public int User1Square {
        get => m_User1Square;
        set => m_User1Square = value;
    }

    /// <summary>
    /// User2のいるマス
    /// </summary>
    [SerializeField] private int m_User2Square = 0;
    public int User2Square {
        get => m_User2Square;
        set => m_User2Square = value;
    }

    /// <summary>
    /// User1がサイコロを振った回数
    /// </summary>
    [SerializeField] private int m_User1MoveCount = 0;
    public int User1MoveCount {
        get => m_User1MoveCount;
        set => m_User1MoveCount = value;
    }

    /// <summary>
    /// User2がサイコロを振った回数
    /// </summary>
    [SerializeField] private int m_User2MoveCount = 0;
    public int User2MoveCount {
        get => m_User2MoveCount;
        set => m_User2MoveCount = value;
    }

    /// <summary>
    /// ターン
    /// </summary>
    [SerializeField] private int m_Turn = -1;
    public int Turn => m_Turn;

    /// <summary>
    /// ゲームデータが削除されるまでの制限時間
    /// </summary>
    [SerializeField] private string m_TimeLimit = string.Empty;
    public string TimeLimit
    {
        get => m_TimeLimit;
    }

    /// <summary>
    /// ゲームデータのコンバート
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static SendGameData Convert(JsonNode json)
    {
        SendGameData sendGameData = new SendGameData();

        sendGameData.m_GameName         = json[0]["m_GameName"].Get<string>();
        sendGameData.m_RoomID           = json[0]["m_RoomID"].Get<string>();
        sendGameData.m_User1PlayerType  = (int)json[0]["m_User1PlayerType"].Get<long>();
        sendGameData.m_User2PlayerType  = (int)json[0]["m_User2PlayerType"].Get<long>();
        sendGameData.m_UserID_01        = json[0]["m_UserID_01"].Get<string>();
        sendGameData.m_UserID_02        = json[0]["m_UserID_02"].Get<string>();
        sendGameData.m_BoardNum         = (int)json[0]["m_BoardNum"].Get<long>();
        sendGameData.m_MissionFlg       = json[0]["m_MissionFlg"].Get<bool>();
        sendGameData.m_MissionNumber    = (int)json[0]["m_MissionNumber"].Get<long>();
        sendGameData.m_DiceNumber       = (int)json[0]["m_DiceNumber"].Get<long>();
        sendGameData.m_User1Square      = (int)json[0]["m_User1Square"].Get<long>();
        sendGameData.m_User2Square      = (int)json[0]["m_User2Square"].Get<long>();
        sendGameData.m_User1MoveCount   = (int)json[0]["m_User1MoveCount"].Get<long>();
        sendGameData.m_User2MoveCount   = (int)json[0]["m_User2MoveCount"].Get<long>();
        sendGameData.m_Turn             = (int)json[0]["m_Turn"].Get<long>();
        sendGameData.m_TimeLimit        = json[0]["m_TimeLimit"].Get<string>();

        return sendGameData;

    }

    /// <summary>
    /// 送信するゲームデータを作成する
    /// </summary>
    /// <param name="gameData"></param>
    /// <returns></returns>
    public static SendGameData CreateData(GameData gameData)
    {
        SendGameData sendGameData = new SendGameData();

        sendGameData.m_GameName         = gameData.GameName;
        sendGameData.m_RoomID           = gameData.RoomID;
        sendGameData.m_User1PlayerType  = (int)gameData.User1PlayerType;
        sendGameData.m_User2PlayerType  = (int)gameData.User2PlayerType;
        sendGameData.m_UserID_01        = gameData.UserID_01;
        sendGameData.m_UserID_02        = gameData.UserID_02;
        sendGameData.m_BoardNum         = gameData.BoardNum;
        sendGameData.m_MissionFlg       = gameData.MissionFlg;
        sendGameData.m_MissionNumber    = gameData.MissionNumber;
        sendGameData.m_DiceNumber       = gameData.DiceNumber;
        sendGameData.m_User1Square      = gameData.User1Square;
        sendGameData.m_User2Square      = gameData.User2Square;
        sendGameData.m_User1MoveCount   = gameData.User1MoveCount;
        sendGameData.m_User2MoveCount   = gameData.User2MoveCount;
        sendGameData.m_Turn             = (int)gameData.Turn;
        sendGameData.m_TimeLimit        = gameData.timeLimit;

        return sendGameData;
    }
}