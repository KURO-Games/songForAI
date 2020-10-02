﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Judge : MonoBehaviour
{
    // プランナーレベルデザイン用
    // perfect ～ badの順に入力
    public static float[] gradesCriterion = { 1.0f, 1.5f, 2, 3 }; // 判定許容値
    public static int[] gradesPoint = { 300, 200, 100, 10 };      // 各判定に応じたスコア


    //public static List<List<GameObject>> GOListArray = new List<List<GameObject>>();// ノーツ座標格納用2次元配列
    //
    // 使い方  GOListArray   [_notesCount[laneNumber]]                   [laneNumber]
    //         GOListArray   [何個目のノーツなのか[何番目のレーンの]]    [何番目のレーンなのか]


    // リザルト用
    public static int totalScore;                        // 合計スコア
    public static int combo;                             // 現在のコンボ
    public static int bestCombo;                         // プレイヤー最大コンボ
    public static int[] totalGrades = new int[5];        // 判定内訳（perfect ～ miss）


    // 内部用
    public static int gameType;                          // key...0  string...1
    public static int point = 0;                         // 判定に応じた得点
    //public static int[] stNotesCount = new int[6];       // バイオリン用ノーツカウント
    static ScoreManager scoreMg;
    static ComboManager comboMg;

    static DrawGrade[] dg = new DrawGrade[8];
    public static GameObject[] drawGrade = new GameObject[8];

    void Start()
    {
        //初期化
        totalScore = 0;
        combo = 0;
        bestCombo = 0;

        for (int i = 0; i < totalGrades.Length; i++)
        {
            totalGrades[i] = 0;
        }

        // 関数を呼ぶためにスクリプトを取得
        GameObject uiObj = GameObject.Find("UICanvas");
        scoreMg = uiObj.GetComponent<ScoreManager>();
        comboMg = uiObj.GetComponent<ComboManager>();

        // 評価UI表示用のスクリプト配列をセット
        switch(gameType)
        {
            case 0:
                for (int i = 0; i < drawGrade.Length; i++)
                {
                    string callObject = "drawGrade" + i;

                    drawGrade[i] = GameObject.Find(callObject);

                    dg[i] = drawGrade[i].GetComponent<DrawGrade>();
                }
                break;

            case 1:
                for (int i = 0; i < 5; i++)
                {
                    string callObject = "drawGrade" + i;

                    drawGrade[i] = GameObject.Find(callObject);

                    dg[i] = drawGrade[i].GetComponent<DrawGrade>();
                }
                break;

                // 使い方
                // dg[laneNumber].DrawGrades(grade(0～5));
        }
    }

    /// <summary>
    /// タップした場所に応じてレーン番号を返します
    /// </summary>
    /// <param name="i">GetTouch</param>
    /// <returns>laneNumber</returns>
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
    /// 判定ライン - ノーツ座標でタップしたタイミングの正確さを返します
    /// </summary>
    /// <param name="i">laneNumber</param>
    /// <param name="j">judgeLine</param>
    /// <returns>absTiming</returns>
    public static float GetAbsTiming(float i, float j)// 判定ライン　－　ノーツ
    {
        float tempTiming = i - j;

        return Mathf.Abs(tempTiming);// 絶対値に変換
    }

    /// <summary>
    /// 判定からノーツ破棄処理まで
    /// </summary>
    /// <param name="i">absTiming</param>
    /// <param name="j">laneNumber</param>
    public static void JudgeGrade(float i, int j)
    {
        // 判定分岐 perfect ～ bad
        if (i <= gradesCriterion[0])
        {
            point = gradesPoint[0];
            combo++;
            totalGrades[0]++;

            dg[j].DrawGrades(0);
            SoundManager.SESoundCue(2);
        }
        else if (i <= gradesCriterion[1])
        {
            point = gradesPoint[1];
            combo++;
            totalGrades[1]++;

            dg[j].DrawGrades(1);
            SoundManager.SESoundCue(2);
        }
        else if (i <= gradesCriterion[2])
        {
            point = gradesPoint[2];
            combo = 0;
            totalGrades[2]++;

            dg[j].DrawGrades(2);
            SoundManager.SESoundCue(3);
        }
        else if (i <= gradesCriterion[3])
        {
            point = gradesPoint[3];
            combo = 0;
            totalGrades[3]++;

            dg[j].DrawGrades(4);
            SoundManager.SESoundCue(4);
        }
        else// 空タップ
        {
            if(KeyJudge.isHold[j] == true)
            {
                point = 0;
                combo = 0;
                totalGrades[4]++;
                comboMg.DrawCombo(combo);

                if (combo > bestCombo)
                {
                    bestCombo = combo;// 最大コンボ記憶
                }

                NotesDestroy(j);
            }
            else
            {
                point = 0;
                SoundManager.SESoundCue(5);
            }
        }

        if (combo > bestCombo)
        {
            bestCombo = combo;// 最大コンボ記憶
        }

        // 空タップでなければ
        if (point > 0)
        {
            // タップノーツかロングノーツかを判別
            totalScore += point;

            scoreMg.DrawScore(totalScore);
            comboMg.DrawCombo(combo);

            if(gameType == 0)
            {
                // ロングノーツか判別
                KeyJudge.notesType = KeyJudge.GOListArray[KeyJudge.keyNotesCount[j]][j].GetComponent<NotesSelector>().NotesType;

                if (KeyJudge.notesType == 2)
                {
                    KeyJudge.isHold[j] = true;
                }
            }
            else
            {
                NotesDestroy(j);
            }
        }
    }

    /// <summary>
    /// ノーツ破棄、配列カウントアップ
    /// </summary>
    /// <param name="i">laneNumber</param>
    public static void NotesDestroy(int i)
    {
        switch (gameType)
        {
            case 0:
                Destroy(KeyJudge.GOListArray[KeyJudge.keyNotesCount[i]][i]);   // 該当ノーツ破棄
                KeyJudge.GOListArray[KeyJudge.keyNotesCount[i]][i] = null;     // 多重タップを防ぐ
                KeyJudge.keyNotesCount[i]++;                          // 該当レーンのノーツカウント++
                break;

            //case 1:
            //    Destroy(StringJudge.GOListArray[stNotesCount[i]][i]);
            //    KeyJudge.GOListArray[stNotesCount[i]][i] = null;
            //    stNotesCount[i]++;
            //    break;

            default:
                break;
        }
    }

    /// <summary>
    /// ノーツがスルーされた時の処理です
    /// </summary>
    /// <param name="i">laneNumber</param>
    public static void NotesCountUp(int i)
    {
        if (combo > bestCombo)
        {
            bestCombo = combo;// 最大コンボ記憶
        }

        combo = 0;
        totalGrades[4]++;

        comboMg.DrawCombo(combo);

        dg[i].DrawGrades(4);

        NotesDestroy(i);
    }
}
