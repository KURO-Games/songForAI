using System;
using System.Collections.Generic;
using UnityEngine;

public class ViolinNotesGenerator : NotesGeneratorBase
{
    [SerializeField, Header("スライドノーツの両端")]
    private GameObject slideNotes;

    [SerializeField, Header("スライドノーツの帯部分")]
    private GameObject slideNotesBody;

    // 垂直方向のノーツレーン
    [SerializeField, Header("垂直方向のノーツレーン")]
    private Transform vLaneTrf;

    // 水平方向のノーツレーン
    [SerializeField, Header("水平方向のノーツレーン")]
    private Transform hLaneTrf;

    // 水平方向への移動ベクトル
    private Vector3 _hMove;

    protected override void CalculateNotesPositions()
    {
        // ノーツ生成およびジャケット表示が済んだら座標計算開始
        if (!Generated || !jacketIsFaded) return;

        SoundManager.BgmTime(ref BgmTimes);

        NotesSpeeds = BgmTimes / 6.12f;
        move        = new Vector3(0, NotesSpeeds * speed); //曲の再生ポジションでとっている値
        _hMove      = new Vector3(NotesSpeeds    * speed, 0);

        if (PlayedBGM) return;

        // ノーツを遅らせる
        SoundManager.BGMSoundCue(MusicDatas.cueMusic);
        PlayedBGM = true;
    }

    protected override void MoveNotes()
    {
        vLaneTrf.position = move   * (-1 * NotesSpeed) + new Vector3(0, offset);
        hLaneTrf.position = _hMove * NotesSpeed        - new Vector3(offset, 0);
    }

