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
    /// サイコロ
    /// </summary>
    [Header("サイコロ")]
    [SerializeField] private Dice m_Dice;

    

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
    /// ローカルで使うサイコロの出目
    /// </summary>
    private int m_DiceNumber = 0;

    /// <summary>
    /// User1がいるマス
    /// </summary>
    private int m_User1Square;

    /// <summary>
    /// User2がいるマス
    /// </summary>
    private int m_User2Square;

    /// <summary>
    /// ゲーム状態
    /// </summary>
    private GameState m_GameState = GameState.Init;

    /// <summary>
    /// ローカルで使うターン情報
    /// </summary>
    private Turn m_NowTurn = Turn.None;

    /// <summary>
    /// 同期が完了したか識別するFlg
    /// </summary>
    private bool m_SyncCompletedFlg = false;

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

        m_NowTurn = GameInfo.Game.Turn;
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

        // サイコロを表示
        SetActiveDice();

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
        SetBoardData();
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
    }

    /// <summary>
    /// ボードを抽選
    /// </summary>
    private void SetBoardData()
    {
        GameInfo.Game.BoardNum = Random.Range(0, m_Boards.Length);
    }

    /// <summary>
    /// ボードをフェードイン表示
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoFadeDisplayBoard()
    {
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
        m_BoardImage.Image.sprite = m_Boards[GameInfo.Game.BoardNum].m_BoardTexture;
        GetPlayerSquares();

        // サイコロを振れる状態なら…
        // サイコロを表示
        if(GameInfo.Game.DiceNumber == 0)
        {
            SetActiveDice();
        }

        // ミッション中なら…
        // ミッションを表示
        if(GameInfo.Game.MissionFlg)
        {

        }

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

    /// <summary>
    /// 前回のゲームデータからプレイヤーのコマを移動させる
    /// </summary>
    private void GetPlayerSquares()
    {
        m_User1Square = GameInfo.Game.User1Square;
        m_User2Square = GameInfo.Game.User2Square;
        
        // 自分がUser1
        if (GameInfo.MyTurn == Turn.User01)
        {
            m_Pieces[0].Acquired(m_User1Square);
            m_Pieces[1].Acquired(m_User2Square);
        }
        // 自分がUser2
        else if (GameInfo.MyTurn == Turn.User02)
        {
            m_Pieces[0].Acquired(m_User2Square);
            m_Pieces[1].Acquired(m_User1Square);
        }
    }

    #endregion

    #endregion

    #region Snaeks and laddersのロジック

    /// <summary>
    /// サイコロを振れるようにする
    /// </summary>
    private void SetActiveDice()
    {
        m_Dice.gameObject.SetActive(true);

        // 0は、サイコロを振れる状態
        m_Dice.DiceNumber = 0;

        // 自分のターンだったらサイコロのEventTriggerをアクティブにする
        if (GameInfo.MyTurn == GameInfo.Game.Turn)
        {
            m_Dice.EventTrigger.enabled = true;
        }
        else
        {
            m_Dice.EventTrigger.enabled = false;
        }

    }

    /// <summary>
    /// サイコロがクリックされた
    /// </summary>
    public void OnClick_Dice()
    {
        if (CheckDiceRoll())
        {
            StartCoroutine(CoDiceRoll());
        }
    }

    /// <summary>
    /// サイコロを振る
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoDiceRoll()
    {
        m_Dice.EventTrigger.enabled = false;

        m_DiceNumber = 0;

        // デバッグ用
        // 次に出すサイコロの目を設定した値にする
        if(GameInfo.ControllDiceNumber != 0)
        {
            m_DiceNumber = GameInfo.ControllDiceNumber;
        }
        // 普通に抽選
        else
        {
            m_DiceNumber = Random.Range(1, 7);
        }

        // 送信用データを作成
        SetSendGameData(m_DiceNumber);

        // ターン変更
        yield return CoChangeTurn();

        // ゲームデータ送信
        yield return CoSendGameData();

        // サイコロを振るアニメーション
        yield return m_Dice.CoDiceRollAnimation(m_DiceNumber);

        // コマを動かす
        yield return CoPieceMove();

        m_NowTurn = GameInfo.Game.Turn;
        m_Dice.DiceNumber = 0;
        m_HUD.SetTurn(GameInfo.Game.Turn);
        m_SyncCompletedFlg = false;

        SetActiveDice();

        // ゲーム終了チェック
        if (CheckGameEnd())
        {
            yield return CoResult();
        }
    }

    /// <summary>
    /// サイコロの結果を元にコマのマスを更新
    /// 送信するゲームデータを作成
    /// </summary>
    private void SetSendGameData(int diceNumber)
    {
        GameInfo.Game.DiceNumber = diceNumber;

        // 自分がUser1
        if (GameInfo.MyTurn == Turn.User01)
        {
            // 値が50を超えないようにする
            GameInfo.Game.User1Square = Mathf.Clamp(m_User1Square + diceNumber, 1, GameData.Width * GameData.Height);
        }
        // 自分がUser2
        else if (GameInfo.MyTurn == Turn.User02)
        {
            // 値が50を超えないようにする
            GameInfo.Game.User2Square = Mathf.Clamp(m_User2Square + diceNumber, 1, GameData.Width * GameData.Height);
        }
    }

    /// <summary>
    /// サイコロを振る（対戦相手の操作同期）
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoDiceRollSync()
    {
        m_Dice.EventTrigger.enabled = false;

        m_DiceNumber = GameInfo.Game.DiceNumber;

        // 相手が使用したサイコロの出目をリセット
        GameInfo.Game.DiceNumber = 0;

        // 相手にも送る
        yield return CoSendGameData();

        // サイコロを振るアニメーション
        yield return m_Dice.CoDiceRollAnimation(m_DiceNumber);

        // コマを動かす
        yield return CoPieceMove();

        m_NowTurn = GameInfo.Game.Turn;
        m_Dice.DiceNumber = 0;
        m_HUD.SetTurn(GameInfo.Game.Turn);
        m_SyncCompletedFlg = true;

        SetActiveDice();

        // ゲーム終了チェック
        if (CheckGameEnd())
        {
            yield return CoResult();
        }
    }

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
    /// </summary>
    /// <returns>TRUE 選択できる FALSE 選択できない</returns>
    public bool CheckDiceRoll()
    {
        if (m_GameState == GameState.Init ||
            m_GameState == GameState.Opening)
        {
            Debug.Log("サイコロクリック無効: ゲーム準備中");
            return false;
        }

        if (m_Dice.DiceNumber == -1)
        {
            Debug.Log("サイコロクリック無効: サイコロを振れるタイミングでない");
            return false;
        }

        if (m_Dice.DiceNumber > 0)
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
            Debug.Log("サイコロクリック無効: 全ての同期が完了していない");
            return false;
        }

        return true;
    }

    /// <summary>
    /// コマ移動
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoPieceMove()
    {

        // 自分のターン
        if (GameInfo.MyTurn == m_NowTurn)
        {
            // 自分がUser1
            if (GameInfo.MyTurn == Turn.User01)
            {
                // 一手目だったとき
                if(m_User1Square == 0)
                {
                    Debug.Log("一手目");
                    yield return m_Pieces[0].CoFirstPieceMove(m_Dice.DiceNumber);
                }
                // 二手目以降
                else
                {
                    yield return m_Pieces[0].CoPieceMove(m_User1Square, m_Dice.DiceNumber);
                    
                }
                m_User1Square = GameInfo.Game.User1Square;
            }
            // 自分がUser2
            else if (GameInfo.MyTurn == Turn.User02)
            {
                // 一手目だったとき
                if (m_User2Square == 0)
                {
                    Debug.Log("一手目");
                    yield return m_Pieces[0].CoFirstPieceMove(m_Dice.DiceNumber);
                }
                // 二手目以降
                else
                {
                    yield return m_Pieces[0].CoPieceMove(m_User2Square, m_Dice.DiceNumber);
                }
                m_User2Square = GameInfo.Game.User2Square;
            }
        }
        // 相手のターン
        else if (GameInfo.OpponentTurn == m_NowTurn)
        {
            // 自分がUser1
            if (GameInfo.MyTurn == Turn.User01)
            {
                // 一手目だったとき
                if (m_User2Square == 0)
                {
                    Debug.Log("一手目");
                    yield return m_Pieces[1].CoFirstPieceMove(m_Dice.DiceNumber);
                }
                // 二手目以降
                else
                {
                    yield return m_Pieces[1].CoPieceMove(m_User2Square, m_Dice.DiceNumber);
                }
                m_User2Square = GameInfo.Game.User2Square;
            }
            // 自分がUser2
            else if (GameInfo.MyTurn == Turn.User02)
            {
                // 一手目だったとき
                if (m_User1Square == 0)
                {
                    Debug.Log("一手目");
                    yield return m_Pieces[1].CoFirstPieceMove(m_Dice.DiceNumber);
                }
                // 二手目以降
                else
                {
                    yield return m_Pieces[1].CoPieceMove(m_User1Square, m_Dice.DiceNumber);
                }
                m_User1Square = GameInfo.Game.User1Square;
            }
        }
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
            // 取得したゲームデータを格納
            GameInfo.Game = gameData;

            // サイコロを振れる状態のときに、
            // 取得したゲームデータがサイコロの出目の値になっていたら…
            if(GameInfo.Game.DiceNumber > 0 && m_Dice.DiceNumber == 0)
            {
                Debug.Log("相手がサイコロを振った");
                yield return CoDiceRollSync();
            }
            
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
        if (m_Pieces[0].SquareNumber > m_Pieces[1].SquareNumber)
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