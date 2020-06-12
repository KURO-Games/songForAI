using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
public class NortsGanarater : MonoBehaviour
{
    [SerializeField, Header("ノーツを生成する元(Prefab)")]
    GameObject notes = null;
    [SerializeField]
    List<GameObject> NotesGen = null;
    public static int Speed = 1000;

    ///デバッグ用。最終的には消す！
    private void Awake()
    {
        if (notes = null)
        {
            Debug.LogError("NotesGeneraterのnotesが指定されていません。");
        }
    }
    public void ButtonPush()
    {
        FileInfo info = new FileInfo(Application.streamingAssetsPath + "/musics.nts");
        StreamReader reader = new StreamReader(info.OpenRead());
        string json = reader.ReadToEnd();
        GenerateNotes(json);
    }
    public void GenerateNotes(string MusicDatas)
    {
        NotesJson musicData = LoadJson(MusicDatas);
        Debug.Log(JsonUtility.ToJson(musicData));
    }
    private NotesJson LoadJson(string MusicStrings)
    {
        return JsonUtility.FromJson<NotesJson>(MusicStrings);
    }


}
//歌詞を外部に頼んでる
//