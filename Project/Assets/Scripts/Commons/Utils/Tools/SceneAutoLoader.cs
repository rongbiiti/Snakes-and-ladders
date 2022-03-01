using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 自動的にシーンが読み込まれるクラス
/// </summary>
public class SceneAutoLoader
{
    /// <summary>
    /// ゲーム開始時（シーン読み込み前）に実行される
    /// </summary>
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void LoadScene()
    {

        // シーンが有効でない時（まだ読み込んでいない時）だけ追加ロードするように
        if (!SceneManager.GetSceneByName(SceneName.Initialize).IsValid())
        {
            SceneManager.LoadScene(SceneName.Initialize, LoadSceneMode.Additive);
        }
    }
}