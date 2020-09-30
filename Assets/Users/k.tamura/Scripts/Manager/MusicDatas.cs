using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 各シーンでデータを共有するためのスクリプト
/// </summary>
public class MusicDatas : SingletonMonoBehaviour<MusicDatas>
{
    public static string MusicName;
    public static int allNotes;
    public static int difficultNumber;
    public static int difficultLevel;
    public static int cueMusic;
    

    private void Start()
    {
        DontDestroyOnLoad(this);
    }
}
