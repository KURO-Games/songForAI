using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StringJudge : MonoBehaviour
{
    public static int score = 0;                                         // スコア
    public static int combo = 0;                                         // 現在のコンボ
    public static int bestcombo = 0;                                     // リザルト用　最大コンボ
    public static int[] totalGrades = { 0, 0, 0, 0, 0 };                 // リザルト用　判定内訳（perfect ～ miss）
    public static int point = 0;                                         // 判定に応じた得点
    public static float comboMag = 1.0f;                                 // コンボに応じたスコア倍率
    private static int[] stNotesCount = { 0, 0, 0, 0, 0, 0 };            // レーンごとのノーツカウント
    private static int laneNumber = 0;                                   // タップしたレーン番号（0 ～ 7）
    private static float absTiming = 0;                                  // 判定ラインy - 該当ノーツy座標の絶対値


    // ノーツ座標格納用2次元配列          
    private static List<List<GameObject>> stGOListArray = new List<List<GameObject>>();
    //
    // 使い方  GOListArray   [_notesCount[laneNumber]]                   [laneNumber]
    //         GOListArray   [何個目のノーツなのか[何番目のレーンの]]    [何番目のレーンなのか]


    // タップ背景 ON/OFF 切り替え用
    private bool[] tapFlag = new bool[6];// 現在タップしているレーンの識別
    private bool[] lastTap = new bool[6];// 前フレームのタップ

    // 判定許容値
    [SerializeField] private float perfect;
    [SerializeField] private float great;
    [SerializeField] private float good;
    [SerializeField] private float bad;

    // その他
    GameObject clickObj;// タップしたレーンの情報を取得

    GameObject uiObj;   // 動的UI表示
    ScoreManager mg1;
    ComboManager mg2;

    [SerializeField] private GameObject verticalJudgeLine;  // 横ノーツ用判定ライン
    [SerializeField] private GameObject horizonJudgeLine;   // 縦ノーツ用判定ライン
    [SerializeField] private GameObject[] stTapBG = new GameObject[6]; // レーンタップ時の背景

    void Start()
    {
        //初期化
        score = 0;
        combo = 0;
        bestcombo = 0;

        for (int i = 0; i < totalGrades.Length; i++)
        {
            totalGrades[i] = 0;
        }

        for (int i = 0; i < stNotesCount.Length; i++)
        {
            stNotesCount[i] = 0;
        }

        //// 関数を呼ぶためにスクリプトを取得
        //uiObj = GameObject.Find("UICtrlCanvas");
        //mg1 = uiObj.GetComponent<ScoreManager>();
        //mg2 = uiObj.GetComponent<ComboManager>();
        //mg1.DrawScore(score);// デフォルトでスコア表示

        // タップ背景用 flag初期化
        for (int i = 0; i < tapFlag.Length; i++)
        {
            tapFlag[i] = false;
            lastTap[i] = false;
        }
    }

    void Update()
    {
        // tapFlag 全てfalse
        for (int i = 0; i < tapFlag.Length; i++)
        {
            tapFlag[i] = false;
        }

        // tapFlagON / OFF処理（マルチタップ対応）
        if (0 < Input.touchCount)
        {
            // タッチされている指の数だけ処理
            for (int i = 0; i < Input.touchCount; i++)
            {
                // タップしたレーンを取得
                laneNumber = Judge.GetLaneNumber(i);

                Debug.Log(laneNumber);

                if (laneNumber == -1)
                    continue;// 処理を抜ける

                tapFlag[laneNumber] = true;
            }
        }

        // 各レーンのタップ状況を前フレームと比較
        for (int i = 0; i < 5; i++)
        {
            // タップ継続
            if ((lastTap[i] == true) && (tapFlag[i] == true))
            {

            }
            // タップ開始
            else if ((lastTap[i] == false) && (tapFlag[i] == true))
            {
                // 判定ライン - ノーツで距離（絶対値）を算出
                absTiming = StGetAbsTiming(i);

                // 距離に応じて判定処理
                StJudgeGrade(absTiming, i);

                stTapBG[i].SetActive(true);
            }
            // タップ終了
            else if ((lastTap[i] == true) && (tapFlag[i] == false))
            {
                stTapBG[i].SetActive(false);
            }
        }
    }

    private float StGetAbsTiming(int i)// 判定ライン　－　ノーツ
    {
        float tempTiming = 9999;// 例外処理用

        // 縦レーン
        if ((stGOListArray[stNotesCount[i]][i] != null) && (i <= 3))
        {
            tempTiming = horizonJudgeLine.transform.position.y -
                         stGOListArray[stNotesCount[i]][i].transform.position.y;
        }
        // 横レーン
        else if ((stGOListArray[stNotesCount[i]][i] != null) && (i >= 4))
        {
            tempTiming = verticalJudgeLine.transform.position.x -
                         stGOListArray[stNotesCount[i]][i].transform.position.x;
        }

        return Mathf.Abs(tempTiming);// 絶対値に変換
    }

    /// <summary>
    /// 判定からノーツ破棄処理まで
    /// </summary>
    /// <param name="i">absTiming</param>
    /// <param name="j">laneNumber</param>
    private void StJudgeGrade(float i, int j)
    {
        // 判定分岐
        if (i <= perfect)
        {
            point = 300;
            combo++;
            totalGrades[0]++;
            SoundManager.SESoundCue(2);
        }
        else if (i <= great)
        {
            point = 200;
            combo++;
            totalGrades[1]++;
            SoundManager.SESoundCue(2);
        }
        else if (i <= good)
        {
            point = 100;
            combo = 0;
            totalGrades[2]++;
            SoundManager.SESoundCue(3);
        }
        else if (i <= bad)
        {
            point = 10;
            combo = 0;
            totalGrades[3]++;
            SoundManager.SESoundCue(4);
        }
        else// 空タップ
        {
            point = 0;
            SoundManager.SESoundCue(5);
        }

        if (combo > bestcombo)
        {
            bestcombo = combo;// 最大コンボ記憶
        }


        // スコア各倍率 switch文にできるかも
        if (combo > 250)
        {
            comboMag = 1.5f;
        }
        else if (combo > 150)
        {
            comboMag = 1.4f;
        }
        else if (combo > 100)
        {
            comboMag = 1.3f;
        }
        else if (combo > 50)
        {
            comboMag = 1.2f;
        }
        else
        {
            comboMag = 1.0f;
        }

        // 空タップでなければ
        if (point > 0)
        {
            float scores = point * comboMag;
            score += (int)scores;

            mg1.DrawScore(score);
            mg2.DrawCombo(combo);

            NotesDestroy(j);
        }
    }

    private static void NotesDestroy(int i)
    {
        Destroy(stGOListArray[stNotesCount[i]][i]);   // 該当ノーツ破棄
        stGOListArray[stNotesCount[i]][i] = null;     // 多重タップを防ぐ
        stNotesCount[i]++;                          // 該当レーンのノーツカウント++
    }
}