using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using CriAtomDebugDetail;

public enum SoundType
{
    BGM = 0,
    SE,
    DemoBGM,
    Scenario,
    Num
}
public enum SoundRoadType
{
    Resources = 0,
    StreamingAsssets,
    Num,
}
/// <summary>
/// サウンドマネージャー
/// singleton化済
/// </summary>
public class SoundManager : SingletonMonoBehaviour<SoundManager>
{
    [SerializeField]
    private CriAtomSource BGMSource = default(CriAtomSource);
    [SerializeField]
    private CriAtomSource SESource = default(CriAtomSource);
    [SerializeField]
    private CriAtomSource DemoBgmSource = default(CriAtomSource);

    private static CriAtomExPlayer BGMExPlayer = default(CriAtomExPlayer);
    private static CriAtomExPlayer SEExPlayer = default(CriAtomExPlayer);
    private static CriAtomExPlayer DemoBGMExPlayer = default(CriAtomExPlayer);
    private static CriAtomExPlayer ScenarioExPlayer = default(CriAtomExPlayer);

    private static CriAtomExAcb BGMCueueSheet = default(CriAtomExAcb);
    private static CriAtomExAcb SeCueueSheet = default(CriAtomExAcb);
    private static CriAtomExAcb DemoBGMCueueSheet = default(CriAtomExAcb);
    private static CriAtomExAcb ScenarioCueueSheet = default(CriAtomExAcb);

    private static readonly string FilePath = Application.streamingAssetsPath + "/Sound/{0}.acb";
    private static readonly string ResourceFilePath = "sound/{0}";
    /// <summary>
    /// 初期化
    /// </summary>
    /// 
    private void OnEnable()
    {
        DontDestroyOnLoad(Instance);
        Instance.Initialize();
    }

    private void Initialize()
    {
        BGMExPlayer = new CriAtomExPlayer();
        SEExPlayer = new CriAtomExPlayer();
        DemoBGMExPlayer = new CriAtomExPlayer();
        ScenarioExPlayer = new CriAtomExPlayer();
        CriAtomEx.RegisterAcf(null, Application.streamingAssetsPath + "/Sound/sfai.acf");
        LoadAsyncCueSheet("Se", SoundType.SE);
        LoadAsyncCueSheet("Demo", SoundType.DemoBGM);

    }

    /// <summary>
    /// BGMを再生する関数 static化済
    /// </summary>
    /// <param name="cueID">cueID</param>
#if SFAI_SOUND
    public static void BGMSoundCue()
    {
        BGMExPlayer.Start();
    }
    public static void BGMSoundCue(int i)
    {
        BGMExPlayer.Start();
    }
#else
    public static void BGMSoundCue(int cueID)
    {
        Instance.BGMSource.Play(cueID);
    }
#endif
    /// <summary>
    /// SEを再生する関数 static化済
    /// </summary>
    /// <param name="cueID">cueID</param>
#if SFAI_SOUND
    public static void SESoundCue(string cueName)
    {
        SEExPlayer.SetCue(SeCueueSheet, cueName);
        SEExPlayer.Start();

    }
    public static void SESoundCue(int cueID)
    {
        SEExPlayer.SetCue(SeCueueSheet, cueID);
        SEExPlayer.Start();
    }
#else
    public static void SESoundCue(int cueID)
    {
        Instance.SESource.Play(cueID);
    }
#endif

    #region 動的にファイルをロード、アンロードする機能
#if SFAI_SOUND
    /// <summary>
    /// LoadSoundFile
    /// </summary>
    /// <param name="cueSheetName"></param>
    /// <param name="soundType"></param>
    /// <param name="soundRoadType"></param>
    public static void LoadAsyncCueSheet(string cueSheetName, SoundType soundType, SoundRoadType soundRoadType = SoundRoadType.StreamingAsssets)
    {
        string path = string.Empty;
        if (soundRoadType.Equals(SoundRoadType.StreamingAsssets))
        {
            path = string.Format(FilePath, cueSheetName);
            Instance.StartCoroutine(Instance.LoadCueSheetCoroutine(cueSheetName,path,soundType));
        }
    }

