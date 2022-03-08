
/// <summary>
/// 使用するURLのタイプ
/// </summary>
public enum URLType
{
    /// <summary>
    /// Info環境
    /// </summary>
    Info,

    /// <summary>
    /// 開発環境
    /// </summary>
    Develop,

    /// <summary>
    /// クアドラのレンタルサーバー環境
    /// </summary>
    Quadra,

    /// <summary>
    /// 本番環境
    /// </summary>
    StudyCompas,
}

public enum PlayerType
{
    /// <summary>
    /// 初期値
    /// </summary>
    None,

    /// <summary>
    /// 講師
    /// </summary>
    Teacher,

    /// <summary>
    /// 生徒
    /// </summary>
    Student
}

/// <summary>
/// ゲーム情報
/// </summary>
public class GameInfo
{
    /// <summary>
    /// 値を1～6にしている場合、サイコロを振ったときその値が出る
    /// </summary>
    public static int ControllDiceNumber = 0;

    /// <summary>
    /// フレームレート
    /// </summary>
    public const int FrameRate = 60;

    /// <summary>
    /// 自身のユーザーID
    /// </summary>
    public static string MyUserID = "0000";

    /// <summary>
    /// 講師か生徒か
    /// </summary>
    public static PlayerType MyPlayerType = PlayerType.None;

    /// <summary>
    /// 自身のターン
    /// </summary>
    public static Turn MyTurn = Turn.None;

    /// <summary>
    /// ゲームデータ
    /// </summary>
    public static GameData Game = default;

    /// <summary>
    /// 使用するURLのタイプ
    /// </summary>
    public static URLType URLType = URLType.Info;

    /// <summary>
    /// 再起動されたか？
    /// </summary>
    public static bool IsRestart = false;

    /// <summary>
    /// 勝利回数
    /// </summary>
    public static int WinCount = 0;

    /// <summary>
    /// 敗北回数
    /// </summary>
    public static int LoseCount = 0;

    /// <summary>
    /// アプリケーション名
    /// </summary>
    public static string ApplicationName = "snakesandladders";

    /// <summary>
    /// 対戦相手のターン
    /// </summary>
    public static Turn OpponentTurn
    {
        get => GameInfo.MyTurn == Turn.User01 ? Turn.User02 : Turn.User01;
    }
}
