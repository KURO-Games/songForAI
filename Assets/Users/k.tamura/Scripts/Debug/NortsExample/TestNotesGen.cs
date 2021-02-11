using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
/// <summary>
/// バイオリン用　テスト
/// </summary>
public class TestNotesGen : MonoBehaviour
{
    [SerializeField, Header("ノーツを生成する元(Prefab)")]
    GameObject NotesPrefab = null, longNotes = null, verticalBar = null, horizonBar = null;
    [SerializeField,Header("生成先")]
    List<GameObject> verticalNotesGen = null, horizonNotesGen = null;
    [SerializeField]
    float NotesDistance = 0.001f;
    [SerializeField]
    float NotesSpeed = 1.0f;
    [SerializeField]
    float speed = 1;
    float bpm = 0;
    //string[] filePaths = System.IO.Directory.GetFiles(Application.streamingAssetsPath, "*.nts");
    //ノーツデータ
    NotesJson.MusicData musicData = new NotesJson.MusicData();
    bool Generated=false;
    bool PlayedBGM=false;
    //StringJudge _judge;
    float fps;

    Vector3 move_y;
    Vector3 move_x;
    /// <summary>
    /// Update 主にノーツの移動部分の計算をしている
    /// </summary>
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
            //Debug.Log(a+" "+b+" "+c);
            move_y = new Vector3(0, c*speed, 0);
            move_x = new Vector3(c * speed, 0, 0);
            if (!PlayedBGM)
            {
                //SoundManager.BGMSoundCue(MusicDatas.cueMusic);
                SoundManager.BGMSoundCue(5);

                PlayedBGM = true;
            }
        }
    }
    /// <summary>
    /// LateUpdate 主にノーツの移動部分の処理をしている
    /// </summary>
    private void LateUpdate()
    {
        verticalNotesGen[0].transform.parent.gameObject.transform.position -= move_y * NotesSpeed;
        horizonNotesGen[0].transform.parent.gameObject.transform.position += move_x * NotesSpeed;
    }
    /// <summary>
    /// 初期化
    /// </summary>
    private void Start()
    {
        Application.targetFrameRate = 60;
    }
    /// <summary>
    /// ノーツ生成を行う関数
    /// </summary>
    public void NotesGenerate()
    {
        //ファイルの読み込み
        //FileInfo info = new FileInfo(Application.streamingAssetsPath + "/music"+MusicDatas.cueMusic+".nts");
        FileInfo info = new FileInfo(Application.streamingAssetsPath + "/stringTest.nts");
        Debug.Log(info);
        StreamReader reader = new StreamReader(info.OpenRead());
        string MusicDatas = reader.ReadToEnd();
        musicData = JsonUtility.FromJson<NotesJson.MusicData>(MusicDatas);
        SpeedMgr.BPM = musicData.BPM;

        // ノーツ生成
        for (int i = 0; musicData.notes.Length > i; i++)
        {
            // リスト初期化
            NotesManager.NotesPositions.Add(new List<NotesInfo>()); //nex

            // ノーツデータを変数に代入
            int LaneNum = musicData.notes[i].block;
            int NotesType = musicData.notes[i].type;
            int NotesNum = musicData.notes[i].num;
            bpm = musicData.BPM;

            if(LaneNum < 4)
            {
                // ノーツの種類判別
                if (NotesType == 1)
                {
                    //生成
                    GameObject GenNotes = Instantiate(NotesPrefab, new Vector3(verticalNotesGen[LaneNum].transform.position.x
                        , 0
                        , 0)
                        , Quaternion.identity) as GameObject;
                    GenNotes.name = "notes_" + NotesNum.ToString();
                    GenNotes.transform.parent = verticalNotesGen[LaneNum].transform;
                    Vector3 positions = new Vector3(0, (NotesNum + 1) * NotesSpeed, 0);
                    GenNotes.transform.localPosition = new Vector3(0, 0, 0);
                    GenNotes.transform.localPosition += positions;
                    notesPositionAdd(GenNotes, LaneNum, i);
                }
                else if (NotesType == 2)
                {
                    int notesNum2 = musicData.notes[i].notes[0].num;
                    GameObject GenNotes = Instantiate(longNotes, new Vector3(verticalNotesGen[LaneNum].transform.position.x
                        , NotesNum * NotesSpeed
                        , 0)
                        , Quaternion.identity) as GameObject;
                    GenNotes.name = "notes_" + NotesNum.ToString();
                    GenNotes.transform.parent = verticalNotesGen[LaneNum].transform;
                    Vector2 longPos = new Vector2(0.23f, notesNum2 - NotesNum);
                    longPos.y *= 0.03f * (16 / musicData.notes[i].LPB);
                    GenNotes.transform.localScale = longPos;
                    notesPositionAdd(GenNotes, LaneNum, i);
                }
            }
            else
            {
                if (NotesType == 1)
                {
                    //生成
                    GameObject GenNotes = Instantiate(NotesPrefab, new Vector3(verticalNotesGen[LaneNum].transform.position.x
                        , 0
                        , 0)
                        , Quaternion.identity) as GameObject;
                    GenNotes.name = "notes_" + NotesNum.ToString();
                    GenNotes.transform.parent = verticalNotesGen[LaneNum].transform;
                    Vector3 positions = new Vector3(0, (NotesNum + 1) * NotesSpeed, 0);
                    GenNotes.transform.localPosition = new Vector3(0, 0, 0);
                    GenNotes.transform.localPosition += positions;
                    notesPositionAdd(GenNotes, LaneNum, i);
                }
                else if (NotesType == 2)
                {
                    int notesNum2 = musicData.notes[i].notes[0].num;
                    GameObject GenNotes = Instantiate(longNotes, new Vector3(verticalNotesGen[LaneNum].transform.position.x
                        , NotesNum * NotesSpeed
                        , 0)
                        , Quaternion.identity) as GameObject;
                    GenNotes.name = "notes_" + NotesNum.ToString();
                    GenNotes.transform.parent = verticalNotesGen[LaneNum].transform;
                    Vector2 longPos = new Vector2(0.23f, notesNum2 - NotesNum);
                    longPos.y *= 0.03f * (16 / musicData.notes[i].LPB);
                    GenNotes.transform.localScale = longPos;
                    notesPositionAdd(GenNotes, LaneNum, i);
                }
            }


            move_y = new Vector3(0, 1.06f*Time.deltaTime, 0);
            move_x = new Vector3(1.06f * Time.deltaTime, 0, 0);
            NotesManager.NextNotesLine.Add(LaneNum);
            Generated = true;

        }
        //StringJudge.ListImport();

    }
    /// <summary>
    /// ノーツリストに追加する関数
    /// </summary>
    /// <param name="notes"></param>
    /// <param name="Lane"></param>
    /// <param name="num"></param>
    private void notesPositionAdd(GameObject notes, int Lane, int num)
    {
        for (int i = 0; i < NotesManager.NotesPositions.Count; i++)
        {
            if (NotesManager.NotesPositions[i][Lane].GameObject == null)
            {
                NotesInfo notesInfo = NotesManager.NotesPositions[i][Lane];
                notesInfo.GameObject = notes;
                break;
            }
        }
    }

}
