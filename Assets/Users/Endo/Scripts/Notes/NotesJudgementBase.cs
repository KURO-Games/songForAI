using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ノーツのタップによる判定を行う抽象クラス
/// </summary>
public abstract class NotesJudgementBase : SingletonMonoBehaviour<NotesJudgementBase>
{
    [SerializeField]
    protected GameObject uiObj;

    // レーンの最大数
    protected static int            maxLaneNum;
    protected static RaycastHit2D[] tapRayHits = new RaycastHit2D[0];
    private static   Camera         _camera;

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
    private static bool[] _tappedLane;     // 現在タップしているレーンの識別
    private static bool[] _lastTappedLane; // 前フレームのタップ
    public static  bool[] justTap;         // ノーツをタップし、コンボが繋がる場合true
    public static  bool[] isHold;          // ロングノーツ識別
    public static  int[]  notesCount;      // 各レーンのノーツカウント
    public         bool[] isHoldView;
    public         int[]  notesCountView;

    public static List<List<(GameObject gameObject, NotesSelector selector)>> GOListArray =
        new List<List<(GameObject gameObject, NotesSelector selector)>>(); // ノーツ座標格納用2次元配列
    // 使い方  GOListArray  [laneNumber]         [notesCount[laneNumber]]
    //        GOListArray  [何番目のレーンなのか] [何個目のノーツなのか[何番目のレーンなのか]]

    /// <summary>
    /// タップ判定時のタイミンググレードごとの処理を行う
    /// </summary>
    /// <param name="laneNum">レーン番号</param>
    /// <param name="tapGrade">タップタイミングのグレード</param>
    protected abstract void EvaluateGrades(int laneNum, TimingGrade tapGrade);

    /// <summary>
    /// タップ判定時のノーツの種類ごとの処理を行う
    /// </summary>
    /// <param name="laneNum">レーン番号</param>
    /// <param name="notesType">ノーツの種類</param>
    /// <param name="slideSection">スライドの節の種類</param>
    protected abstract void JudgeNotesType(int laneNum, NotesType notesType, SlideNotesSection? slideSection);

    /// <summary>
    /// 入力判定に基づいて各レーンのタップ状況を前フレームと比較し、ノーツを処理する
    /// </summary>
    /// <param name="tappedLane">現フレームの各クレーンのタップ状況</param>
    /// <param name="lastTappedLane">前フレームの各レーンのタップ状況</param>
    protected abstract void UpdateNotesDisplay(bool[] tappedLane, bool[] lastTappedLane);

    // Start is called before the first frame update
    protected virtual void Start()
    {
        totalScore   = 0;
        currentCombo = 0;
        bestCombo    = 0;
        Array.Clear(TotalGrades, 0, TotalGrades.Length);

        maxLaneNum = NotesGeneratorBase.musicData.maxBlock;
        scoreMgr   = uiObj.GetComponent<ScoreManager>();
        comboMgr   = uiObj.GetComponent<ComboManager>();
        _camera    = Camera.main;

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

        isHoldView     = isHold;
        notesCountView = notesCount;
    }

    private void LateUpdate()
    {
        for (int i = 0; i < maxLaneNum; i++)
        {
            _lastTappedLane[i] = _tappedLane[i]; // 次フレームで比較するためタップ状況を保存
        }

        Array.Clear(tapRayHits, 0, tapRayHits.Length);
    }

