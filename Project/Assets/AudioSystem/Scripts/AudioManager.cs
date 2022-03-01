using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Audio Key
/// </summary>
public class AudioKey
{
    /// <summary>
    /// メインBGM
    /// </summary>
    public const string MainBgm = "MainBgm";

    /// <summary>
    /// カードを配るSE
    /// </summary>
    public const string DealCardSE = "DealCardSE";

    /// <summary>
    /// カードをめくるSE
    /// </summary>
    public const string TurningOverCardSE = "TurningOverCardSE";

    /// <summary>
    /// カードペアSE
    /// </summary>
    public const string PairCardSE = "PairCardSE";

    /// <summary>
    /// 勝利SE
    /// </summary>
    public const string WinSE = "WinSE";

    /// <summary>
    /// 敗北SE
    /// </summary>
    public const string LoseSE = "LoseSE";

    /// <summary>
    /// ゲームスタートSE
    /// </summary>
    public const string GameStartSE = "GameStartSE";

    /// <summary>
    /// ボタンSE
    /// </summary>
    public const string ButtonSE = "ButtonSE";
}

/// <summary>
/// サウンド用Flg
/// </summary>
public enum SoundFlg
{
    /// <summary>
    /// サウンドを鳴らす
    /// </summary>
    ON,

    /// <summary>
    /// サウンドを鳴らさない
    /// </summary>
    OFF,
}

/// <summary>
/// Audio Manager Class
/// </summary>
public class AudioManager : SingletonMonoBehaviour<AudioManager>
{

    /// <summary>
    /// デフォルトのBgm音量
    /// </summary>
    private const float DefalutBgmVolume = 0.5f;

    /// <summary>
    /// デフォルトのSe音量
    /// </summary>
    private const float DefalutSeVolume = 0.5f;

    /// <summary>
    /// BGMキー名
    /// </summary>
    private string[] m_BgmKey = new string[]
    {
        AudioKey.MainBgm,
    };

    /// <summary>
    /// SEキー名
    /// </summary>
    private string[] m_SeKey = new string[]
    {
        AudioKey.DealCardSE,
        AudioKey.TurningOverCardSE,
        AudioKey.PairCardSE,
        AudioKey.WinSE,
        AudioKey.LoseSE,
        AudioKey.GameStartSE,
        AudioKey.ButtonSE,
    };

    /// <summary>
    /// 現在再生中のBGMキー
    /// </summary>
    private string m_CurrentBgMKey = string.Empty;
    public string CurrentBgMKey => m_CurrentBgMKey;

    /// <summary>
    /// 現在再生中のBGMの音量
    /// </summary>
    private float m_CurrentBGMVolume = 0;

    /// <summary>
    /// Bgmの音量の倍率
    /// この値を調整してBgmの音量調整をしている
    /// </summary>
    private float m_BgmVolumeMag = 0.2f;

    /// <summary>
    /// Seの音量の倍率
    /// この値を調整してSeの音量調整をしている
    /// </summary>
    private float m_SeVolumeMag = 0.2f;

    /// <summary>
    /// BGM音源
    /// </summary>
    [SerializeField] private AudioClip[] m_BgmFiles;

    /// <summary>
    /// SE音源
    /// </summary>
    [SerializeField] private AudioClip[] m_SeFiles;

    /// <summary>
    /// BGM再生オブジェクト
    /// </summary>
    [SerializeField] private AudioSource m_BgmSource;

    /// <summary>
    /// SE再生オブジェクト
    /// </summary>
    [SerializeField] private AudioSource m_SeSource;

    /// <summary>
    /// BGMが再生できるかどうか
    /// </summary>
    private SoundFlg m_PlayBgmFlg = SoundFlg.ON;

    /// <summary>
    /// SEが再生できるかどうか
    /// </summary>
    private SoundFlg m_PlaySeFlg = SoundFlg.ON;

    /// <summary>
    /// BGM一覧
    /// </summary>
    private Dictionary<string, AudioClip> m_BgmList = new Dictionary<string, AudioClip>();

    /// <summary>
    /// SE一覧
    /// </summary>
    private Dictionary<string, AudioClip> m_SeList = new Dictionary<string, AudioClip>();


    /// <summary>
    /// Awake
    /// </summary>
    protected override void Awake()
    {
        base.Awake();

        // BGM一覧
        for (int i = 0; i < m_BgmKey.Length; i++)
        {
            this.m_BgmList.Add(m_BgmKey[i], this.m_BgmFiles[i]);
        }

        // SE一覧
        for (int j = 0; j < m_SeKey.Length; j++)
        {
            this.m_SeList.Add(m_SeKey[j], this.m_SeFiles[j]);
        }

        // シーンをまたいでも破棄されないゲームオブジェクトにする。
        // DontDestroyOnLoad(this.gameObject);
    }

