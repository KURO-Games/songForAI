using System;
using UnityEngine;

/// <summary>
/// 二重鍵盤演奏において、ノーツのタップによる判定を行う
/// </summary>
public class PianoNotesJudgement : NotesJudgementBase
{
    [SerializeField]
    private GameObject leftJudgeLine; // 左判定ライン

    [SerializeField]
    private GameObject rightJudgeLine; // 右判定ライン

    [SerializeField]
    private GameObject[] tapBG = new GameObject[8]; // レーンタップ時の背景

    [SerializeField]
    private GameObject[] mask = new GameObject[8]; // ロングノーツ用マスク

    protected override void Start()
    {
        maxLaneNum = 8;

        base.Start();

        for (int i = 0; i < tapBG.Length; i++)
        {
            tapBG[i].SetActive(false);
            mask[i].SetActive(false);
        }
    }

    protected override void EvaluateGrades(TimingGrade tapGrade, int laneNum)
    {
        switch (tapGrade)
        {
            case TimingGrade.Perfect:
                currentCombo++;
#if SFAI_SOUND
#else
                SoundManager.SESoundCue(2);
#endif

                break;

            case TimingGrade.Great:
                currentCombo++;
#if SFAI_SOUND
#else
                SoundManager.SESoundCue(2);
#endif

                break;

            case TimingGrade.Good:
                currentCombo = 0;
#if SFAI_SOUND
#else
                SoundManager.SESoundCue(3);
#endif

                break;

            case TimingGrade.Bad:
                currentCombo = 0;
#if SFAI_SOUND
#else
                SoundManager.SESoundCue(4);
#endif

                break;

            case TimingGrade.Miss:
                // ロングノーツ終点のミス判定
                if (isHold[laneNum])
                {
                    if (currentCombo > bestCombo)
                    {
                        bestCombo = currentCombo;
                    }

                    currentCombo = 0;
                    TotalGrades[4]++;
                    comboMgr.DrawCombo(currentCombo);
                    DestroyNotes(laneNum);
                }
                // 空タップ
                else
                {
#if SFAI_SOUND
#else
                    SoundManager.SESoundCue(5);
#endif
                }

                break;

            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    protected override void JudgeNotesType(int notesType, int laneNum)
    {
        // ロングノーツか判別
        if ((notesType       == 2) &&
            (isHold[laneNum] == false))
        {
            isHold[laneNum] = true; // ホールド開始
        }
        else
        {
            DestroyNotes(laneNum);
        }
    }

    protected override void UpdateNotesDisplay(bool[] tappedLane, bool[] lastTappedLane)
    {
        for (int laneNum = 0; laneNum < tappedLane.Length; laneNum++)
        {
            float absTiming            = 9999; // 初期化（0ではだめなので）
            bool  isTappedThisLane     = tappedLane[laneNum];
            bool  isTappedLastThisLane = lastTappedLane[laneNum];

            // タップを全く行っていなければ判定しない
            if (!isTappedThisLane && !isTappedLastThisLane) continue;

            // レーン内のノーツのインデックス
            // FIXME: レーン内の最終ノーツの場合、そのままノーツカウントを渡すとインデックス範囲外になるため、暫定的に-1している
            int laneNotesNum = (GOListArray[laneNum].Count == notesCount[laneNum])
                                   ? notesCount[laneNum] - 1
                                   : notesCount[laneNum];

            (GameObject notesObj, NotesSelector notesSel) = GOListArray[laneNum][laneNotesNum];

            switch (isTappedLastThisLane)
            {
                // タップ継続
                case true when isTappedThisLane:
                {
                    // ロングノーツホールド中、終点を通過した場合
                    if (isHold[laneNum])
                    {
                        // 左レーン
                        if (laneNum <= 3 &&
                            leftJudgeLine.transform.position.y - GradesCriterion[3] >
                            notesSel.EndNotes.transform.position.y)
                        {
                            NotesCountUp(laneNum);
                            isHold[laneNum] = false;
                        }
                        // 右レーン
                        else if (laneNum >= 4 &&
                                 rightJudgeLine.transform.position.y - GradesCriterion[3] >
                                 notesSel.EndNotes.transform.position.y)
                        {
                            NotesCountUp(laneNum);
                            isHold[laneNum] = false;
                        }
                    }

                    break;
                }

                // タップ開始
                case false when isTappedThisLane:
                {
                    if ((notesObj != null) && (laneNum <= 3))
                    {
                        absTiming = GetAbsTiming(notesObj.transform.position.y,
                                                 leftJudgeLine.transform.position.y);
                    }
                    else if ((notesObj != null) && (laneNum >= 4))
                    {
                        absTiming = GetAbsTiming(notesObj.transform.position.y,
                                                 rightJudgeLine.transform.position.y);
                    }

                    // 距離に応じて判定処理
                    JudgeGrade(absTiming, laneNum);

                    tapBG[laneNum].SetActive(true);

                    break;
                }

                // タップ終了
                case true when !isTappedThisLane:
                {
                    if (isHold[laneNum])
                    {
                        if ((notesObj != null) && (laneNum <= 3))
                        {
                            absTiming = GetAbsTiming(notesSel.EndNotes.transform.position.y,
                                                     leftJudgeLine.transform.position.y);
                        }
                        else if ((notesObj != null) && (laneNum >= 4))
                        {
                            absTiming = GetAbsTiming(notesSel.EndNotes.transform.position.y,
                                                     rightJudgeLine.transform.position.y);
                        }

                        JudgeGrade(absTiming, laneNum);

                        isHold[laneNum] = false;
                    }

                    tapBG[laneNum].SetActive(false);

                    break;
                }
            }

            mask[laneNum].SetActive(isHold[laneNum]);
        }
    }
}
