using System.Collections.Generic;
using UnityEngine;

public class PianoNotesGenerator : NotesGeneratorBase
{
    protected override void CalculateNotesPositions()
    {
        if (!Generated) return;

        SoundManager.BgmTime(ref BgmTimes);

        NotesSpeeds = ((BgmTimes) / 6.12f);
        move        = new Vector3(0, NotesSpeeds * speed, 0); //曲の再生ポジションでとっている値

        if (PlayedBGM) return;

        // ノーツを遅らせる
        SoundManager.BGMSoundCue(MusicDatas.cueMusic);
        PlayedBGM = true;
    }

    protected override void MoveNotes()
    {
        rootObj.transform.position = move * (-1 * NotesSpeed) + new Vector3(0, offset, 0);
    }

    protected override void LoadNotes()
    {
        // ノーツ生成
        for (int i = 0; musicData.notes.Length > i; i++)
        {
            // リスト初期化
            NotesManager.NotesPositions.Add(new List<GameObject>()); //nex

            for (int e = 0; e < musicData.maxBlock; e++)
            {
                NotesManager.NotesPositions[i].Add(null);
            }

            // ノーツデータを変数に代入
            int laneNum   = musicData.notes[i].block;
            int notesType = musicData.notes[i].type;
            int notesNum  = musicData.notes[i].num;
            // bpm = musicData.BPM;

            // ノーツの種類判別
            switch ((NotesType) notesType)
            {
                case NotesType.Single:
                {
                    //生成
                    GameObject GenNotes = Instantiate(NotesPrefab, new Vector3(NotesGen[laneNum].transform.position.x
                                                                               , 0
                                                                               , 0)
                                                      , Quaternion.identity);

                    GenNotes.name             = "notes_" + notesNum;
                    GenNotes.transform.parent = NotesGen[laneNum].transform;
                    Vector3 positions = new Vector3(0, (notesNum + 1) * NotesSpeed, 0);
                    GenNotes.transform.localPosition =  new Vector3(0, 0, 0);
                    GenNotes.transform.localPosition += positions;
                    NotesPositionAdd(GenNotes, laneNum, i);

                    break;
                }

                case NotesType.LongAndSlide:
                {
                    int notesNum2 = musicData.notes[i].notes[0].num;
                    GameObject GenNotes = Instantiate(longNotes, new Vector3(NotesGen[laneNum].transform.position.x
                                                                             , notesNum * NotesSpeed
                                                                             , 0)
                                                      , Quaternion.identity);

                    GenNotes.name             = "notes_" + notesNum;
                    GenNotes.transform.parent = NotesGen[laneNum].transform;
                    Vector2 longPos = new Vector2(0.19f, notesNum2 - notesNum);
                    longPos.y *= 0.03f * (16 / musicData.notes[i].LPB);
                    //longPos.y *= 0.03f * (4 / musicData.notes[i].LPB);
                    GenNotes.transform.localScale = longPos;
                    NotesPositionAdd(GenNotes, laneNum, i);

                    // ロングノーツ始点オブジェクト生成
                    GameObject startEdge = Instantiate(edgeStart, new Vector3(NotesGen[laneNum].transform.position.x
                                                                              , notesNum * NotesSpeed
                                                                              , 0)
                                                       , Quaternion.identity);

                    startEdge.name             = "startEdge_" + notesNum;
                    startEdge.transform.parent = GenNotes.transform;

                    // ロングノーツ終点オブジェクト生成
                    GameObject endEdge = Instantiate(edgeEnd, new Vector3(NotesGen[laneNum].transform.position.x
                                                                          , GenNotes.GetComponent<NotesSelector>()
                                                                              .EndNotes.transform.position.y
                                                                          , 0)
                                                     , Quaternion.identity);

                    endEdge.name             = "endEdge_" + notesNum;
                    endEdge.transform.parent = GenNotes.transform;

                    break;
                }

                default:
                    continue;
            }

            move = new Vector3(0, 1.06f * Time.deltaTime, 0);
            NotesManager.NextNotesLine.Add(laneNum);
            Generated = true;
        }
    }
}
