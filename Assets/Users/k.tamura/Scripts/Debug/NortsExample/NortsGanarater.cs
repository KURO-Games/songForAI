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
    Judge _judge;
    private void Update()
    {

        if (Generated)
        { 
            NotesGen[0].transform.root.gameObject.transform.position -= new Vector3(0,1, 0)*Time.deltaTime;
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
        SpeedMgr.BPM = musicData.BPM;
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
            GenNotes.name = "notes_"+NotesNum.ToString();
            GenNotes.transform.parent = NotesGen[LaneNum].transform;

            notesPositionAdd(GenNotes, LaneNum, i);

            NotesManager.NextNotesLine.Add(LaneNum);
            Generated = true;

        }
        Judge.ListImport();

    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="info">FilePath</param>
    public void GenerateNotes(FileInfo info)
    {
        StreamReader reader = new StreamReader(info.OpenRead());
        string MusicDatas = reader.ReadToEnd();
        musicData = JsonUtility.FromJson<NotesJson.MusicData>(MusicDatas);
    }
    private void notesPositionAdd(GameObject notes, int Lane, int num)
    {
       
        NotesManager.NotesPositions.Add(new List<GameObject>()); //next
        for (int i = 0; i < Lane; i++)
        {
            NotesManager.NotesPositions[num].Add(null);
        }
        NotesManager.NotesPositions[num].Add(notes);

    }

}