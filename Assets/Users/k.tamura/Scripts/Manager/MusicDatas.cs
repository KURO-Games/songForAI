using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 演奏モードの種類
/// </summary>
/// <remarks>
/// None: -1, Piano: 1, Violin: 2, Drum: 3
/// </remarks>
public enum GameType
{
    None  = -1,
    Piano = 0,
    Violin = 1,
    Drum = 2
}

/// <summary>
/// ノーツの種類
/// </summary>
/// <remarks>
/// Single: 1, LongAndSlide: 2
/// </remarks>
public enum NotesType
{
    Single = 1,
    LongAndSlide
}

/// <summary>
/// 各シーンでデータを共有するためのスクリプト
/// </summary>
public class MusicDatas : SingletonMonoBehaviour<MusicDatas>
{
    public static string   MusicName;
    public static string   NotesDataName;
    public static int      MusicNumber; // 0から数える
    public static int      totalNotes;  // 両方start関数なのでとりあえずNotesGeneratorからScoreManagerに直接代入
    public static int      difficultNumber;
    public static int      difficultLevel;
    public static int      cueMusic;
    public static GameType gameType; // 選択中の演奏モード

    private void Start()
    {
        DontDestroyOnLoad(this);

        gameType = GameType.Piano;
        difficultNumber = 0;
        //cueMusic = 0;
    }
}