    /// <summary>
    /// 指定したBGMを再生する
    /// </summary>
    /// <param name="audioName">再生するBGMの名前</param>
    /// <param name="volume">音量</param>
    /// <param name="isLoop">BGMをループするか？</param>
    public void PlayBgm(string audioName, float volume = DefalutBgmVolume, bool isLoop = true)
    {
        // ループを設定
        m_BgmSource.loop = isLoop;

        // 現在のBGMキーを設定
        m_CurrentBgMKey = audioName;

        // 現在のBGMの音量設定
        m_CurrentBGMVolume = volume;

        // 再生不可の場合再生しない
        if (m_PlayBgmFlg == SoundFlg.OFF || m_CurrentBgMKey == string.Empty)
        {
            return;
        }

        // 再生中のBGMと指定されたBGMが異なる場合、BGMを変更する
        if (this.m_BgmSource.clip != this.m_BgmList[m_CurrentBgMKey])
        {
            this.m_BgmSource.volume = m_CurrentBGMVolume * m_BgmVolumeMag;
            this.m_BgmSource.time = 0.0f;
            this.m_BgmSource.Stop();
            this.m_BgmSource.clip = m_BgmList[audioName];

            this.m_BgmSource.Play();
        }
    }

    /// <summary>
    /// 指定したSEを再生する
    /// </summary>
    /// <param name="audioName">再生するSEの名前</param>
    /// <param name="volume">音量</param>
    public void PlaySe(string audioName, float volume = DefalutSeVolume)
    {
        // 再生不可の場合再生しない
        if (m_PlaySeFlg == SoundFlg.OFF || audioName == string.Empty) { return; }

        this.m_SeSource.volume = volume * m_SeVolumeMag;
        this.m_SeSource.PlayOneShot(m_SeList[audioName]);
    }

    /// <summary>
    /// 再生中のSEを停止する
    /// </summary>
    public void StopSe()
    {
        this.m_SeSource.Stop();
        this.m_SeSource.clip = null;
    }

    /// <summary>
    /// 再生中のBGMを停止する
    /// </summary>
    public void StopBgm()
    {
        this.m_BgmSource.Stop();
        this.m_BgmSource.clip = null;
    }

    /// <summary>
    /// 再生中のBGMを一時停止する
    /// </summary>
    public void PauseBgm()
    {
        this.m_BgmSource.Stop();
    }

    /// <summary>
    /// BGM再生の可否を設定する
    /// </summary>
    /// <param name="flag"></param>
    public void SetBgmFlg(SoundFlg flag)
    {
        m_PlayBgmFlg = flag;

        // 再生中のBGMを停止する
        if (m_PlayBgmFlg == SoundFlg.OFF)
        {
            StopBgm();
        }
    }

    /// <summary>
    /// SE再生の可否を設定する
    /// </summary>
    /// <param name="flag"></param>
    public void SetSeFlg(SoundFlg flag)
    {
        m_PlaySeFlg = flag;
    }

    /// <summary>
    /// BGM音量の倍率を設定
    /// </summary>
    /// <param name="value"></param>
    public void SetBgmVolumeMag(float value)
    {
        m_BgmVolumeMag = value;
        m_BgmSource.volume = m_CurrentBGMVolume * m_BgmVolumeMag;
    }

    /// <summary>
    /// SE音量の倍率を設定
    /// </summary>
    /// <param name="value">倍率の値</param>
    public void SetSeVolumeMag(float value)
    {
        m_SeVolumeMag = value;
    }

    /// <summary>
    /// BGM再生可否を取得する
    /// </summary>
    /// <returns></returns>
    public SoundFlg GetBgmFlg()
    {
        return m_PlayBgmFlg;
    }

    /// <summary>
    /// SE再生可否を取得する
    /// </summary>
    /// <returns></returns>
    public SoundFlg GetSeFlg()
    {
        return m_PlaySeFlg;
    }

    /// <summary>
    /// BGM音量の倍率を取得する
    /// </summary>
    /// <returns></returns>
    public float GetBgmVolumeMag()
    {
        return m_BgmVolumeMag;
    }

    /// <summary>
    /// SE音量の倍率を取得する
    /// </summary>
    /// <returns></returns>
    public float GetSeVolumeMag()
    {
        return m_SeVolumeMag;
    }

    /// <summary>
    /// BGMが再生中か？
    /// </summary>
    /// <returns></returns>
    public bool IsPlayingBgm()
    {
        return m_BgmSource.isPlaying;
    }
}