    /// <summary>
    /// タップした場所に応じてレーン番号を返します
    /// </summary>
    /// <param name="touchIndex">GetTouch</param>
    /// <returns>laneNumber</returns>
    private static int GetTappedLaneNumber(int touchIndex = -1)
    {
        int        laneNum = -1; // 例外処理用
        GameObject clickedObj;   // 都度初期化
        Ray        ray;

#if UNITY_EDITOR
        ray        = _camera.ScreenPointToRay(Input.mousePosition);
        tapRayHits = Physics2D.RaycastAll(ray.origin, ray.direction, 10, 1);

        foreach (RaycastHit2D hit in tapRayHits)
        {
            clickedObj = hit.transform.gameObject;

            // レーンを取得できていれば番号取得
            if (clickedObj == null || !clickedObj.CompareTag("Lane")) continue;

            laneNum = int.Parse(clickedObj.name);

            break;
        }
#endif

#if UNITY_IOS
        if (Input.touchCount > 0 && touchIndex >= 0)
        {
            // タッチ情報を取得
            Touch t = Input.GetTouch(touchIndex);

            // タップ時処理
            ray = _camera.ScreenPointToRay(t.position);
            RaycastHit2D[] hits = Physics2D.RaycastAll(ray.origin, ray.direction, 10f, 1);

            if (hits.Length == 0) return laneNum;

            foreach (RaycastHit2D hit in hits)
            {
                clickedObj = hit.transform.gameObject;

                // バイオリンのスライドレーンをタップしてたら一時情報保持
                if (clickedObj.CompareTag("SlidableArea"))
                {
                    tapRayHits = hits;

                    continue;
                }

                // レーンを取得できていれば番号取得
                if (clickedObj == null || !clickedObj.CompareTag("Lane")) continue;

                laneNum = int.Parse(clickedObj.name); // 文字列を数字に変換

                break;
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
    // TODO: GetGradeFromAccuracy()と統合
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
    protected static TimingGrade GetGradeFromAccuracy(float timingAcc)
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
    protected virtual void JudgeGrade(int laneNum, float tapAcc)
    {
        TimingGrade tapGrade    = GetGradeFromAccuracy(tapAcc); // タップ判定
        int         addingScore = GradesPoint[(int) tapGrade];  // 加算スコア

        // 判定ごとのカウント加算（ミス時は空タップか要判定のため一時スキップ）
        if (tapGrade != TimingGrade.Miss)
        {
            TotalGrades[(int) tapGrade]++;
        }

        // エフェクト用 great以上で該当レーンをtrue
        if ((int) tapGrade < 2)
        {
            justTap[laneNum] = true;
        }

        // 空タップじゃなければ判定UI描画
        if (tapGrade != TimingGrade.Miss || isHold[laneNum])
        {
            _drawGrades[laneNum].DrawGrades((int) tapGrade);
        }

        Instance.EvaluateGrades(laneNum, tapGrade);

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

        NotesSelector thisNotesSel = GOListArray[laneNum][notesCount[laneNum]].selector;

        Instance.JudgeNotesType(laneNum, thisNotesSel.notesType, thisNotesSel.slideSection);
    }

    /// <summary>
    /// ノーツ破棄、配列カウントアップ
    /// </summary>
    /// <param name="laneNum">laneNumber</param>
    protected virtual void DestroyNotes(int laneNum)
    {
        (GameObject notesObj, NotesSelector _) = GOListArray[laneNum][notesCount[laneNum]];

        Destroy(notesObj);     // 該当ノーツ破棄
        notesCount[laneNum]++; // 該当レーンのノーツカウント++
    }

    /// <summary>
    /// ノーツがスルーされた時の処理です
    /// </summary>
    /// <param name="laneNum">laneNumber</param>
    /// <param name="doDestroy">ノーツを破棄するかどうか</param>
    public static void NotesCountUp(int laneNum, bool doDestroy = true)
    {
        if (currentCombo > bestCombo)
        {
            bestCombo = currentCombo; // 最大コンボ記憶
        }

        currentCombo = 0;
        TotalGrades[4]++;

        comboMgr.DrawCombo(currentCombo);

        _drawGrades[laneNum].DrawGrades(4);

        if (doDestroy)
        {
            Instance.DestroyNotes(laneNum);
        }
        else
        {
            notesCount[laneNum]++;
        }
    }

    public static void ListImport()
    {
        GOListArray = NotesManager.NotesPositions;
    }
}
