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
        for (int i = 0; i < tappedLane.Length; i++)
        {
            float absTiming = 9999; // 初期化（0ではだめなので）

            switch (lastTappedLane[i])
            {
                // タップ継続
                case true when tappedLane[i]:
                {
                    // ロングノーツホールド中、終点を通過した場合
                    if (isHold[i])
                    {
                        // 左レーン
                        if (i <= 3 &&
                            leftJudgeLine.transform.position.y - GradesCriterion[3] >
                            GOListArray[notesCount[i]][i]
                                .GetComponent<NotesSelector>()
                                .EndNotes.transform.position.y)
                        {
                            NotesCountUp(i);
                            isHold[i] = false;
                        }
                        // 右レーン
                        else if (i >= 4 &&
                                 rightJudgeLine.transform.position.y - GradesCriterion[3] >
                                 GOListArray[notesCount[i]][i]
                                     .GetComponent<NotesSelector>()
                                     .EndNotes.transform.position.y)
                        {
                            NotesCountUp(i);
                            isHold[i] = false;
                        }
                    }

                    break;
                }

                // タップ開始
                case false when tappedLane[i]:
                {
                    if ((GOListArray[notesCount[i]][i] != null) && (i <= 3))
                    {
                        absTiming = GetAbsTiming(GOListArray[notesCount[i]][i].transform.position.y
                                                 , leftJudgeLine.transform.position.y);
                    }
                    else if ((GOListArray[notesCount[i]][i] != null) && (i >= 4))
                    {
                        absTiming = GetAbsTiming(GOListArray[notesCount[i]][i].transform.position.y
                                                 , rightJudgeLine.transform.position.y);
                    }

                    // 距離に応じて判定処理
                    JudgeGrade(absTiming, i);

                    tapBG[i].SetActive(true);

                    break;
                }

                // タップ終了
                case true when !tappedLane[i]:
                {
                    if (isHold[i])
                    {
                        if ((GOListArray[notesCount[i]][i] != null) && (i <= 3))
                        {
                            absTiming = GetAbsTiming(GOListArray[notesCount[i]][i]
                                                     .GetComponent<NotesSelector>()
                                                     .EndNotes.transform.position.y
                                                     , leftJudgeLine.transform.position.y);
                        }
                        else if ((GOListArray[notesCount[i]][i] != null) && (i >= 4))
                        {
                            absTiming = GetAbsTiming(GOListArray[notesCount[i]][i]
                                                     .GetComponent<NotesSelector>()
                                                     .EndNotes.transform.position.y
                                                     , rightJudgeLine.transform.position.y);
                        }

                        JudgeGrade(absTiming, i);

                        isHold[i] = false;
                    }

                    tapBG[i].SetActive(false);

                    break;
                }
            }

            mask[i].SetActive(isHold[i]);
        }
    }
}
