using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
public class NotesGenerater : MonoBehaviour
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
    float fps;

    Vector3 move;
    private void Update()
    {
        fps = 1 / Time.deltaTime;
        //Debug.Log(fps);
        if (Generated)
        {
            //NotesGen[0].transform.root.gameObject.transform.position -= move*Time.deltaTime*NotesSpeed*speed;
            float a = 60 / bpm;
            float b = a * fps;
            float c = b / 8;
            Debug.Log(a+" "+b+" "+c);
            move = new Vector3(0, c*speed, 0);

            if (!PlayedBGM)
            {
                //SoundManager.BGMSoundCue(MusicDatas.cueMusic);
                SoundManager.BGMSoundCue(6);

                PlayedBGM = true;
            }
        }
    }

    private void LateUpdate()
    {
        NotesGen[0].transform.root.gameObject.transform.position -= move * NotesSpeed;
    }
    private void Start()
    {
        Application.targetFrameRate = 60;
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
        FileInfo info = new FileInfo(Application.streamingAssetsPath + "/music7.nts");

        GenerateNotes(info);
        Debug.Log(musicData);
        Debug.Log(musicData.notes[0].block);
        SpeedMgr.BPM = musicData.BPM;
        Debug.Log(musicData.BPM);

        for (int i = 0; musicData.notes.Length > i; i++)
        {
            NotesManager.NotesPositions.Add(new List<GameObject>()); //nex
            for (int e = 0; e < 8; e++)
            {
                NotesManager.NotesPositions[i].Add(null);
            }
            int LaneNum = musicData.notes[i].block;
            int NotesType = musicData.notes[i].type;
            int NotesNum = musicData.notes[i].num;
            bpm = musicData.BPM;
            Debug.Log("LaneNum:" + LaneNum + " NotesType:" + NotesType + " NotesNum:" + NotesNum);
            //GameObject GenNotes = Instantiate(NotesPrefab, new Vector3(NotesGen[LaneNum].transform.position.x, (NotesGen[LaneNum].transform.parent.gameObject.transform.position.y + NotesGen[LaneNum].transform.parent.root.gameObject.transform.position.y + NotesNum)*NotesSpeed, 0), Quaternion.identity);
            if (NotesType == 1)
            {
                GameObject GenNotes = Instantiate(NotesPrefab, new Vector3(NotesGen[LaneNum].transform.position.x
                    , 0
                    , 0)
                    , Quaternion.identity) as GameObject;
                GenNotes.name = "notes_" + NotesNum.ToString();
                GenNotes.transform.parent = NotesGen[LaneNum].transform;
                Vector3 positions = new Vector3(0, (NotesNum +1) * NotesSpeed, 0);
                GenNotes.transform.localPosition = new Vector3(0, 0, 0);
                GenNotes.transform.localPosition += positions;
                notesPositionAdd(GenNotes, LaneNum, i);
            }
            else if(NotesType == 2)
            {
                int notesNum2 = musicData.notes[i].notes[0].num;
                GameObject GenNotes = Instantiate(longNotes, new Vector3(NotesGen[LaneNum].transform.position.x
                    , NotesNum * NotesSpeed
                    , 0)
                    , Quaternion.identity) as GameObject;
                GenNotes.name = "notes_" + NotesNum.ToString();
                GenNotes.transform.parent = NotesGen[LaneNum].transform;
                Vector2 longPos = new Vector2(0.23f,notesNum2-NotesNum);
                longPos.y *= 0.03f;
                GenNotes.transform.localScale = longPos;
                notesPositionAdd(GenNotes, LaneNum, i);
            }

            move = new Vector3(0, 1.06f*Time.deltaTime, 0);
            Debug.Log(move);
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