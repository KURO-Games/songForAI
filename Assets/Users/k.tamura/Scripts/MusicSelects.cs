﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 曲の名前(enum)
/// </summary>
public enum MusicNames
{
    hoge,
}

public class MusicSelects : SingletonMonoBehaviour<MusicSelects>
{
    /// <summary>
    /// 曲名を上のEnumと同じ配列に入れる
    /// </summary>
    private readonly string[] musicNames = new string[]
    {
        "hoge"
    };

    /// <summary>
    /// ノーツの名前を配列通りに入力
    /// </summary>
    private readonly string[] musicNotesNames = new string[]
    {
        "hoge"
    };
    /// <summary>
    /// 曲のNum指定
    /// </summary>
    private readonly int[] cueMusicID = new int[]
    {
        1
    };
    /// <summary>
    /// 曲名を参照してenumを返す変数
    /// </summary>
    Dictionary<string, MusicNames> MusicNameDict=new Dictionary<string,MusicNames>
    {
        {"hoge",MusicNames.hoge },
    };
    /// <summary>
    /// 選択された曲の名前通りのデータをMusicDatasに入力
    /// </summary>
    /// <param name="selectMusicNames"></param>
    public static void MusicSelector(MusicNames selectMusicNames)
    {
        MusicDatas.MusicName = Instance.musicNames[(int)selectMusicNames];
        MusicDatas.NotesDataName = Instance.musicNotesNames[(int)selectMusicNames];
        MusicDatas.cueMusic = Instance.cueMusicID[(int)selectMusicNames];
    }
    /// <summary>
    /// 曲名のstring配列を返す
    /// </summary>
    /// <returns></returns>
    public static string[] MusicNameArray()
    {
        return Instance.musicNames;
    }
    /// <summary>
    /// 曲名と連動したenumを返す
    /// </summary>
    /// <returns></returns>
    public static Dictionary<string,MusicNames> MusicDict()
    {
        return Instance.MusicNameDict;
    }
}

/// <summary>
/// サンプルコード
/// </summary>
public class hoge {
    /// <summary>
    /// 曲リストを変数に保存
    /// </summary>
    string[] mArray = MusicSelects.MusicNameArray();
    /// <summary>
    /// Dictionaryを変数に保存
    /// </summary>
    Dictionary<string,MusicNames> musicdict = MusicSelects.MusicDict();
    private void Hoge()
    {
        // 曲名に基づいてenumを出力させる
        MusicNames musicNames = musicdict[mArray[0]];
        // enumを送って曲名などを保存
        MusicSelects.MusicSelector(musicNames);
    }
}