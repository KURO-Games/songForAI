using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public abstract class NotesGeneratorBase : MonoBehaviour
{
    [SerializeField, Header("ノーツを生成する元(Prefab)")]
    protected GameObject NotesPrefab,
                         longNotes,
                         edgeStart,
                         edgeEnd;

    [SerializeField, Header("生成先")]
    protected List<GameObject> NotesGen;

    public static float NotesSpeed;
    public static float speed;
    public static float offset; // 譜面の再生を遅らせる

    protected long  BgmTimes;
    protected float NotesSpeeds;

    //string[] filePaths = System.IO.Directory.GetFiles(Application.streamingAssetsPath, "*.nts");
    //ノーツデータ
    protected NotesJson.MusicData musicData;
    protected bool                Generated;

    protected bool PlayedBGM;
    // protected KeyJudge            _judge;
    // protected float               fps;

    protected Vector3 move;

    // とりあえずここで設定 musicselectsに移動するかも
    protected float[] highSpeeds = {0.28f, 0.5f, 0.28f};
    protected float[] Speeds     = {0.258f, 0.139f, 0.295f};
    protected float[] offsets    = {0, -9.4f, -9.2f};

    /// <summary>
    /// 初期化
    /// </summary>
    protected virtual void Start()
    {
        NotesSpeed = highSpeeds[MusicDatas.MusicNumber];
        speed      = Speeds[MusicDatas.MusicNumber];
        offset     = offsets[MusicDatas.MusicNumber];
        //NotesSpeed = highSpeeds[1];
        //speed = Speeds[1];
        //offset = offsets[1];

        Application.targetFrameRate = 60;
    }

    protected virtual void Update()
    {
        CalculateNotesPositions();
    }

    protected virtual void LateUpdate()
    {
        MoveNotes();
    }

    /// <summary>
    /// ノーツの移動部分の計算
    /// </summary>
    protected abstract void CalculateNotesPositions();

    /// <summary>
    /// ノーツの移動部分の処理
    /// </summary>
    protected abstract void MoveNotes();

    /// <summary>
    /// ノーツ生成を行う
    /// </summary>
    public void NotesGenerate()
    {
        //ファイルの読み込み
        var info = new FileInfo(Application.streamingAssetsPath +
                                $"/{MusicDatas.NotesDataName}_{MusicDatas.difficultNumber}.nts");

        //FileInfo info = new FileInfo(Application.streamingAssetsPath + "/YourSmile_0.nts");
        //Debug.Log(info);
        var reader  = new StreamReader(info.OpenRead());
        var Musics_ = reader.ReadToEnd();
        musicData    = JsonUtility.FromJson<NotesJson.MusicData>(Musics_);
        SpeedMgr.BPM = musicData.BPM;

        // ノーツ情報の読み込み
        LoadNotes();

        ScoreManager.maxScore = Judge.gradesPoint[0] * musicData.notes.Length; // perfect時の得点 * 最大コンボ　で天井点を取得

        KeyJudge.ListImport();

        // 継承先では base.NotesGenerate() を呼び出した後に生成処理を書くこと
    }

    /// <summary>
    /// ノーツ情報を読み込み、NotesManagerにそれを登録する
    /// </summary>
    protected abstract void LoadNotes();

    /// <summary>
    /// ノーツリストに追加する関数
    /// </summary>
    /// <param name="notes"></param>
    /// <param name="lane"></param>
    /// <param name="num"></param>
    protected static void NotesPositionAdd(GameObject notes, int lane, int num)
    {
        foreach (var t in NotesManager.NotesPositions.Where(t => t[lane] == null))
        {
            t[lane] = notes;

            break;
        }
    }
}
