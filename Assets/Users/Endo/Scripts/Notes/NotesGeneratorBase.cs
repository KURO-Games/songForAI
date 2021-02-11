using System.Collections.Generic;
using System.IO;
using UnityEngine;

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
    public static NotesJson.MusicData MusicData;
    protected     bool                Generated;

    protected bool PlayedBGM;

    protected Vector3 move;

    // ルートオブジェクト
    protected GameObject rootObj;

    // とりあえずここで設定 musicselectsに移動するかも
    private readonly float[]   _highSpeeds = {0.28f, 0.5f, 0.28f};
    private readonly float[]   _Speeds     = {0.258f, 0.139f, 0.295f};
    private readonly float[][] _offsets    = {new[] {35.4f, 24, 31}, new[] {35.4f, 27, 33}, new[] {35.4f, 20, 31}};

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
        NotesSpeed    = _highSpeeds[MusicDatas.MusicNumber];
        speed         = _Speeds[MusicDatas.MusicNumber];
        offset        = _offsets[(int) MusicDatas.gameType][MusicDatas.MusicNumber];
        jacketIsFaded = false;
        rootObj       = notesGen[0].transform.root.gameObject;
        MusicData     = default;

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
        MusicData = JsonUtility.FromJson<NotesJson.MusicData>(Musics_);

        // ノーツ格納リスト初期化
        NotesManager.NotesPositions.Clear();

        for (int i = 0; i < MusicData.maxBlock; i++)
        {
            NotesManager.NotesPositions.Add(new List<NotesInfo>());
        }

        // ノーツ情報の読み込み
        LoadNotes();

        ScoreManager.maxScore =
            NotesJudgementBase.GradesPoint[0] * MusicData.notes.Length; // perfect時の得点 * 最大コンボ　で天井点を取得

        NotesJudgementBase.ListImport();
    }

    /// <summary>
    /// ノーツリストに追加する関数
    /// </summary>
    /// <param name="notes"></param>
    /// <param name="lane"></param>
    protected static void NotesPositionAdd(GameObject notes, int lane)
    {
        NotesSelector selector = notes.GetComponent<NotesSelector>();
        NotesManager.NotesPositions[lane].Add(new NotesInfo(notes, selector));
    }
}
