using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ゲーム状態
/// </summary>
enum GameState
{
    Init,
    Opening,
    Game,
    Result,
}

/// <summary>
/// ゲームを管理するクラス
/// </summary>
public class GameManager : SingletonMonoBehaviour<GameManager>
{
    /// <summary>
    /// ボードの情報
    /// </summary>
    [Header("ボードの情報")]
    [SerializeField] private Board[] m_Boards;

    /// <summary>
    /// ボードを表示する時間
    /// </summary>
    [Header("ボードを表示する時間")]
    [SerializeField] private float m_BoardFadeTime = 1f;

    /// <summary>
    /// トランプの回転する時間（トランプをひっくり返すのに掛かる時間）
    /// </summary>
    private const float TrumpRotateTime = 0.5f;

    /// <summary>
    /// トランプの移動に掛かる時間
    /// </summary>
    private const float TrumpMoveTime = 1f;

    /// <summary>
    /// 二つの選択したトランプを表示する時間
    /// </summary>
    private const float TrumpDisplayTime = 0.5f;

    /// <summary>
    /// 一つのトランプを配る時間
    /// </summary>
    private const float TrumpDealTime = 0.1f;

    [Header("ゲームデータ同期")]
    [SerializeField] private GameDataSync m_GameDataSync = null;

    [Header("ボード画像表示用UI")]
    [SerializeField] private BoardImage m_BoardImage = default;

    [Header("プレイヤーのコマ")]
    [SerializeField] private Piece[] m_Pieces = default;

    [Header("HUD")]
    [SerializeField] private HUD m_HUD = null;

    [Header("結果表示用UI")]
    [SerializeField] private ResultUI m_ResultUI = default;

    [Header("オープニング演出用UI")]
    [SerializeField] private OpeningUI m_OpeningUI = default;

    

    

    /// <summary>
    /// ゲーム状態
    /// </summary>
    private GameState m_GameState = GameState.Init;

    /// <summary>
    /// 同期が完了したか識別するFlg
    /// </summary>
    private bool m_SyncCompletedFlg = false;

    /// <summary>
    /// 選択したトランプの数
    /// </summary>
    private int m_SelectTrumpCount = 0;

    /// <summary>
    /// Start
    /// </summary>
    private void Start()
    {
        // メインBGM再生
        AudioManager.I.PlayBgm(AudioKey.MainBgm, 0.5f);

        // 初期化処理実行
        StartCoroutine(CoInitialize());
    }

    #region 初期化

    /// <summary>
    /// 初期化
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoInitialize()
    {
        // 状態を初期化状態に設定
        m_GameState = GameState.Init;

        // 前回の状態からゲームを開始する
        if (GameInfo.IsRestart)
        {
            yield return CoRestart();
        }
        // ゲームを開始する
        else
        {
            yield return CoStart();
        }
    }

    /// <summary>
    /// 自身のターンから始まるかどうか調べる。
    /// </summary>
    private void CheckMyTurn()
    {
        // 自分のターン中なら同期完了状態にする
        // 相手のターン中なら同期未完了状態にする
        if (GameInfo.MyTurn == GameInfo.Game.Turn)
        {
            m_SyncCompletedFlg = true;
        }
        else
        {
            m_SyncCompletedFlg = false;
        }
    }

    #region 通常にゲームを開始

    /// <summary>
    /// ゲームを開始する
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoStart()
    {
        // 自分のターン中か調べる
        CheckMyTurn();

        // User1が先に盤面情報を設定する。
        // User2はデータベース上の盤面情報を取得し、設定。
        if (GameInfo.MyTurn == Turn.User01)
        {
            yield return CoInitialize_User1();
            Debug.Log("User1初期化");
        }
        else if (GameInfo.MyTurn == Turn.User02)
        {
            yield return CoInitialize_User2();
            Debug.Log("User2初期化");
        }

        // ボードを表示
        yield return CoFadeDisplayBoard();

        // オープニング演出
        yield return CoOpening();

        // ターン表示テキストを設定
        m_HUD.SetTurn(GameInfo.Game.Turn);

        // ゲーム同期を開始する
        m_GameDataSync.StartGameSync();

        // 状態をGameプレイ状態に設定
        m_GameState = GameState.Game;
    }

    /// <summary>
    /// User1の初期化
    /// </summary>
    /// <returns></returns>

    private IEnumerator CoInitialize_User1()
    {
        // User1側で盤面を設定し、データを送信する。
        //SetCellArray();
        //CreateTrump();
        yield return m_GameDataSync.CoUpdateGameData(GameInfo.Game);
    }

    /// <summary>
    /// User2の初期化
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoInitialize_User2()
    {
        // User1側で設定した盤面データを同期
        yield return m_GameDataSync.CoSetUpDataSync_User2();
        //CreateTrump();
    }

