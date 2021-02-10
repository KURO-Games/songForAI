using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 曲の名前(enum)
/// </summary>
public enum MusicNames
{
    Shining = 0,
    YourSmile = 1,
    DevilCastle = 2
}

public static class MusicSelects
{
    /// <summary>
    /// 曲名を上のEnumと同じ配列に入れる
    /// </summary>
    public static readonly string[] musicNames = new string[]
    {
        "シャイニングスター",
        "君の笑顔",
        "魔王城"
    };

    /// <summary>
    /// ノーツの名前を配列通りに入力
    /// </summary>
    public static readonly string[] musicNotesNames = new string[]// loadで使うのでpublic
    {
        "Shining",
        "YourSmile",
        "DevilCastle"
    };

    /// <summary>
    /// 演奏方法、曲ごとの難易度
    /// </summary>
    public static readonly int[,,] musicDifficulty = new int[3, 3, 4]
    {
        {
            { 2, 8, 12, 17 }, { 3, 6, 13, 20 }, { 5, 12, 18, 28 }
        },
        {
            { 1, 2, 3, 4 }, { 10, 11, 12, 13 }, { 10, 20, 30, 40 }
        },
        {
            { 0, 0, 0, 0 }, { 0, 0, 0, 0 }, { 0, 0, 0, 0 }
        }
    };

    /// <summary>
    /// 曲のNum指定
    /// </summary>
    private static readonly int[] cueMusicID = new int[]
    {
        5,
        6,
        7

    };
    /// <summary>
    /// 曲名を参照してenumを返す変数
    /// </summary>
    private static readonly Dictionary<string, MusicNames> MusicNameDict = new Dictionary<string, MusicNames>
    {
        {"Shining",MusicNames.Shining },
        {"YourSmile",MusicNames.YourSmile },
        {"DevilCastle",MusicNames.DevilCastle }
    };
    /// <summary>
    /// 選択された曲の名前通りのデータをMusicDatasに入力
    /// </summary>
    /// <param name="selectMusicNames"></param>
    public static void MusicSelector(MusicNames selectMusicNames)
    {
        MusicDatas.MusicName = musicNames[(int)selectMusicNames];
        MusicDatas.NotesDataName = musicNotesNames[(int)selectMusicNames];
        MusicDatas.cueMusic = cueMusicID[(int)selectMusicNames];
    }
    /// <summary>
    /// 曲名のstring配列を返す
    /// </summary>
    /// <returns></returns>
    public static string[] MusicNameArray()
    {
        return musicNotesNames;
    }
    /// <summary>
    /// 曲名と連動したenumを返す
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string, MusicNames> MusicDict()
    {
        return MusicNameDict;
    }
}