    protected override void LoadNotes()
    {
        // ノーツ生成
        foreach (NotesJson.Notes thisNotes in MusicData.notes)
        {
            // ノーツデータを変数に代入
            int laneNum   = thisNotes.block;
            int notesType = thisNotes.type;
            int notesNum  = thisNotes.num;
            // bpm = musicData.BPM;

            // ノーツの種類判別
            switch ((NotesType) notesType)
            {
                // 単ノーツ
                case NotesType.Single:
                {
                    // 生成位置のトランスフォーム
                    Transform notesGenPosTrf = notesGen[laneNum].transform;

                    // 生成位置
                    Vector3 notesGenPos = new Vector3(notesGenPosTrf.position.x, 0);

                    // 生成
                    GameObject genNotes = Instantiate(NotesPrefab, notesGenPos, Quaternion.identity);

                    genNotes.name                    =  $"notes_{notesNum}";
                    genNotes.transform.parent        =  notesGenPosTrf;
                    genNotes.transform.localPosition =  Vector3.zero;
                    genNotes.transform.localPosition += new Vector3(0, (notesNum + 1) * NotesSpeed);

                    NotesPositionAdd(genNotes, laneNum);

                    break;
                }

                // スライドノーツ
                case NotesType.LongAndSlide:
                {
                    // 先頭ノーツ生成位置のトランスフォーム
                    Transform headSlideNotesGenPosTrf = notesGen[laneNum].transform;

                    // 先頭ノーツの生成位置
                    Vector3 headSlideNotesGenPos = headSlideNotesGenPosTrf.position;

                    // 先頭ノーツ生成
                    GameObject headSlideNotesObj = Instantiate(slideNotes, headSlideNotesGenPos, Quaternion.identity);

                    // 各種設定
                    headSlideNotesObj.name                    =  $"slideStart_{notesNum}";
                    headSlideNotesObj.transform.parent        =  headSlideNotesGenPosTrf;
                    headSlideNotesObj.transform.localPosition =  Vector3.zero;
                    headSlideNotesObj.transform.localPosition += new Vector3(-(notesNum + 1) * NotesSpeed, 0);

                    // 1つ前のスライドノーツ情報記憶
                    Transform     prevSlideNotesTrf = headSlideNotesObj.transform;
                    NotesSelector prevSlideNotesSel = headSlideNotesObj.GetComponent<NotesSelector>();

                    prevSlideNotesSel.laneNum      = laneNum;
                    prevSlideNotesSel.slideSection = SlideNotesSection.Head;

                    NotesPositionAdd(headSlideNotesObj, laneNum);

                    // 先頭以降のノーツ生成
                    foreach (NotesJson.Notes nextSlideNotes in thisNotes.notes)
                    {
                        // ノーツ配列でのインデックス
                        int       j                  = Array.IndexOf(thisNotes.notes, nextSlideNotes);
                        int       nextSlideLaneNum   = nextSlideNotes.block;
                        int       nextSlideNotesNum  = nextSlideNotes.num;
                        bool      isEndNotes         = j == thisNotes.notes.Length - 1;
                        Transform nextSlideGenPosTrf = notesGen[nextSlideLaneNum].transform;

                        // 中間ノーツの生成位置
                        Vector3 nextSlideNotesGenPos = nextSlideGenPosTrf.position;

                        // 中間ノーツ生成
                        GameObject nextSlideNotesObj =
                            Instantiate(slideNotes, nextSlideNotesGenPos, Quaternion.identity);

                        Transform     nextNotesTrf = nextSlideNotesObj.transform;
                        NotesSelector nextNotesSel = nextSlideNotesObj.GetComponent<NotesSelector>();

                        nextNotesSel.laneNum        = nextSlideLaneNum;
                        nextNotesSel.prevSlideNotes = new NotesInfo(prevSlideNotesTrf.gameObject, prevSlideNotesSel);

                        // 末尾ノーツかどうかで情報変更
                        if (isEndNotes)
                        {
                            nextSlideNotesObj.name    = "slideEnd_";
                            nextNotesSel.slideSection = SlideNotesSection.Foot;
                        }
                        else
                        {
                            nextSlideNotesObj.name    = "slideMid_";
                            nextNotesSel.slideSection = SlideNotesSection.Middle;
                        }

                        nextSlideNotesObj.name     += nextSlideNotesNum;
                        nextNotesTrf.parent        =  nextSlideGenPosTrf;
                        nextNotesTrf.localPosition =  Vector3.zero;
                        nextNotesTrf.localPosition += new Vector3(-(nextSlideNotesNum + 1) * NotesSpeed, 0);

                        // 帯の位置
                        Vector3 slideBodyGenPos = new Vector3(nextSlideNotesNum * NotesSpeed,
                                                              nextSlideGenPosTrf.position.y);

                        // 帯生成
                        GameObject slideBodyObj = Instantiate(slideNotesBody, slideBodyGenPos, Quaternion.identity);
                        Transform  slideBodyTrf = slideBodyObj.transform;

                        slideBodyObj.name   = $"slideNotes_{nextSlideNotesNum}";
                        slideBodyTrf.parent = prevSlideNotesTrf;

                        // 位置をスライドノーツの中間に設定
                        slideBodyTrf.position = Vector3.Lerp(nextNotesTrf.position,
                                                             prevSlideNotesTrf.position,
                                                             .5f);

                        // 角度を前後ノーツのベクトルと平行に
                        slideBodyTrf.localRotation = Quaternion.Euler(
                            0, 0, GetAngle(prevSlideNotesTrf.position, nextNotesTrf.position));

                        // 前後ノーツ間の距離
                        float prevAndNextNotesDist = Vector3.Distance(prevSlideNotesTrf.position,
                                                                      nextNotesTrf.position);

                        // 帯のスケールの比率。帯と親のスケールが異なる場合への対応
                        float slideBodyScaleRatio = slideBodyTrf.localScale.x / slideBodyTrf.parent.localScale.x;

                        // 帯のスケール。そのままだと向きが逆転するため符号反転
                        Vector3 slideBodyScale = new Vector3(-prevAndNextNotesDist / 10 * slideBodyScaleRatio,
                                                             -slideBodyTrf.localScale.y);

                        slideBodyTrf.localScale = slideBodyScale;

                        prevSlideNotesSel.nextSlideNotes = new NotesInfo(nextSlideNotesObj, nextNotesSel);

                        prevSlideNotesTrf = nextSlideNotesObj.transform;
                        prevSlideNotesSel = nextSlideNotesObj.GetComponent<NotesSelector>();

                        NotesPositionAdd(nextSlideNotesObj, nextSlideLaneNum);
                    }

                    break;
                }

                default:
                    continue;
            }

            move = new Vector3(0, 1.06f * Time.deltaTime);
            NotesManager.NextNotesLine.Add(laneNum);
        }

        Generated = true;
    }

    /// <summary>あるオブジェクトからあるオブジェクトまでの角度を取得する</summary>
    /// <param name="start">始点のベクトル</param>
    /// <param name="target">終点のベクトル</param>
    /// <returns>角度</returns>
    private static float GetAngle(Vector2 start, Vector2 target)
    {
        Vector2 dt     = target - start;
        float   radian = Mathf.Atan2(dt.y, dt.x);
        float   degree = radian * Mathf.Rad2Deg;

        return degree;
    }
}
