using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ノーツのタップによる判定を行う抽象クラス
/// </summary>
public abstract class NotesJudgementBase : SingletonMonoBehaviour<NotesJudgementBase>
{
    [SerializeField]
    protected GameObject uiObj;

    // レーンの最大数。各継承先のStart()オーバーライドで設定してください
    protected int maxLaneNum;

    // プランナーレベルデザイン用
    // perfect ～ badの順に入力
    protected static readonly float[] GradesCriterion = {1.0f, 1.5f, 2, 3};     // 判定許容値
    public static readonly    int[]   GradesPoint     = {300, 200, 100, 10, 0}; // 各判定に応じたスコア

    // タップタイミングのグレード
    protected enum TimingGrade
    {
        Perfect = 0,
        Great,
        Good,
        Bad,
        Miss
    };

    // リザルト用
    public static          int   totalScore;               // 合計スコア
    public static          int   currentCombo;             // 現在のコンボ
    public static          int   bestCombo;                // プレイヤー最大コンボ
    public static readonly int[] TotalGrades = new int[5]; // 判定内訳（perfect ～ miss）

    // 内部用
    protected static ScoreManager scoreMgr;
    protected static ComboManager comboMgr;

    // OPTIMIZE: 現状一時変数として使われるので、今後必要なければ無駄な参照保持を減らすため解消する
    private static GameObject[] _drawGradeObjs;
    private static DrawGrade[]  _drawGrades;

    // タップ背景 ON/OFF 切り替え用
    private static   bool[] _tappedLane;     // 現在タップしているレーンの識別
    private static   bool[] _lastTappedLane; // 前フレームのタップ
    public static    bool[] justTap;         // ノーツをタップし、コンボが繋がる場合true
    public static    bool[] isHold;          // ロングノーツ識別
    protected static int[]  notesCount;      // 各レーンのノーツカウント

    protected static List<List<(GameObject gameObject, NotesSelector selector)>> GOListArray =
        new List<List<(GameObject gameObject, NotesSelector selector)>>(); // ノーツ座標格納用2次元配列
    // 使い方  GOListArray  [laneNumber]         [notesCount[laneNumber]]
    //        GOListArray  [何番目のレーンなのか] [何個目のノーツなのか[何番目のレーンなのか]]

    /// <summary>
    /// タップ判定時のタイミンググレードごとの処理を行う
    /// </summary>
    /// <param name="tapGrade">タップタイミングのグレード</param>
    /// <param name="laneNum">レーン番号</param>
    protected abstract void EvaluateGrades(TimingGrade tapGrade, int laneNum);

    /// <summary>
    /// タップ判定時のノーツの種類ごとの処理を行う
    /// </summary>
    /// <param name="notesType">ノーツの種類</param>
    /// <param name="laneNum">レーン番号</param>
    protected abstract void JudgeNotesType(NotesType notesType, int laneNum);

    /// <summary>
    /// 入力判定に基づいて各レーンのタップ状況を前フレームと比較し、ノーツを処理する
    /// </summary>
    /// <param name="tappedLane">現フレームの各クレーンのタップ状況</param>
    /// <param name="lastTappedLane">前フレームの各レーンのタップ状況</param>
    protected abstract void UpdateNotesDisplay(bool[] tappedLane, bool[] lastTappedLane);

    // Start is called before the first frame update
    protected virtual void Start()
    {
        maxLaneNum = NotesGeneratorBase.musicData.maxBlock;
        scoreMgr   = uiObj.GetComponent<ScoreManager>();
        comboMgr   = uiObj.GetComponent<ComboManager>();

        _drawGrades     = new DrawGrade[maxLaneNum];
        _drawGradeObjs  = new GameObject[maxLaneNum];
        _tappedLane     = new bool[maxLaneNum];
        _lastTappedLane = new bool[maxLaneNum];
        justTap         = new bool[maxLaneNum];
        isHold          = new bool[maxLaneNum];
        notesCount      = new int[maxLaneNum];

        // 評価UI表示用のスクリプト配列をセット
        for (int i = 0; i < maxLaneNum; i++)
        {
            string callObject = $"drawGrade{i}";

            _drawGradeObjs[i] = GameObject.Find(callObject);

            _drawGrades[i] = _drawGradeObjs[i].GetComponent<DrawGrade>();
            // 使い方
            // drawGrades[laneNumber].DrawGrades(grade(0～5));
        }
    }

    private void Update()
    {
        // タップフラグリセット
        for (int i = 0; i < maxLaneNum; i++)
        {
            _tappedLane[i] = false;
        }

        #region タップ入力判定

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            int laneNum = GetTappedLaneNumber();

            if (laneNum >= 0)
            {
                _tappedLane[laneNum] = true;
            }
        }
#endif

#if UNITY_IOS
        if (Input.touchCount > 0)
        {
            // タッチされている指の数だけ処理
            for (int i = 0; i < Input.touchCount; i++)
            {
                // タップしたレーンを取得
                int laneNumber = GetTappedLaneNumber(i);

                if (laneNumber == -1)
                    continue; // 処理を抜ける

                _tappedLane[laneNumber] = true;
            }
        }
#endif

        #endregion

