using System;
using System.Collections.Generic;
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

    [SerializeField, Header("スライド判定領域")]
    private GameObject slidableArea;

    [SerializeField, Header("スライドノーツホールド時用のマスク")]
    private GameObject slideNotesMask;

    private static bool             _isHoldSlideLane;                // スライドレーン全体をホールドしているか
    private static Vector3          _singleJudgeLinePos;             // シングルレーンの位置
    private static Vector3          _slideJudgeLinePos;              // スライドレーンの位置
    private static bool             _isNowSliding;                   // スライドレーンをスライドしているか
    private static bool             _isTouchedNotesWhileSlideInPrev; // 前フレーム、スライド判定領域でスライドノーツに触れていたか
    private static List<GameObject> _cachedSlidingNotes;             // 判定ライン通過後のスライドノーツ格納用

    private static int[] _cachedSlidingNotesCount; // 判定ラインを通過したノーツ数格納用
    // スライドのnotesCountをリアルタイムに反映するとバグるので、一旦記録してから一斉反映している

    protected override void Start()
    {
        base.Start();

        _singleJudgeLinePos      = singleJudgeLine.transform.position;
        _slideJudgeLinePos       = slideJudgeLine.transform.position;
        _cachedSlidingNotes      = new List<GameObject>();
        _cachedSlidingNotesCount = new int[maxLaneNum];
        slideNotesMask.SetActive(false);
    }

    protected override void EvaluateGrades(int laneNum, TimingGrade tapGrade)
    {
        switch (tapGrade)
        {
            case TimingGrade.Perfect:
                currentCombo++;
                SoundManager.SESoundCue(2);

                break;

            case TimingGrade.Great:
                currentCombo++;
                SoundManager.SESoundCue(2);

                break;

            case TimingGrade.Good:
                currentCombo = 0;
                SoundManager.SESoundCue(3);

                break;

            case TimingGrade.Bad:
                currentCombo = 0;
                SoundManager.SESoundCue(4);

                break;

            case TimingGrade.Miss:
                // スライドノーツ
                if (laneNum >= 4 && isHold[laneNum])
                {
                    if (currentCombo > bestCombo)
                    {
                        bestCombo = currentCombo;
                    }

                    NotesSelector notesSel     = GOListArray[laneNum][notesCount[laneNum]].selector;
                    NotesSelector nextNotesSel = notesSel.nextSlideNotes.selector;

                    // 次のスライドノーツが末尾ならそちらも同時に破棄
                    if (nextNotesSel != null && nextNotesSel.slideSection == SlideNotesSection.Foot)
                    {
                        TotalGrades[4]++;
                        SetSlideLaneHoldState(false);
                        DestroyNotes(nextNotesSel.laneNum);
                    }
                }
                // 空タップ
                else
                {
                    SoundManager.SESoundCue(5);
                }

                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(tapGrade), tapGrade, null);
        }
    }

    protected override void JudgeNotesType(int laneNum, NotesType notesType, SlideNotesSection? slideSection)
    {
        switch (notesType)
        {
            case NotesType.Single:
                DestroyNotes(laneNum);

                break;

            case NotesType.LongAndSlide:
                // スライドノーツのホールド開始なら全スライドレーンをホールド中に
                if (!isHold[laneNum] && slideSection != SlideNotesSection.Foot && slideSection != null)
                {
                    SetSlideLaneHoldState(true);
                }

                break;

            default:
                DestroyNotes(laneNum);

                break;
        }
    }

    protected override void UpdateNotesDisplay(bool[] tappedLane, bool[] lastTappedLane)
    {
        (bool isNowSliding, bool isTouchedNotesWhileSlide) = CheckRaycast();
        _isNowSliding                                      = isNowSliding;

        for (int laneNum = 0; laneNum < maxLaneNum; laneNum++)
        {
            float absTiming              = 9999;
            bool  isThisLaneTapped       = tappedLane[laneNum];
            bool  isThisLaneTappedInPrev = lastTappedLane[laneNum];
            bool  isSlideLane            = laneNum == 4 || laneNum == 5;

            // タップを全く行っていなければ判定しない
            if (!isThisLaneTapped       &&
                !isThisLaneTappedInPrev &&
                (!_isNowSliding || !isSlideLane)) continue;

            // レーン内のノーツのインデックス
            // OPTIMIZE: レーン内の最終ノーツの場合、そのままノーツカウントを渡すとインデックス範囲外になるため、暫定的に-1している
            int laneNotesCount = (GOListArray[laneNum].Count == notesCount[laneNum])
                                     ? notesCount[laneNum] - 1
                                     : notesCount[laneNum];

            // スライドノーツカウントの記録分を反映
            laneNotesCount += _cachedSlidingNotesCount[laneNum];

            (GameObject notesObj, NotesSelector notesSel) = GOListArray[laneNum][laneNotesCount];
            bool isSingleNotes = notesSel.notesType == NotesType.Single;
            bool isSlideNotes  = notesSel.notesType == NotesType.LongAndSlide;

            switch (isThisLaneTapped)
            {
                // タップ開始およびスライドレーン進入時
                case true when !isThisLaneTappedInPrev && !_isTouchedNotesWhileSlideInPrev:
                {
                    // TODO: レーンホールドでも先頭ノーツが判定されてしまうので、フラグで管理する

                    (GameObject prevNotesObj, NotesSelector prevNotesSel) = notesSel.prevSlideNotes;

                    // 判定処理
                    if (notesObj != null &&                                                      // 対象が存在
                        (prevNotesObj == null || prevNotesSel != null && prevNotesSel.isJudged)) // 1つ前のノーツが破棄または判定済み
                    {
                        Vector3 notesPos = notesObj.transform.position;

                        // タイミング判定
                        if (isSingleNotes)
                        {
                            absTiming = GetAbsTiming(notesPos.y, _singleJudgeLinePos.y);
                        }
                        else if (isSlideNotes)
                        {
                            absTiming = GetAbsTiming(notesPos.x, _slideJudgeLinePos.x);
                            TimingGrade grade = GetGradeFromAccuracy(absTiming);

                            _cachedSlidingNotes.Add(notesObj);
                            _cachedSlidingNotesCount[laneNum]++;

                            // 末尾ノーツかミスならそれまでの一連を削除
                            if (isHold[laneNum] &&
                                (notesSel.slideSection == SlideNotesSection.Foot || grade == TimingGrade.Miss))
                            {
                                (GameObject nextNotesObj, NotesSelector nextNotesSel) = notesSel.nextSlideNotes;

                                // 次のノーツが末尾ならそちらも破棄対象に（ミス時）
                                if (nextNotesSel != null && nextNotesSel.slideSection == SlideNotesSection.Foot)
                                {
                                    int nextLaneNum = nextNotesSel.laneNum;

                                    _cachedSlidingNotes.Add(nextNotesObj);
                                    _cachedSlidingNotesCount[nextLaneNum]++;

                                    JudgeGrade(nextLaneNum, absTiming);
                                }

                                SetSlideLaneHoldState(false);
                                DestroyCachedNotes();
                                AddCachedNotesCount();
                            }
                            // ミスじゃなければ判定済みとする
                            else
                            {
                                notesSel.isJudged = true;
                            }
                        }
                    }

                    // 判定ラインからの距離に応じて判定
                    JudgeGrade(laneNum, absTiming);

                    // レーンのタップエフェクトがあるなら表示処理をここへ

                    break;
                }

                // スライドレーンホールド（並行スライドノーツ。レーン進入時は含まない）
                case true when isThisLaneTappedInPrev || _isTouchedNotesWhileSlideInPrev:
                {
                    // FIXME: 水平スライドノーツの判定がバグってる

                    (GameObject prevNotesObj, NotesSelector prevNotesSel) = notesSel.prevSlideNotes;

                    // 1つ前に未処理ノーツが残っていなければ処理
                    if (prevNotesObj != null || (prevNotesSel != null && !prevNotesSel.isJudged)) break;

                    SlideNotesSection? notesSlideSection = notesSel.slideSection;

                    // 中間・末尾スライドノーツをホールドしているときのみ処理
                    if (!isHold[laneNum] || notesSlideSection == SlideNotesSection.Head) break;

                    // 判定ラインを超えているときのみ処理
                    if (!(_slideJudgeLinePos.x < notesObj.transform.position.x)) break;

                    _cachedSlidingNotes.Add(notesObj);
                    _cachedSlidingNotesCount[laneNum]++;

                    // 判定領域をスワイプしており、未判定ノーツなら
                    if ((isTouchedNotesWhileSlide || _isTouchedNotesWhileSlideInPrev) && !notesSel.isJudged)
                    {
                        // ノーツカウント
                        // NotesCountUp(laneNum, false);
                        JudgeGrade(laneNum, GradesCriterion[0]);
                        notesSel.isJudged = true;

                        // 末尾のときはホールド判定解除
                        if (notesSlideSection == SlideNotesSection.Foot)
                        {
                            SetSlideLaneHoldState(false);
                            DestroyCachedNotes();
                            AddCachedNotesCount();
                        }
                    }
                    // 判定領域外ならミス
                    else if ((!isTouchedNotesWhileSlide || !_isTouchedNotesWhileSlideInPrev) && !notesSel.isJudged)
                    {
                        (GameObject nextNotesObj, NotesSelector nextNotesSel) = notesSel.nextSlideNotes;

                        // 次のノーツが末尾ならそちらも破棄対象に
                        if (nextNotesSel.slideSection == SlideNotesSection.Foot)
                        {
                            int nextLaneNum = nextNotesSel.laneNum;

                            _cachedSlidingNotes.Add(nextNotesObj);
                            _cachedSlidingNotesCount[nextLaneNum]++;

                            JudgeGrade(nextLaneNum, 99);
                            DestroyCachedNotes();
                            AddCachedNotesCount();
                        }

                        JudgeGrade(laneNum, 99);
                        SetSlideLaneHoldState(false);
                    }

                    break;
                }

                // タップ領域を除く、スライド領域ホールド
                case false when _isNowSliding:
                    break;

                // タップ領域からスライドレーンにスライドしたとき
                case false when isThisLaneTappedInPrev:
                    break;

                // タップ終了
                case false when isThisLaneTappedInPrev && !_isNowSliding:
                    if (isHold[laneNum])
                    {
                        if (notesObj != null)
                        {
                            absTiming = GetAbsTiming(notesObj.transform.position.x, _slideJudgeLinePos.x);
                        }

                        JudgeGrade(laneNum, absTiming);
                        SetSlideLaneHoldState(false);
                    }

                    // レーンのタップエフェクトがあるなら非表示処理をここへ

                    break;
            }
        }

        _isTouchedNotesWhileSlideInPrev = isTouchedNotesWhileSlide;
    }

    protected override void DestroyNotes(int laneNum)
    {
        (GameObject notesObj, NotesSelector notesSel)         = GOListArray[laneNum][notesCount[laneNum]];
        (GameObject nextNotesObj, NotesSelector nextNotesSel) = notesSel.nextSlideNotes;

        Destroy(notesObj);
        notesCount[laneNum]++;

        // スライドノーツならホールド判定解除
        if (notesSel.slideSection != null)
        {
            SetSlideLaneHoldState(false);
        }

        // 次のスライドノーツが末尾ならそちらも同時に破棄
        if (nextNotesSel              != null &&
            nextNotesSel.slideSection == SlideNotesSection.Foot)
        {
            Destroy(nextNotesObj);
            notesCount[nextNotesSel.laneNum]++;
        }
    }

    /// <summary>
    /// スライドレーンのホールド状況を設定する
    /// </summary>
    /// <param name="flag"></param>
    private void SetSlideLaneHoldState(bool flag)
    {
        slideNotesMask.SetActive(flag);
        isHold[4] = flag;
        isHold[5] = flag;
    }

    /// <summary>
    /// スライド中に通過したノーツを一斉破棄する
    /// </summary>
    private static void DestroyCachedNotes()
    {
        foreach (GameObject notes in _cachedSlidingNotes)
        {
            Destroy(notes);
        }

        _cachedSlidingNotes.Clear();
    }

    /// <summary>
    /// スライド中に通過したノーツのカウントを一斉アップする
    /// </summary>
    private static void AddCachedNotesCount()
    {
        notesCount[4] += _cachedSlidingNotesCount[4];
        notesCount[5] += _cachedSlidingNotesCount[5];

        Array.Clear(_cachedSlidingNotesCount, 0, _cachedSlidingNotesCount.Length);
    }

    /// <summary>
    /// タッチしている場所にあるオブジェクトを確認し、スライド状況を返す
    /// </summary>
    /// <returns></returns>
    private static (bool isNowSliding, bool isTouchedNotesWhileSlide) CheckRaycast()
    {
        // 返り値用
        // bool isNowSliding;             // スライド領域をスライド中か
        bool isTouchedNotesWhileSlide; // スライド中にスライドノーツに触れたか

        // 判定用
        bool isTouchedLane         = false; // レーンに触れているか
        bool isTouchedSlidableArea = false; // スライド領域であるか
        bool isTouchedNotes        = false; // ノーツに触れているか

        if (tapRayHits == null) return (false, false);

        foreach (RaycastHit2D hit in tapRayHits)
        {
            Transform hitTrf = hit.transform;

            if (hitTrf == null) continue;

            if (hitTrf.CompareTag("Lane"))
            {
                isTouchedLane = true;
            }
            else if (hitTrf.CompareTag("SlidableArea"))
            {
                isTouchedSlidableArea = true;
            }
            else if (hitTrf.CompareTag("Notes"))
            {
                isTouchedNotes = true;
            }
        }

        // isNowSliding             = isTouchedLane && isTouchedSlidableArea;
        // isTouchedNotesWhileSlide = isNowSliding  && isTouchedNotes;
        isTouchedNotesWhileSlide = isTouchedLane && isTouchedSlidableArea && isTouchedNotes;

        return (isTouchedSlidableArea, isTouchedNotesWhileSlide);
    }
}
