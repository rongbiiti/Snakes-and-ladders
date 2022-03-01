using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// タイトル管理用クラス
/// </summary>
public class TitleManager : UWRHelper
{
    /// <summary>
    /// Start
    /// </summary>
    protected virtual IEnumerator Start()
    {
        // 指定秒数待機...
        yield return new WaitForSeconds(0.1f);

        // TODO: ゲームデータ削除処理用
        // WebGLで起動されたら...
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            // ゲームKey取得
            yield return CoGetGameKey();

            // ユーザーID取得
            yield return CoGetUserID();
        }

        // タイトルBGM再生
        AudioManager.I.PlayBgm(AudioKey.MainBgm, 0.5f);

        // クリックされるまで待機...
        yield return new WaitUntil(() => OnClick());

        // ボタンSE再生
        AudioManager.I.PlaySe(AudioKey.ButtonSE);

        // ルームシーンへ遷移
        SceneFadeManager.I.Load(SceneName.Room);

    }
    /// <summary>
    /// クリックされているか？
    /// </summary>
    /// <returns>TRUE: クリックされた FALSE: クリックされていない</returns>
    private bool OnClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            List<RaycastResult> results = new List<RaycastResult>();

            // マウスポインタの位置にレイ飛ばし、ヒットしたものを保存
            // ポインタ（マウス/タッチ）イベントに関連するイベントの情報
            var pointer = new PointerEventData(EventSystem.current);
            pointer.position = Input.mousePosition;
            EventSystem.current.RaycastAll(pointer, results);

            // UIがヒットしていればfalseを返す
            foreach (RaycastResult target in results)
            {
                return false;
            }

            // UIがヒットしていなればtrueを返す
            return true;
        }
        return false;
    }

    /// <summary>
    /// GameKey取得
    /// </summary>
    /// <returns></returns>
    private IEnumerator CoGetGameKey()
    {
        // ゲームが実行されている環境がWebGLの場合なら...
        // GameKey取得
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (GetClieParameters.m_GameKey != null)
            {
                KeyData.GameKey = GetClieParameters.m_GameKey;
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
        // ゲームが実行されている環境がWebGLの場合なら...
        // UserID取得
        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            if (GetClieParameters.m_UserId != null)
            {
                GameInfo.MyUserID = GetClieParameters.m_UserId;
            }
        }
        yield break;
    }
}