        // 各レーンのタップ状況を前フレームと比較
        UpdateNotesDisplay(_tappedLane, _lastTappedLane);
    }

    private void LateUpdate()
    {
        for (int i = 0; i < maxLaneNum; i++)
        {
            _lastTappedLane[i] = _tappedLane[i]; // 次フレームで比較するためタップ状況を保存
        }
    }

    /// <summary>
    /// タップした場所に応じてレーン番号を返します
    /// </summary>
    /// <param name="touchIndex">GetTouch</param>
    /// <returns>laneNumber</returns>
    private static int GetTappedLaneNumber(int touchIndex = -1)
    {
        int          laneNum = -1; // 例外処理用
        GameObject   clickedObj;   // 都度初期化
        Ray          ray;
        RaycastHit2D hit;

#if UNITY_EDITOR
        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        hit = Physics2D.Raycast(ray.origin, ray.direction, 10, 1);

        if (hit.collider)
        {
            clickedObj = hit.transform.gameObject;

            if (clickedObj != null && clickedObj.CompareTag("Lane"))
            {
                laneNum = int.Parse(clickedObj.name);
            }

            if (laneNum >= 0)
            {
                _tappedLane[laneNum] = true;
            }
        }
#endif

#if UNITY_IOS
        if (Input.touchCount > 0 && touchIndex >= 0)
        {
            // タッチ情報を取得
            Touch t = Input.GetTouch(touchIndex);

            // タップ時処理
            ray = Camera.main.ScreenPointToRay(t.position);
            hit = Physics2D.Raycast(ray.origin, ray.direction, 10f, 1);

            if (!hit) return laneNum;

            clickedObj = hit.transform.gameObject;

            if ((clickedObj != null) && (clickedObj.CompareTag("Lane"))) // tagでレーンを識別
            {
                string s = clickedObj.name; // レーン番号を取得
                laneNum = int.Parse(s);     // 文字列を数字に変換
            }
        }
#endif

        return laneNum;
    }

    /// <summary>
    /// 判定ライン - ノーツ座標でタップしたタイミングの正確さを返します
    /// </summary>
    /// <param name="laneNum">laneNumber</param>
    /// <param name="judgeLine">judgeLine</param>
    /// <returns>absTiming</returns>
    protected static float GetAbsTiming(float laneNum, float judgeLine) // 判定ライン　－　ノーツ
    {
        float tempTiming = laneNum - judgeLine;

        return Mathf.Abs(tempTiming); // 絶対値に変換
    }

    /// <summary>
    /// タップ精度からグレードを判定する
    /// </summary>
    /// <param name="timingAcc">タップタイミングの精度</param>
    /// <returns>判定結果</returns>
    private static TimingGrade GetGradeFromAccuracy(float timingAcc)
    {
        // タップグレード
        TimingGrade grade = TimingGrade.Miss;

        // グレード判別
        for (int i = 0; i < GradesCriterion.Length; i++)
        {
            if (!(timingAcc <= GradesCriterion[i])) continue;

            grade = (TimingGrade) i;

            break;
        }

        return grade;
    }

    /// <summary>
    /// 判定からノーツ破棄処理まで
    /// </summary>
    /// <param name="tapAcc">タップタイミングの精度</param>
    /// <param name="laneNum">レーン番号</param>
    protected static void JudgeGrade(float tapAcc, int laneNum)
    {
        TimingGrade tapGrade    = GetGradeFromAccuracy(tapAcc); // タップ判定
        int         addingScore = GradesPoint[(int) tapGrade];  // 加算スコア

        // 判定ごとのカウント加算（ミス時はからタップか要判定のため一時スキップ）
        if (tapGrade != TimingGrade.Miss)
        {
            TotalGrades[(int) tapGrade]++;
        }

        // エフェクト用 great以上で該当レーンをtrue
        if((int)tapGrade < 2)
        {
            justTap[laneNum] = true;
        }

        // 判定UI描画
        _drawGrades[laneNum].DrawGrades((int) tapGrade);

        Instance.EvaluateGrades(tapGrade, laneNum);

        // ミスでなければコンボおよびノーツ判定を処理
        if (tapGrade == TimingGrade.Miss) return;

        // 最大コンボ記憶
        if (currentCombo > bestCombo)
        {
            bestCombo = currentCombo;
        }

        // スコア加算・描画
        totalScore += addingScore;

        scoreMgr.DrawScore(totalScore);
        comboMgr.DrawCombo(currentCombo);

        int thisNotesType = GOListArray[laneNum][notesCount[laneNum]].selector.NotesType;

        Instance.JudgeNotesType((NotesType) thisNotesType, laneNum);
    }

    /// <summary>
    /// ノーツ破棄、配列カウントアップ
    /// </summary>
    /// <param name="laneNum">laneNumber</param>
    protected static void DestroyNotes(int laneNum)
    {
        (GameObject notesObj, NotesSelector notesSel) = GOListArray[laneNum][notesCount[laneNum]];
        Destroy(notesObj); // 該当ノーツ破棄
        notesObj = null;   // 多重タップを防ぐ
        notesSel = null;
        notesCount[laneNum]++; // 該当レーンのノーツカウント++
    }

    /// <summary>
    /// ノーツがスルーされた時の処理です
    /// </summary>
    /// <param name="laneNum">laneNumber</param>
    public static void NotesCountUp(int laneNum)
    {
        if (currentCombo > bestCombo)
        {
            bestCombo = currentCombo; // 最大コンボ記憶
        }

        currentCombo = 0;
        TotalGrades[4]++;

        comboMgr.DrawCombo(currentCombo);

        _drawGrades[laneNum].DrawGrades(4);

        DestroyNotes(laneNum);
    }

    public static void ListImport()
    {
        GOListArray = NotesManager.NotesPositions;
    }
}
