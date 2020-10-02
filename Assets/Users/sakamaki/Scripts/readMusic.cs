using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;
//using UnityEditor.UIElements;
using System;

public class readMusic : MonoBehaviour
{
    // 保管する用のListClass
    // 配列を使う
    private List<string[]> MusicList = new List<string[]>();

    void Start()
    {
        // csvFileを習得する
        string csvFilePath = Application.streamingAssetsPath + ("/Musics.csv");
        // Debug.log確認
        Debug.Log(csvFilePath);
        string csvFile = File.ReadAllText(csvFilePath);
        // Debug.log確認
        Debug.Log(csvFile);
        // ToString変換を行いcsvFileの中身をString型に変換を行う
        string stringcsv = csvFile.ToString();
        Debug.Log(stringcsv);

        string[] sep = stringcsv.Split('\n');
        for (int i = 0; sep.Length <= i; i++)
        { 
        Debug.Log(sep[i].Split(','));
        }
        MusicList.Add(sep);    }
}
