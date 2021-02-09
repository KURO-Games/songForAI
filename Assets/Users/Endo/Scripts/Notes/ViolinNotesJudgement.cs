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

    [SerializeField]
    private ParticleSystem slideParticle;

    private static bool    _isHoldSlideLane;                // スライドレーン全体をホールドしているか
    private static Vector3 _singleJudgeLinePos;             // シングルレーンの位置
    private static Vector3 _slideJudgeLinePos;              // スライドレーンの位置
    private static bool    _isNowSliding;                   // スライドレーンをスライドしているか
    private static bool    _isSlidingInPrev;                // 前フレーム、スライドレーンをスライドしていたか
    private static bool    _isTouchedNotesWhileSlideInPrev; // 前フレーム、スライド判定領域でスライドノーツに触れていたか

    // 判定ラインを通過したノーツ数格納用
    // スライドのnotesCountをリアルタイムに反映するとバグるので、一旦記録してから一斉反映している
    private static int[]            _cachedSlidingNotesCount;
    private static bool             _isCached;
    private static List<GameObject> _cachedSlidingNotes; // 判定ライン通過後のスライドノーツ格納用

    protected override void Start()
    {
        base.Start();

        _isHoldSlideLane                = false;
        _singleJudgeLinePos             = singleJudgeLine.transform.position;
        _slideJudgeLinePos              = slideJudgeLine.transform.position;
        _isNowSliding                   = false;
        _isSlidingInPrev                = false;
        _isTouchedNotesWhileSlideInPrev = false;
        _cachedSlidingNotes             = new List<GameObject>();
        _cachedSlidingNotesCount        = new int[maxLaneNum];
        _isCached                       = false;
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
                    // ベストコンボ数更新
                    if (currentCombo > bestCombo)
                    {
                        bestCombo = currentCombo;
                    }

                    (GameObject notesObj, NotesSelector notesSel) = GOListArray[laneNum][notesCount[laneNum]];
                    NotesSelector nextNotesSel = notesSel.nextSlideNotes.selector;

                    // 次のスライドノーツが末尾ならそちらも破棄対象に
                    if (nextNotesSel != null && nextNotesSel.slideSection == SlideNotesSection.Foot)
                    {
                        CacheNotesCount(laneNum, notesObj);
                        JudgeGrade(laneNum, 99);
                    }

                    SetSlideLaneHoldState(false);
                    AddCachedNotesCount();
                    DestroyCachedNotes();
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
                if (!isHold[laneNum]                       && // スライドレーン未ホールド
                    slideSection != SlideNotesSection.Foot && // 末尾ではない
                    !slideNotesMask.activeSelf)               // マスクが有効ではない
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
        // FIXME: スライドノーツの通過判定中にミスすると、一部破棄されないノーツがあり、またいくつか先のノーツが破棄されることがある

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
                    bool isDestroyed = false;

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
                            // レーンとノーツに触れているときのみ判定
                            if (!isTouchedNotesWhileSlide) break;

                            absTiming = 0;

                            CacheNotesCount(laneNum, notesObj);

                            // 末尾ノーツならそれまでの一連を削除
                            if (isHold[laneNum] && notesSel.slideSection == SlideNotesSection.Foot)
                            {
                                if (_isCached)
                                {
                                    isDestroyed = true;

                                    AddCachedNotesCount();
                                    DestroyCachedNotes();
                                }
                            }
                            // ミスじゃなければ判定済みとする
                            else
                            {
                                notesSel.isJudged = true;
                            }
                        }

                        // 判定ラインからの距離に応じて判定
                        JudgeGrade(laneNum, absTiming);

                        // ↑のノーツ破棄時にホールド状態を切り替えてもJudgeGrade→JudgeNotesTypeでホールド状態が戻るので、ここでfalseに
                        if (isDestroyed) SetSlideLaneHoldState(false);
                    }

                    // レーンのタップエフェクトがあるなら表示処理をここへ

                    break;
                }

                // スライドレーンホールド（並行スライドノーツ。レーン進入時は含まない）
                case true when isThisLaneTappedInPrev || _isTouchedNotesWhileSlideInPrev:
                {
                    (GameObject prevNotesObj, NotesSelector prevNotesSel) = notesSel.prevSlideNotes;

                    // 1つ前に未処理ノーツが残っていなければ処理
                    if (prevNotesObj == null || (prevNotesSel != null && !prevNotesSel.isJudged)) break;

                    SlideNotesSection? notesSlideSection = notesSel.slideSection;

                    // 中間・末尾スライドノーツをホールドしているときのみ処理
                    if (!isHold[laneNum] || notesSlideSection == SlideNotesSection.Head) break;

                    // 判定ラインを超えているときのみ処理
                    if (!(_slideJudgeLinePos.x < notesObj.transform.position.x)) break;

                    CacheNotesCount(laneNum, notesObj);

                    // 判定領域をスワイプしており、未判定ノーツなら
                    if ((isTouchedNotesWhileSlide || _isTouchedNotesWhileSlideInPrev) && !notesSel.isJudged)
                    {
                        JudgeGrade(laneNum, GradesCriterion[0]);
                        notesSel.isJudged = true;

                        // 末尾のときはホールド判定解除
                        if (notesSlideSection == SlideNotesSection.Foot)
                        {
                            SetSlideLaneHoldState(false);
                            AddCachedNotesCount();
                            DestroyCachedNotes();
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

                            CacheNotesCount(nextLaneNum, nextNotesObj);

                            JudgeGrade(nextLaneNum, 99);
                            AddCachedNotesCount();
                            DestroyCachedNotes();
                        }

                        JudgeGrade(laneNum, 99);
                        SetSlideLaneHoldState(false);
                    }

                    break;
                }

                // タップ領域を除く、スライド領域ホールド
                case false when _isNowSliding:
                    break;

                // タップ領域から領域外にスライドしたとき
                case false when isThisLaneTappedInPrev:
                {
                    // 主にミス判定
                    (GameObject nextNotesObj, NotesSelector nextNotesSel) = notesSel.nextSlideNotes;

                    // 末尾ノーツの直前でミスした場合、末尾を破棄予定に
                    if (isHold[laneNum] && nextNotesSel != null && nextNotesSel.slideSection == SlideNotesSection.Foot)
                    {
                        CacheNotesCount(nextNotesSel.laneNum, nextNotesObj);
                    }

                    // 破棄予定ノーツがあれば破棄
                    if (_isCached)
                    {
                        AddCachedNotesCount();
                        DestroyCachedNotes();
                        SetSlideLaneHoldState(false);
                    }

                    break;
                }

                case false when !_isNowSliding && _isSlidingInPrev:
                    Debug.Log("test");

                    break;

                // タップ終了
                case false when isThisLaneTappedInPrev && !_isNowSliding:
                    // 現状、通過ノーツに触れたらとなっているため機能してない
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

        // スライド中はエフェクトの位置を指に追従させる
        if (slideParticle.isPlaying)
        {
            slideParticle.transform.position = tappedSlideLanePos;
        }

        _isSlidingInPrev                = _isNowSliding;
        _isTouchedNotesWhileSlideInPrev = isTouchedNotesWhileSlide;
    }

    public override void NotesCountUp(int laneNum, bool doDestroy = true, bool isLongStart = false)
    {
        if (currentCombo > bestCombo)
        {
            bestCombo = currentCombo; // 最大コンボ記憶
        }

        currentCombo = 0;
        TotalGrades[4]++;

        comboMgr.DrawCombo(currentCombo);

        DrawGrades[laneNum].DrawGrades(4);

        if (doDestroy)
        {
            DestroyNotes(laneNum);
        }
        // 破棄フラグがなければスライドノーツだとみなし、破棄をせずカウントのみ行う
        else
        {
            notesCount[laneNum]++;
            TotalJudgedNotesCount++;
        }
    }

    protected override void DestroyNotes(int laneNum, bool isLongStart = false)
    {
        (GameObject notesObj, NotesSelector notesSel)         = GOListArray[laneNum][notesCount[laneNum]];
        (GameObject nextNotesObj, NotesSelector nextNotesSel) = notesSel.nextSlideNotes;

        Destroy(notesObj);
        notesCount[laneNum]++;
        TotalJudgedNotesCount++;

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
            TotalJudgedNotesCount++;
        }
    }

    /// <summary>
    /// スライドレーンのホールド状況を設定する
    /// </summary>
    /// <param name="flag"></param>
    private void SetSlideLaneHoldState(bool flag)
    {
        if (flag)
        {
            slideParticle.Play();
        }
        else
        {
            slideParticle.Stop();
        }

        slideNotesMask.SetActive(flag);
        isHold[4] = flag;
        isHold[5] = flag;
    }

    /// <summary>
    /// スライド中に通過したノーツおよびそのレーンのカウントを一時キャッシュする
    /// </summary>
    /// <param name="laneNum">レーン番号</param>
    /// <param name="notesObj">対象ノーツ</param>
    private static void CacheNotesCount(int laneNum, GameObject notesObj)
    {
        _cachedSlidingNotes.Add(notesObj);
        _cachedSlidingNotesCount[laneNum]++;
        _isCached = true;
    }

    /// <summary>
    /// スライド中に通過したノーツのカウントを一斉アップする
    /// </summary>
    private static void AddCachedNotesCount()
    {
        notesCount[4]         += _cachedSlidingNotesCount[4];
        notesCount[5]         += _cachedSlidingNotesCount[5];
        TotalJudgedNotesCount += _cachedSlidingNotesCount[4] + _cachedSlidingNotesCount[5];
        _isCached             =  false;

        Array.Clear(_cachedSlidingNotesCount, 0, _cachedSlidingNotesCount.Length);
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