    private IEnumerator CoFadeDisplayBoard()
    {
        // ボードをフェードイン表示
        yield return m_BoardImage.CoFadeIn(m_Boards[GameInfo.Game.BoardNum].m_BoardTexture, m_BoardFadeTime);
    }

    

    

    /// <summary>
    /// オープニング演出
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoOpening()
    {
        m_GameState = GameState.Opening;
        yield return m_OpeningUI.CoProduction(0.25f, 0.25f);
    }

    #endregion

    #region 再起動（前回の状態からゲームを開始）

    /// <summary>
    /// 前回のゲームから開始する。
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoRestart()
    {
        // 再起動FlgをFalseに設定
        GameInfo.IsRestart = false;

        // 自分のターン中か調べる
        CheckMyTurn();

        // 前回のゲームデータの盤面情報を生成する。
        //GetCreateCellArray();

        // ターン表示テキストを設定
        m_HUD.SetTurn(GameInfo.Game.Turn);

        // ゲームがすでに終了していたら...
        // ゲーム結果を表示
        if (CheckGameEnd())
        {
            yield return CoResult();
            yield break;
        }

        // ゲーム同期を開始する
        m_GameDataSync.StartGameSync();

        // 状態をGameプレイ状態に設定
        m_GameState = GameState.Game;

        Debug.Log("前回のデータからゲームを開始しました");
    }



    #endregion

    #endregion

    #region Snaeks and laddersのロジック

    /// <summary>
    /// ターンを変更する
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoChangeTurn()
    {
        // ターンを変更する
        switch (GameInfo.Game.Turn)
        {
            case Turn.User01:
                GameInfo.Game.Turn = Turn.User02;
                break;
            case Turn.User02:
                GameInfo.Game.Turn = Turn.User01;
                break;
        }
        yield break;
    }

    /// <summary>
    /// サイコロを振れるか調べる
    /// Dice.csでも使用している
    /// </summary>
    /// <param name="dice">サイコロデータ</param>
    /// <returns>TRUE 選択できる FALSE 選択できない</returns>
    public bool CheckDiceRoll(Dice dice)
    {
        if (m_GameState == GameState.Init ||
            m_GameState == GameState.Opening)
        {
            Debug.Log("サイコロクリック無効: ゲーム準備中");
            return false;
        }

        if (dice.DiceNumber == -1)
        {
            Debug.Log("サイコロクリック無効: サイコロを振れるタイミングでない");
            return false;
        }

        if (dice.DiceNumber > 0)
        {
            Debug.Log("サイコロクリック無効:　既にサイコロを振っている");
            return false;
        }

        if (GameInfo.MyTurn != GameInfo.Game.Turn)
        {
            Debug.Log("サイコロクリック無効: 自身のターンではない");
            return false;
        }

        if(!m_SyncCompletedFlg)
        {
            Debug.Log("サイコロクリック無効: 全ての同期が完了していません。\n");
            return false;
        }

        return true;
    }

    

    /// <summary>
    /// ゲームが終了したか調べる
    /// </summary>
    /// <returns>TRUE: ゲーム終了 FALSE: ゲーム続行</returns>
    private bool CheckGameEnd()
    {
        // ゴールマス
        int goalSquare = GameData.Height * GameData.Width;

        // どちらかがゴールマスに到達していたら、ゲーム終了
        if (GameInfo.Game.User1Square >= goalSquare ||
            GameInfo.Game.User2Square >= goalSquare)
        {
            return true;
        }

        return false;
    }

    

    #endregion

    #region データ送信/更新

    /// <summary>
    /// ゲームデータ送信
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoSendGameData()
    {
        

        // ゲームデータ送信
        yield return m_GameDataSync.CoUpdateGameData(GameInfo.Game);
    }

    /// <summary>
    /// 画面同期
    /// データベース上のデータと差異があれば画面を更新する
    /// </summary>
    /// <returns></returns>
    public IEnumerator CoScreenSync(GameData gameData)
    {
        // 同期が完了していないなら...
        if (!m_SyncCompletedFlg)
        {
            // ゲームデータを格納
            GameInfo.Game = gameData;

            // 

            
            yield break;
        }

    }
    #endregion

    #region ゲーム結果

    /// <summary>
    /// ゲーム結果
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoResult()
    {
        m_GameState = GameState.Result;

        // ゲームの同期を止める
        m_GameDataSync.StopGameSync();

        // プレイヤーの勝利
        if (m_HUD.Users[0].SquareNum > m_HUD.Users[1].SquareNum)
        {
            yield return m_ResultUI.CoWin();
        }
        // プレイヤーの敗北
        else
        {
            yield return m_ResultUI.CoLose();
        }
    }

    #endregion
}