    public static void UnLoadCueSheet(string cueSheetName)
    {
        CriAtom.RemoveCueSheet(cueSheetName);
    }
    private IEnumerator LoadCueSheetCoroutine(string cueSheetName, string path, SoundType soundType)
    {
        CriAtom.AddCueSheetAsync(cueSheetName, path, "");

        while (CriAtom.CueSheetsAreLoading == true)
        {
            yield return null;
        }
        switch (soundType)
        {
            case SoundType.BGM:
                BGMCueueSheet = CriAtom.GetCueSheet(cueSheetName).acb;
                BGMExPlayer.SetCue(BGMCueueSheet, cueSheetName);
                BGMExPlayer.Prepare();
                break;
            case SoundType.SE:
                SeCueueSheet = CriAtom.GetCueSheet(cueSheetName).acb;
                break;
            case SoundType.DemoBGM:
                DemoBGMCueueSheet = CriAtom.GetCueSheet(cueSheetName).acb;
                break;
            case SoundType.Scenario:
                ScenarioCueueSheet = CriAtom.GetCueSheet(cueSheetName).acb;
                break;
            default:
                Debug.LogErrorFormat("Not Sound Type. Ex : {0}", soundType.ToString());
                break;

        }
    }
#endif
    #endregion
    #region シナリオ
#if SFAI_SOUND


#endif
    #endregion

    /// <summary>
    /// demo用BGMを流すファイル
    /// </summary>
    /// <param name="cueID"></param>
#if SFAI_SOUND
    public static void DemoBGMSoundCue(string cueName)
    {
        DemoBGMExPlayer.SetCue(DemoBGMCueueSheet, cueName);
        DemoBGMExPlayer.Start();
    }
    public static void DemoBGMSoundCue(int cueID)
    {
        DemoBGMExPlayer.SetCue(DemoBGMCueueSheet, cueID);
        DemoBGMExPlayer.Start();
    }
#else
    public static void DemoBGMSoundCue(int cueID)
    {
        Instance.DemoBgmSource.Play(cueID);
    }
#endif
    /// <summary>
    /// BGMを停止する関数 static化済
    /// </summary>
    public static void AllBGMSoundStop()
    {
        Instance.DemoBgmSource.Stop();
        Instance.BGMSource.Stop();
    }
    /// <summary>
    /// BGMを止めるためのプログラム
    /// </summary>
    public static void BGMStop()
    {
#if SFAI_SOUND
        BGMExPlayer.Stop();
#else
        Instance.BGMSource.Stop();
#endif
    }
    /// <summary>
    /// ポーズ設定
    /// </summary>
    /// <param name="isPause"></param>
    public static void BGMPause(bool isPause)
    {
#if SFAI_SOUND
        BGMExPlayer.Pause(isPause);
#else
        Instance.BGMSource.Pause(isPause);
#endif
    }



    /// <summary>
    /// BGMの現在のステータスを取得する関数 static化済
    /// </summary>
    /// <returns></returns>
#if SFAI_SOUND
    public static CriAtomExPlayer.Status BGMStatus()
    {
        return BGMExPlayer.GetStatus();
    }
#else
    public static CriAtomSource.Status BGMStatus()
    {
        return Instance.BGMSource.status;
    }
#endif
    /// <summary>
    /// BGMの再生時間の取得[ms]
    /// </summary>
    /// <param name="times"></param>
#if SFAI_SOUND
    public static void BgmTime(ref long times)
    {
        times = BGMExPlayer.GetTime();
    }
#else
    public static void BgmTime(ref long times)
    {
        times = Instance.BGMSource.time;
    }
#endif
}
