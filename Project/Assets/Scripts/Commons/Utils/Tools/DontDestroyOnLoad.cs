using UnityEngine;

/// <summary>
/// シーン遷移後も破棄されないオブジェクト
/// </summary>
/// <remarks>
/// このスクリプトがアタッチされているオブジェクトを親オブジェクトとして、
/// シーン遷移後も使いたいオブジェクトを子要素として追加していく。
/// </remarks>
public class DontDestroyOnLoad : MonoBehaviour
{
    /// <summary>
    /// 開始時に呼ばれる
    /// </summary>
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
}