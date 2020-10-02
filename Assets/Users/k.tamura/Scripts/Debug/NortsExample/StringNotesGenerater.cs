using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
/// <summary>
/// バイオリン用ノーツgenerator
/// </summary>
public class StringNotesGenerater : MonoBehaviour
{
    [SerializeField, Header("ノーツを生成する元(Prefab)")]
    GameObject NotesPrefab = null, longNotes = null, bar = null;
    [SerializeField,Header("生成先")]
    List<GameObject> NotesGen = null;
    [SerializeField]
    float NotesDistance = 0.001f;
    [SerializeField]
    float NotesSpeed = 1.0f;
    [SerializeField]
    float speed = 1;
    float bpm = 0;
    string[] filePaths = System.IO.Directory.GetFiles(Application.streamingAssetsPath, "*.nts");
    NotesJson.MusicData musicData = new NotesJson.MusicData();
    bool Generated=false;
    bool PlayedBGM=false;
    KeyJudge _judge;
    private void Update()
    {

        if (Generated)
        { 
            NotesGen[0].transform.root.gameObject.transform.position -= new Vector3(0,1, 0)*Time.deltaTime*NotesSpeed*speed;
            if (NotesGen[0].transform.root.gameObject.transform.position.y <= bar.transform.position.y && !PlayedBGM)
            {
                //SoundManager.BGMSoundCue(MusicDatas.cueMusic);
                SoundManager.BGMSoundCue(5);

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
        

        //FileInfo info = new FileInfo(Application.streamingAssetsPath + "/music"+MusicDatas.cueMusic+".nts");
        FileInfo info = new FileInfo(Application.streamingAssetsPath + "/music6.nts");

        GenerateNotes(info);
        Debug.Log(musicData);
        Debug.Log(musicData.notes[0].block);
        SpeedMgr.BPM = musicData.BPM;
        Debug.Log(musicData.BPM);
        bpm = 60 / musicData.BPM;
        Debug.Log(bpm);
        for (int i = 0; musicData.notes.Length > i; i++)
        {
            NotesManager.NotesPositions.Add(new List<GameObject>()); //nex
            for (int e = 0; e < 6; e++)
            {
                NotesManager.NotesPositions[i].Add(null);
            }
            int LaneNum = musicData.notes[i].block;
            int NotesType = musicData.notes[i].type;
            int NotesNum = musicData.notes[i].num;

            Debug.Log("LaneNum:" + LaneNum + " NotesType:" + NotesType + " NotesNum:" + NotesNum);
            //GameObject GenNotes = Instantiate(NotesPrefab, new Vector3(NotesGen[LaneNum].transform.position.x, (NotesGen[LaneNum].transform.parent.gameObject.transform.position.y + NotesGen[LaneNum].transform.parent.root.gameObject.transform.position.y + NotesNum)*NotesSpeed, 0), Quaternion.identity);
            if (NotesType == 1)
            {
                GameObject GenNotes = null;
                if (LaneNum < 4)
                {
                    GenNotes = Instantiate(NotesPrefab, new Vector3(NotesGen[LaneNum].transform.position.x
                        , (NotesGen[LaneNum].transform.parent.gameObject.transform.position.y + NotesNum - 1) * NotesSpeed
                        , 0)
                        , Quaternion.identity) as GameObject;
                }
                else
                {
                    GenNotes = Instantiate(NotesPrefab, new Vector3(NotesGen[LaneNum].transform.position.x
                        , (NotesGen[LaneNum].transform.parent.gameObject.transform.position.x + NotesNum - 1) * NotesSpeed
                        , 0)
                        , Quaternion.identity) as GameObject;
                }
                GenNotes.name = "notes_" + NotesNum.ToString();
                GenNotes.transform.parent = NotesGen[LaneNum].transform;
                notesPositionAdd(GenNotes, LaneNum, i);
            }
            else if(NotesType == 2)
            {
                int notesNum2 = musicData.notes[i].notes[0].num;
                GameObject GenNotes = Instantiate(longNotes, new Vector3(NotesGen[LaneNum].transform.position.x
                    , (NotesGen[LaneNum].transform.parent.gameObject.transform.position.x + NotesNum - 1) * NotesSpeed
                    , 0)
                    , Quaternion.identity) as GameObject;
                GenNotes.name = "notes_" + NotesNum.ToString();
                GenNotes.transform.parent = NotesGen[LaneNum].transform;
                Vector2 longPos = new Vector2(0.23f,notesNum2-NotesNum);
                longPos.y *= 0.03f;
                GenNotes.transform.localScale = longPos;
                notesPositionAdd(GenNotes, LaneNum, i);
            }
            

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
        for (int i = 0; i < NotesManager.NotesPositions.Count; i++)
        {
            if (NotesManager.NotesPositions[i][Lane] == null)
            {
                NotesManager.NotesPositions[i][Lane] = notes;
                break;
            }
        }
    }

}