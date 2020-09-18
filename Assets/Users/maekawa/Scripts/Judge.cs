using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge : MonoBehaviour
{
    // 判定基準値 シリアライズできない
    static float[] gradeCriterion = { 5, 10, 15, 20 };

    public static int score = 0;                                         // スコア
    public static int combo = 0;                                         // 現在のコンボ
    public static int bestcombo = 0;                                     // リザルト用　最大コンボ
    public static int[] totalGrades = { 0, 0, 0, 0, 0 };                 // リザルト用　判定内訳（perfect ～ miss）
    public static int point = 0;                                         // 判定に応じた得点
    public static float comboMag = 1.0f;                                 // コンボに応じたスコア倍率
    public static int[] keyNotesCount = new int[8];
    public static int[] stNotesCount = new int[6];

    public static List<List<GameObject>> GOListArray = new List<List<GameObject>>();// ノーツ座標格納用2次元配列
    //
    // 使い方  GOListArray   [_notesCount[laneNumber]]                   [laneNumber]
    //         GOListArray   [何個目のノーツなのか[何番目のレーンの]]    [何番目のレーンなのか]

    static ScoreManager mg1;
    static ComboManager mg2;

    void Start()
    {
        //初期化
        score = 0;
        combo = 0;
        bestcombo = 0;

        for (int i = 0; i < keyNotesCount.Length; i++)
        {
            keyNotesCount[i] = 0;
        }

        for(int i = 0; i < stNotesCount.Length; i++)
        {
            stNotesCount[i] = 0;
        }

        // 関数を呼ぶためにスクリプトを取得
        GameObject uiObj = GameObject.Find("UICtrlCanvas");
        mg1 = uiObj.GetComponent<ScoreManager>();
        mg2 = uiObj.GetComponent<ComboManager>();
        mg1.DrawScore(score);// デフォルトでスコア表示

        for (int i = 0; i < totalGrades.Length; i++)
        {
            totalGrades[i] = 0;
        }
    }

    public static void ListImport()
    {
        GOListArray = NotesManager.NotesPositions;
        Debug.Log(GOListArray[0][7]);
    }

    /// <summary>
    /// タップした場所に応じてレーン番号を取得します
    /// </summary>
    /// <param name="i">GetTouch</param>
    /// <returns></returns>
    public static int GetLaneNumber(int i)
    {
        int laneNum = -1;// 例外処理用
        GameObject clickObj = null; // 都度初期化

        // タッチ情報を取得
        Touch t = Input.GetTouch(i);

        // タップ時処理
        Ray ray = Camera.main.ScreenPointToRay(t.position);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10f, 1);

        if (hit)
        {
            clickObj = hit.transform.gameObject;

            if ((clickObj != null) && (clickObj.tag == ("Lane")))// tagでレーンを識別
            {
                string s = clickObj.name;  // レーン番号を取得
                laneNum = int.Parse(s);    // 文字列を数字に変換
            }
        }

        return laneNum;
    }

    /// <summary>
    /// 判定ライン - ノーツ座標でタップしたタイミングの正確さを求めます
    /// </summary>
    /// <param name="i">laneNumber</param>
    /// <param name="j">gameType</param>
    /// <param name="g">right, leftJudgeLine</param>
    /// <returns></returns>
    public static float GetAbsTiming(int i, int j, float f)// 判定ライン　－　ノーツ
    {
        float tempTiming = 9999;// 初期化（0ではだめなので）

        switch (j)
        {
            // 二重鍵盤
            case 0:
                tempTiming = f - GOListArray[keyNotesCount[i]][i].transform.position.y;
                break;
            // バイオリン縦レーン
            case 1:
                tempTiming = f - GOListArray[stNotesCount[i]][i].transform.position.y;
                break;
            // バイオリン横レーン
            case 2:
                tempTiming = f - GOListArray[stNotesCount[i]][i].transform.position.x;
                break;
         
            default:
                break;
        }

        return Mathf.Abs(tempTiming);// 絶対値に変換
    }
    /// <summary>
    /// 判定からノーツ破棄処理まで
    /// </summary>
    /// <param name="i">absTiming</param>
    /// <param name="j">laneNumber</param>
    /// <param name="k">gameType</param>
    public static void JudgeGrade(float i, int j, int k)
    {
        // 判定分岐
        if (i <= gradeCriterion[0])
        {
            point = 300;
            combo++;
            totalGrades[0]++;
            SoundManager.SESoundCue(2);
        }
        else if (i <= gradeCriterion[1])
        {
            point = 200;
            combo++;
            totalGrades[1]++;
            SoundManager.SESoundCue(2);
        }
        else if (i <= gradeCriterion[2])
        {
            point = 100;
            combo = 0;
            totalGrades[2]++;
            SoundManager.SESoundCue(3);
        }
        else if (i <= gradeCriterion[3])
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

            NotesDestroy(j, k);
        }
    }

    /// <summary>
    /// ノーツ破棄、配列カウントアップ
    /// </summary>
    /// <param name="i">laneNumber</param>
    /// <param name="j">gameType</param>
    public static void NotesDestroy(int i, int j)
    {
        switch (j)
        {
            case 0:
                Destroy(GOListArray[keyNotesCount[i]][i]);   // 該当ノーツ破棄
                GOListArray[keyNotesCount[i]][i] = null;     // 多重タップを防ぐ
                keyNotesCount[i]++;                          // 該当レーンのノーツカウント++
                break;

            case 1:
                Destroy(GOListArray[stNotesCount[i]][i]);   // 該当ノーツ破棄
                GOListArray[stNotesCount[i]][i] = null;     // 多重タップを防ぐ
                stNotesCount[i]++;                          // 該当レーンのノーツカウント++
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// ノーツがスルーされた時の処理です
    /// </summary>
    /// <param name="i">laneNumber</param>
    /// <param name="j">gameType</param>
    public static void NotesCountUp(string i, int j)
    {
        if (combo > bestcombo)
        {
            bestcombo = combo;// 最大コンボ記憶
        }

        combo = 0;
        totalGrades[4]++;

        int tempLaneNum = int.Parse(i);// 文字列を数字に変換

        NotesDestroy(tempLaneNum, j);

        // コンボ描画処理はNotesCountUpスクリプトで行う
    }
}
