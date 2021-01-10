using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class NotesGeneratorBase : MonoBehaviour
{
    [SerializeField, Header("ノーツを生成する元(Prefab)")]
    protected GameObject NotesPrefab,
                         longNotes,
                         edgeStart,
                         edgeEnd;

    [SerializeField, Header("生成先")]
    protected List<GameObject> notesGen;

    protected static float NotesSpeed;
    protected static float speed;
    protected static float offset;        // 譜面の再生を遅らせる
    public static    bool  jacketIsFaded; // 演奏開始時のジャケット表示のフェードが終了しているか

    protected long  BgmTimes;
    protected float NotesSpeeds;

    //string[] filePaths = System.IO.Directory.GetFiles(Application.streamingAssetsPath, "*.nts");
    //ノーツデータ
    protected NotesJson.MusicData musicData;
    protected bool                Generated;

    protected bool PlayedBGM;
    // protected KeyJudge _judge;
    // protected float    fps;

    protected Vector3 move;

    // ルートオブジェクト
    protected GameObject rootObj;

    // とりあえずここで設定 musicselectsに移動するかも
    protected readonly float[] highSpeeds = {0.28f, 0.5f, 0.28f};
    protected readonly float[] Speeds     = {0.258f, 0.139f, 0.295f};
    protected readonly float[] offsets    = {0, -9.4f, -9.2f};

    /// <summary>
    /// ノーツの移動部分の計算
    /// </summary>
    protected abstract void CalculateNotesPositions();

    /// <summary>
    /// ノーツの移動部分の処理
    /// </summary>
    protected abstract void MoveNotes();

    /// <summary>
    /// ノーツ情報を読み込み、NotesManagerにそれを登録する
    /// </summary>
    protected abstract void LoadNotes();

    /// <summary>
    /// 初期化
    /// </summary>
    protected virtual void Start()
    {
        NotesSpeed = highSpeeds[MusicDatas.MusicNumber];
        speed      = Speeds[MusicDatas.MusicNumber];
        offset     = offsets[MusicDatas.MusicNumber];
        rootObj    = notesGen[0].transform.root.gameObject;
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
    /// ノーツ生成を行う
    /// </summary>
    public void NotesGenerate()
    {
        //ファイルの読み込み
        FileInfo info = new FileInfo(
            $"{Application.streamingAssetsPath}/{MusicDatas.NotesDataName}_{(int) MusicDatas.gameType}_{MusicDatas.difficultNumber}.nts");

        StreamReader reader  = new StreamReader(info.OpenRead());
        string       Musics_ = reader.ReadToEnd();
        musicData    = JsonUtility.FromJson<NotesJson.MusicData>(Musics_);
        SpeedMgr.BPM = musicData.BPM;

        // ノーツ情報の読み込み
        LoadNotes();

        ScoreManager.maxScore =
            NotesJudgementBase.GradesPoint[0] * musicData.notes.Length; // perfect時の得点 * 最大コンボ　で天井点を取得

        NotesJudgementBase.ListImport();
    }

    /// <summary>
    /// ノーツリストに追加する関数
    /// </summary>
    /// <param name="notes"></param>
    /// <param name="lane"></param>
    /// <param name="num"></param>
    protected static void NotesPositionAdd(GameObject notes, int lane, int num)
    {
        // OPTIMIZE: 現状の処理だとNotesManager.NotesPositionsの二次元配列の構造が "[ノーツ][レーン]" となってしまい、
        // また、都度先頭から空きを探しているので意図した正常な形式とならない様子（KeyJudgeでの読み取りおよび処理はなぜか動作）
        // NotesManager.NotesPositions[num][lane] = notes とすればとりあえず正しくなる、その場合KeyJudge側の調整が必要
        foreach (List<GameObject> t in NotesManager.NotesPositions.Where(t => t[lane] == null))
        {
            t[lane] = notes;

            break;
        }
    }
}
