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
        Initialize();
    }

    private void Initialize()
    {
        DontDestroyOnLoad(this);
        BGMExPlayer = new CriAtomExPlayer();
        SEExPlayer = new CriAtomExPlayer();
        DemoBGMExPlayer = new CriAtomExPlayer();
        ScenarioExPlayer = new CriAtomExPlayer();
        CriAtomEx.RegisterAcf(null, Application.streamingAssetsPath + "/Sound/sfai.acf");
        LoadAsyncCueSheet("Se", SoundType.SE);
        LoadAsyncCueSheet("Demo", SoundType.DemoBGM);
        DemoBGMExPlayer.AttachFader();
        DemoBGMExPlayer.SetFadeOutTime(3000);

    }

    /// <summary>
    /// BGMを再生する関数 static化済
    /// </summary>
    /// <param name="cueID">cueID</param>
    public static void BGMSoundCue()
    {
        BGMExPlayer.Start();
    }
    public static void BGMSoundCue(int i)
    {
        BGMExPlayer.Start();
    }
    /// <summary>
    /// SEを再生する関数 static化済
    /// </summary>
    /// <param name="cueID">cueID</param>
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

    #region 動的にファイルをロード、アンロードする機能
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
            LoadCueSheet(cueSheetName, path, soundType);
        }
    }

    private static void UnLoadCueSheet(string cueSheetName)
    {
        CriAtom.RemoveCueSheet(cueSheetName);
    }
    public static void UnLoadCueSheet(SoundType soundType)
    {
        switch (soundType)
        {
            case SoundType.BGM:
                if (BGMCueueSheet != default(CriAtomExAcb))
                    BGMCueueSheet.Dispose();
                break;
            case SoundType.Scenario:
                if (ScenarioCueueSheet != default(CriAtomExAcb))
                    ScenarioCueueSheet.Dispose();
                break;
        }
    }
    private static void LoadCueSheet(string cueSheetName, string path, SoundType soundType)
    {
        UnLoadCueSheet(soundType);
        CriAtom.AddCueSheetAsync(cueSheetName, path, "");
        switch (soundType)
        {
            case SoundType.BGM:
                BGMCueueSheet= CriAtomExAcb.LoadAcbFile(null,path,null);
                BGMExPlayer.SetCue(BGMCueueSheet, cueSheetName);
                BGMExPlayer.Prepare();
                break;
            case SoundType.SE:
                SeCueueSheet = CriAtomExAcb.LoadAcbFile(null, path, null);
                break;
            case SoundType.DemoBGM:
                DemoBGMCueueSheet = CriAtomExAcb.LoadAcbFile(null, path, null);
                break;
            case SoundType.Scenario:
                ScenarioCueueSheet = CriAtomExAcb.LoadAcbFile(null, path, null);
                break;
            default:
                Debug.LogErrorFormat("Not Sound Type. Ex : {0}", soundType.ToString());
                break;

        }
    }
    #endregion
    #region シナリオ
    public static void ScenarioSoundCue(string cueName)
    {
        SEExPlayer.SetCue(SeCueueSheet, cueName);
        SEExPlayer.Start();
    }
    public static void ScenarioSoundCue(int cueID)
    {
        SEExPlayer.SetCue(SeCueueSheet, cueID);
        SEExPlayer.Start();
    }

    #endregion

    /// <summary>
    /// demo用BGMを流すファイル
    /// </summary>
    /// <param name="cueID"></param>
    public static void DemoBGMSoundCue(string cueName)
    {
        DemoBGMExPlayer.SetCue(DemoBGMCueueSheet, cueName);
        DemoBGMExPlayer.Start();
    }
    public static void DemoBGMSoundCue(int cueID)
    {
        if (DemoBGMCueueSheet == default(CriAtomExAcb))
            LoadAsyncCueSheet("Demo", SoundType.DemoBGM);
        DemoBGMExPlayer.SetCue(DemoBGMCueueSheet, cueID);
        DemoBGMExPlayer.Start();
    }

    /// <summary>
    /// BGMを停止する関数 static化済
    /// </summary>
    public static void AllBGMSoundStop()
    {
        DemoBGMExPlayer.Stop();
        BGMExPlayer.Stop();
    }
    /// <summary>
    /// BGMを止めるためのプログラム
    /// </summary>
    public static void BGMStop()
    {
        BGMExPlayer.Stop();
    }

    public static void DemoStop()
    {
        DemoBGMExPlayer.Stop();
    }
    /// <summary>
    /// ポーズ設定
    /// </summary>
    /// <param name="isPause"></param>
    public static void BGMPause(bool isPause)
    {
        BGMExPlayer.Pause(isPause);
    }



    /// <summary>
    /// BGMの現在のステータスを取得する関数 static化済
    /// </summary>
    /// <returns></returns>
    public static CriAtomExPlayer.Status BGMStatus()
    {
        return BGMExPlayer.GetStatus();
    }
    /// <summary>
    /// BGMの再生時間の取得[ms]
    /// </summary>
    /// <param name="times"></param>
    public static void BgmTime(ref long times)
    {
        times = BGMExPlayer.GetTime();
    }
}
