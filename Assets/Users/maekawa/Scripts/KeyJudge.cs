using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyJudge : MonoBehaviour
{
    // タップ背景 ON/OFF 切り替え用
    private bool[] tapFlag = new bool[8];// 現在タップしているレーンの識別
    private bool[] lastTap = new bool[8];// 前フレームのタップ

    [SerializeField] private GameObject leftJudgeLine;  // 左判定ライン
    [SerializeField] private GameObject rightJudgeLine; // 右判定ライン
    [SerializeField] private GameObject[] TapBG = new GameObject[8]; // レーンタップ時の背景

    private void Start()
    {
        // タップ判定用 flag初期化
        for (int i = 0; i < tapFlag.Length; i++)
        {
            tapFlag[i] = false;
            lastTap[i] = false;
            TapBG[i].SetActive(false);
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
            // タップ継続
            if ((lastTap[i] == true) && (tapFlag[i] == true))
            {

            }
            // タップ開始
            else if ((lastTap[i] == false) && (tapFlag[i] == true))
            {
                float absTiming = 9999;// 初期化（0ではだめなので）

                // 左レーン
                if ((Judge.GOListArray[Judge.keyNotesCount[i]][i] != null) && (i <= 3))
                {
                    absTiming = Judge.GetAbsTiming(i, leftJudgeLine.transform.position.y);
                }
                // 右レーン
                else if ((Judge.GOListArray[Judge.keyNotesCount[i]][i] != null) && (i >= 4))
                {
                    absTiming = Judge.GetAbsTiming(i, rightJudgeLine.transform.position.y);
                }

                // 距離に応じて判定処理
                Judge.JudgeGrade(absTiming, i);

                TapBG[i].SetActive(true);
            }
            // タップ終了
            else if ((lastTap[i] == true) && (tapFlag[i] == false))
            {
                TapBG[i].SetActive(false);
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
}