using System;
using UnityEngine;

/// <summary>
/// 弦楽器演奏において、ノーツのタップによる判定を行う
/// </summary>
public class ViolinNotesJudgement : NotesJudgementBase
{
    [SerializeField, Header("シングルノーツの判定ライン")]
    private GameObject singleJudgeLine;

    [SerializeField, Header("スライドノーツの判定ライン")]
    private GameObject slideJudgeLine;

    private static bool    _isHoldSlideLane;    // スライドレーン全体をホールドしているか
    private static Vector3 _singleJudgeLinePos; // シングルレーンの位置
    private static Vector3 _slideJudgeLinePos;  // スライドレーンの位置

    protected override void Start()
    {
        base.Start();

        _singleJudgeLinePos = singleJudgeLine.transform.position;
        _slideJudgeLinePos  = slideJudgeLine.transform.position;
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
                // TODO: スライドノーツミス判定
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(tapGrade), tapGrade, null);
        }
    }

    protected override void JudgeNotesType(NotesType notesType, int laneNum)
    {
        switch (notesType)
        {
            case NotesType.Single:
                DestroyNotes(laneNum);

                break;

            // TODO: スライドノーツのホールド判定
            case NotesType.LongAndSlide:
                break;

            default:
                DestroyNotes(laneNum);

                break;
        }
    }

    protected override void UpdateNotesDisplay(bool[] tappedLane, bool[] lastTappedLane)
    {
        for (int laneNum = 0; laneNum < maxLaneNum; laneNum++)
        {
            float absTiming            = 9999;
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
            bool isSingleNotes  = laneNum  <= 3;
            bool isSlideNotes   = laneNum  >= 4;

            switch (isTappedLastThisLane)
            {
                // タップ継続
                case true when isTappedThisLane:
                    break;

                // タップ開始
                case false when isTappedThisLane:
                {
                    if (!isNotesObjNull)
                    {
                        Vector3 notesPos = notesObj.transform.position;

                        /* タイミング判定 */
                        // シングルノーツ
                        if (isSingleNotes)
                        {
                            absTiming = GetAbsTiming(notesPos.y, _singleJudgeLinePos.y);
                        }
                        // スライドノーツ
                        else if (isSlideNotes)
                        {
                            absTiming = GetAbsTiming(notesPos.x, _slideJudgeLinePos.x);
                        }
                    }

                    // 判定ラインからの距離に応じて判定
                    JudgeGrade(absTiming, laneNum);

                    // レーンのタップエフェクトがあるなら表示処理をここへ

                    break;
                }

                // タップ終了
                case true when !isTappedThisLane:
                    // レーンのタップエフェクトがあるなら非表示処理をここへ

                    break;
            }
        }
    }
}
