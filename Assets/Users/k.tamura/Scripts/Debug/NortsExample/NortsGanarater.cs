using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
public class NortsGanarater : MonoBehaviour
{
    [SerializeField, Header("ノーツを生成する元(Prefab)")]
    GameObject NotesPrefab = null,bar;
    [SerializeField,Header("生成先")]
    List<GameObject> NotesGen = null;
    [SerializeField]
    float NotesDistance = 0.001f;
    [SerializeField]
    int NotesSpeed = 1;
    float bpm = 0;
    string[] filePaths = System.IO.Directory.GetFiles(Application.streamingAssetsPath, "*.nts");
    NotesJson.MusicData musicData = new NotesJson.MusicData();
    bool Generated=false;
    bool PlayedBGM=false;
    private void Update()
    {

        if (Generated)
        { 
            NotesGen[0].transform.root.gameObject.transform.position -= new Vector3(0,bpm, 0)*Time.deltaTime;
            if (NotesGen[0].transform.root.gameObject.transform.position.y <= bar.transform.position.y && !PlayedBGM)
            {
                SoundManager.BGMSoundCue(0);
                PlayedBGM = true;
            }
        }
    }
    public void ButtonPush()
    {
        //        //今後ノーツデータを取得して曲選択画面に表示させるプログラム一部実装(ファイル操作)
        //        foreach (string filePath in filePaths)
        //        {
        //            if (System.IO.Path.GetExtension(filePath) != ".nts")
        //            {
        //                continue;
        //            }

        //            using (var stream = new System.IO.FileStream(
        //                    filePath,
        //                    System.IO.FileMode.Create))
        //            {
        //                
        //            }
        //        }
        

        FileInfo info = new FileInfo(Application.streamingAssetsPath + "/musics.nts");
        GenerateNotes(info);
        Debug.Log(musicData);
        Debug.Log(musicData.notes[0].lane);

        int BPM = musicData.BPM;
        Debug.LogError(BPM);
        Debug.Log(musicData.BPM);
        bpm = 60 / musicData.BPM;
        Debug.Log(bpm);
        for (int i = 0; musicData.notes.Length > i; i++)
        {
            int LaneNum = musicData.notes[i].lane;
            int NotesType = musicData.notes[i].type;
            int NotesNum = musicData.notes[i].num;

            Debug.Log("LaneNum:" + LaneNum + " NotesType:" + NotesType + " NotesNum:" + NotesNum);
            //GameObject GenNotes = Instantiate(NotesPrefab, new Vector3(NotesGen[LaneNum].transform.position.x, (NotesGen[LaneNum].transform.parent.gameObject.transform.position.y + NotesGen[LaneNum].transform.parent.root.gameObject.transform.position.y + NotesNum)*NotesSpeed, 0), Quaternion.identity);
            GameObject GenNotes = Instantiate(NotesPrefab, new Vector3(NotesGen[LaneNum].transform.position.x
                , (NotesGen[LaneNum].transform.parent.gameObject.transform.position.y  + NotesNum -1)*NotesSpeed
                , 0)
                , Quaternion.identity) as GameObject;
            GenNotes.name = "notes"+NotesNum.ToString();
            GenNotes.transform.parent = NotesGen[LaneNum].transform;
            Generated = true;

        }


    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="info"></param>
    public void GenerateNotes(FileInfo info)
    {
        StreamReader reader = new StreamReader(info.OpenRead());
        string MusicDatas = reader.ReadToEnd();
        musicData = LoadJson(MusicDatas);
    }
    /// <summary>
    /// Json読み込み
    /// </summary>
    /// <param name="MusicStrings"></param>
    /// <returns></returns>
    private NotesJson.MusicData LoadJson(string MusicStrings)
    {
        return JsonUtility.FromJson<NotesJson.MusicData>(MusicStrings);
    }


}