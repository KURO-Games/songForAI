using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PianoNotesGenerator : NotesGeneratorBase
{
    protected override void CalculateNotesPositions()
    {
        // ノーツ生成およびジャケット表示が済んだら座標計算開始
        if (!Generated || !jacketIsFaded) return;

        SoundManager.BgmTime(ref BgmTimes);

        NotesSpeeds = BgmTimes / 6.12f;
        move        = new Vector3(0, NotesSpeeds * speed, 0); //曲の再生ポジションでとっている値

        if (PlayedBGM) return;

        SoundManager.BGMSoundCue(MusicDatas.cueMusic);
        PlayedBGM = true;
    }

    protected override void MoveNotes()
    {
        rootObj.transform.position = move * (-1 * NotesSpeed) + new Vector3(0, offset);
    }

    protected override void LoadNotes()
    {
        // ノーツ生成
        foreach (NotesJson.Notes thisNotes in musicData.notes)
        {
            // ノーツデータを変数に代入
            int laneNum   = thisNotes.block;
            int notesType = thisNotes.type;
            int notesNum  = thisNotes.num;
            // 生成先のトランスフォーム
            Transform notesGenPosTrf = notesGen[laneNum].transform;
            // bpm = musicData.BPM;

            // ノーツの種類判別
            switch ((NotesType) notesType)
            {
                case NotesType.Single:
                {
                    // 生成位置
                    Vector3 notesGenPos = new Vector3(notesGenPosTrf.position.x, 0);

                    // 生成
                    GameObject genNotes = Instantiate(NotesPrefab, notesGenPos, Quaternion.identity);

                    // 各種設定
                    genNotes.name                    =  $"notes_{notesNum}";
                    genNotes.transform.parent        =  notesGenPosTrf;
                    genNotes.transform.localPosition =  Vector3.zero;
                    genNotes.transform.localPosition += new Vector3(0, (notesNum + 1) * NotesSpeed);

                    NotesPositionAdd(genNotes, laneNum);

                    break;
                }

                case NotesType.LongAndSlide:
                {
                    int        longNotesNum      = thisNotes.notes[0].num;
                    Vector3    longNotesGenPos   = new Vector3(notesGenPosTrf.position.x, 0);
                    GameObject longNotesGenNotes = Instantiate(longNotes, longNotesGenPos, Quaternion.identity);

                    // 各種設定
                    longNotesGenNotes.name                    =  $"longNotes_{notesNum}";
                    longNotesGenNotes.transform.parent        =  notesGenPosTrf;
                    longNotesGenNotes.transform.localPosition =  Vector3.zero;
                    longNotesGenNotes.transform.localPosition += new Vector3(0, notesNum * NotesSpeed);

                    // スケール設定
                    Vector2 longNotesScale = new Vector2(0.19f, longNotesNum - notesNum);
                    longNotesScale.y                       *= 0.03f * (16f / thisNotes.LPB);
                    longNotesGenNotes.transform.localScale =  longNotesScale;

                    NotesPositionAdd(longNotesGenNotes, laneNum);

                    /* ロングノーツ始点 */

                    // 始点位置
                    Vector3 startGenPos = new Vector3(notesGenPosTrf.position.x,
                                                      longNotesGenNotes.transform.position.y - .1f);

                    // 始点オブジェクト生成
                    GameObject startEdge = Instantiate(edgeStart, startGenPos, Quaternion.identity);

                    // 各種設定
                    startEdge.name             = $"startEdge_{notesNum}";
                    startEdge.transform.parent = longNotesGenNotes.transform;

                    NotesSelector longNotesSel = longNotesGenNotes.GetComponent<NotesSelector>();

                    /* ロングノーツ終点 */

                    // 終点位置
                    Vector3 endGenPos = new Vector3(notesGenPosTrf.position.x,
                                                    longNotesSel.EndNotes.transform.position.y + .1f);

                    // 終点オブジェクト生成
                    GameObject endEdge = Instantiate(edgeEnd, endGenPos, Quaternion.identity);

                    // 各種設定
                    endEdge.name             = $"endEdge_{notesNum}";
                    endEdge.transform.parent = longNotesGenNotes.transform;

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
}
