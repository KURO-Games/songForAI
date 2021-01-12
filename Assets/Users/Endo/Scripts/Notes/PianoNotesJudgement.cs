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
    private GameObject[] laneBg = new GameObject[8]; // レーンタップ時の背景

    [SerializeField]
    private GameObject[] mask = new GameObject[8]; // ロングノーツ用マスク

    private static Vector3 _leftJudgeLinePos;  // 左レーンの位置
    private static Vector3 _rightJudgeLinePos; // 右レーンの位置

    protected override void Start()
    {
        base.Start();

        _leftJudgeLinePos  = leftJudgeLine.transform.position;
        _rightJudgeLinePos = rightJudgeLine.transform.position;

        for (int i = 0; i < maxLaneNum; i++)
        {
            laneBg[i].SetActive(false);
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

    protected override void JudgeNotesType(NotesType notesType, int laneNum)
    {
        // ロングノーツか判別
        if ((notesType       == NotesType.LongAndSlide) &&
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
        for (int laneNum = 0; laneNum < maxLaneNum; laneNum++)
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
            bool isNotesObjNull = notesObj == null;
            bool isLeftLane     = laneNum  <= 3;
            bool isRightLane    = laneNum  >= 4;

            switch (isTappedLastThisLane)
            {
                // タップ継続
                case true when isTappedThisLane:
                {
                    // ロングノーツホールド中、終点を通過した場合
                    if (isHold[laneNum])
                    {
                        Vector3 endNotesPos = notesSel.EndNotes.transform.position;

                        // 左レーン
                        if (isLeftLane &&
                            _leftJudgeLinePos.y - GradesCriterion[3] > endNotesPos.y)
                        {
                            NotesCountUp(laneNum);
                            isHold[laneNum] = false;
                        }
                        // 右レーン
                        else if (isRightLane &&
                                 _rightJudgeLinePos.y - GradesCriterion[3] > endNotesPos.y)
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
                    if (!isNotesObjNull)
                    {
                        Vector3 notesPos = notesObj.transform.position;

                        if (isLeftLane)
                        {
                            absTiming = GetAbsTiming(notesPos.y, _leftJudgeLinePos.y);
                        }
                        else if (isRightLane)
                        {
                            absTiming = GetAbsTiming(notesPos.y, _rightJudgeLinePos.y);
                        }
                    }


                    // 距離に応じて判定処理
                    JudgeGrade(absTiming, laneNum);

                    laneBg[laneNum].SetActive(true);

                    break;
                }

                // タップ終了
                case true when !isTappedThisLane:
                {
                    if (isHold[laneNum])
                    {
                        Vector3 endNotesPos = notesSel.EndNotes.transform.position;

                        if (!isNotesObjNull && isLeftLane)
                        {
                            absTiming = GetAbsTiming(endNotesPos.y, _leftJudgeLinePos.y);
                        }
                        else if (!isNotesObjNull && isRightLane)
                        {
                            absTiming = GetAbsTiming(endNotesPos.y, _rightJudgeLinePos.y);
                        }

                        JudgeGrade(absTiming, laneNum);

                        isHold[laneNum] = false;
                    }

                    laneBg[laneNum].SetActive(false);

                    break;
                }
            }

            mask[laneNum].SetActive(isHold[laneNum]);
        }
    }
}
