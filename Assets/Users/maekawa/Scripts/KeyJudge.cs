using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyJudge : MonoBehaviour
{
    // タップ背景 ON/OFF 切り替え用
    private bool[] tapFlag = new bool[8];// 現在タップしているレーンの識別
    private bool[] lastTap = new bool[8];// 前フレームのタップ
    public static bool[] isHold = new bool[8];// 二重鍵盤用ロングノーツ識別
    public static int notesType;
    public static int[] keyNotesCount = new int[8];      // 二重鍵盤用ノーツカウント
    public static List<List<GameObject>> GOListArray = new List<List<GameObject>>();// ノーツ座標格納用2次元配列
    //
    // 使い方  GOListArray   [_notesCount[laneNumber]]                   [laneNumber]
    //         GOListArray   [何個目のノーツなのか[何番目のレーンの]]    [何番目のレーンなのか]

    [SerializeField] private GameObject leftJudgeLine;  // 左判定ライン
    [SerializeField] private GameObject rightJudgeLine; // 右判定ライン
    [SerializeField] private GameObject[] tapBG = new GameObject[8]; // レーンタップ時の背景

    private void Start()
    {
        Judge.gameType = 0;// 二十鍵盤仕様
        notesType = 0;

        // タップ判定用 flag初期化
        for (int i = 0; i < tapFlag.Length; i++)
        {
            tapFlag[i] = false;
            lastTap[i] = false;
            isHold[i] = false;
            tapBG[i].SetActive(false);
        }

        for (int i = 0; i < keyNotesCount.Length; i++)
        {
            keyNotesCount[i] = 0;
        }
    }

    void Update()
    {
        // tapFlag 全てfalse
        for (int i = 0; i < tapFlag.Length; i++)
        {
            tapFlag[i] = false;
        }

        // tapFlagON/OFF処理（マルチタップ対応）
        if (0 < Input.touchCount)
        {
            // タッチされている指の数だけ処理
            for (int i = 0; i < Input.touchCount; i++)
            {
                // タップしたレーンを取得
                int laneNumber = Judge.GetLaneNumber(i);

                if (laneNumber == -1)
                    continue;// 処理を抜ける

                tapFlag[laneNumber] = true;
            }
        }

        // 各レーンのタップ状況を前フレームと比較
        for(int i = 0; i < tapFlag.Length; i++)
        {
            float absTiming = 9999;// 初期化（0ではだめなので）

            // タップ継続
            if ((lastTap[i] == true) && (tapFlag[i] == true))
            {
                // ロングノーツホールド中、終点を通過した場合
                if(isHold[i] == true)
                {
                    // 左レーン
                    if (i <= 3 && leftJudgeLine.transform.position.y - Judge.gradesCriterion[3] > GOListArray[keyNotesCount[i]][i].GetComponent<NotesSelector>().EndNotes.transform.position.y)
                    {
                        Judge.NotesDestroy(i);
                    }
                    // 右レーン
                    else if (i >= 4 && rightJudgeLine.transform.position.y - Judge.gradesCriterion[3] > GOListArray[keyNotesCount[i]][i].GetComponent<NotesSelector>().EndNotes.transform.position.y)
                    {
                        Judge.NotesDestroy(i);
                    }
                }
            }
            // タップ開始
            else if ((lastTap[i] == false) && (tapFlag[i] == true))
            {
                if ((GOListArray[keyNotesCount[i]][i] != null) && (i <= 3))
                {
                    absTiming = Judge.GetAbsTiming(GOListArray[keyNotesCount[i]][i].transform.position.y
                                , leftJudgeLine.transform.position.y);
                }
                else if ((GOListArray[keyNotesCount[i]][i] != null) && (i >= 4))
                {
                    absTiming = Judge.GetAbsTiming(GOListArray[keyNotesCount[i]][i].transform.position.y
                                , rightJudgeLine.transform.position.y);
                }

                // 距離に応じて判定処理
                Judge.JudgeGrade(absTiming, i);

                tapBG[i].SetActive(true);
            }
            // タップ終了
            else if ((lastTap[i] == true) && (tapFlag[i] == false))
            {
                // ホールド中なら
                if(isHold[i])
                {
                    if ((GOListArray[keyNotesCount[i]][i] != null) && (i <= 3))
                    {
                        absTiming = Judge.GetAbsTiming(GOListArray[keyNotesCount[i]][i].GetComponent<NotesSelector>().EndNotes.transform.position.y
                                    , leftJudgeLine.transform.position.y);
                    }
                    else if ((GOListArray[keyNotesCount[i]][i] != null) && (i >= 4))
                    {
                        absTiming = Judge.GetAbsTiming(GOListArray[keyNotesCount[i]][i].GetComponent<NotesSelector>().EndNotes.transform.position.y
                                    , rightJudgeLine.transform.position.y);
                    }

                    Judge.JudgeGrade(absTiming, i);

                    isHold[i] = false;
                }

                tapBG[i].SetActive(false);
            }
        }
    }

    private void LateUpdate()
    {
        for(int i = 0; i < lastTap.Length; i++)
        {
            lastTap[i] = tapFlag[i];// 次フレームで比較するためタップ状況を保存
        }
    }

    public static void ListImport()
    {
        GOListArray = NotesManager.NotesPositions;
        Debug.Log(GOListArray[0][7]);
    }
}