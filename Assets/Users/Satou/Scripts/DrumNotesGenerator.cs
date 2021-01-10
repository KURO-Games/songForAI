using UnityEngine;
using System.Collections.Generic;
using System.Linq;
/// <summary>
/// ドラム用ノーツジェネレータ
/// </summary>
public class DrumNotesGenerator : NotesGeneratorBase
{
    int drumNotesCount;                         //ノーツのカウント
    List<GameObject> nowInProgressDrumNotes = new List<GameObject>();    //拡大中のノーツのリスト
    List<int> allNotesLaneNum;                  //すべてのノーツのレーン番号を格納
    GameObject LastNotes;                       //最後のノーツ格納

    protected override void CalculateNotesPositions()
    {
        if (!Generated || !jacketIsFaded) return;

        SoundManager.BgmTime(ref BgmTimes);

        NotesSpeeds = ((BgmTimes) / 6.12f);
        move = new Vector3(0, NotesSpeeds * speed, 0); //曲の再生ポジションでとっている値

        if (PlayedBGM) return;

        // ノーツを遅らせる
        SoundManager.BGMSoundCue(MusicDatas.cueMusic);
        PlayedBGM = true;
    }

    protected override void MoveNotes()
    {
        if (!Generated) return;

        //ドラムの円の中心から浮き出るように動かす
        //allNotesLaneNum次に生成されるノーツを確認して拡大される
        //NotesGen[0].transform.root.gameObject.transform.position = move * (-1 * NotesSpeed) + new Vector3(0, offset, 0);
        var displayPosition = NotesSpeeds * speed;      //ドラムの表示位置

        //ノーツの拡大表示処理
        if(drumNotesCount < musicData.notes.Length && LastNotes != null)
        {
            //ノーツデータを変数に代入
            int notesNum = musicData.notes[drumNotesCount].num;
            int laneNum = musicData.notes[drumNotesCount].block;

            //ノーツの表示位置なったら対象のノーツ処理対象に追加する
            if (notesNum <= displayPosition)
            {
                //処理させたいノーツを検索させる
                List<GameObject> wantToBeProgress = new List<GameObject>();
                foreach(List<GameObject> lane in NotesManager.NotesPositions)
                {
                    foreach(GameObject notes in lane)
                    {
                        if (notes != null && notes.name == $"notes_{notesNum}")
                        {
                            wantToBeProgress.Add(notes);
                        }
                    }
                }

                //デストロイされたノーツは排除
                nowInProgressDrumNotes.RemoveAll(notes => notes == null);
                //処理させたいノーツを処理中に追加
                nowInProgressDrumNotes.AddRange(wantToBeProgress);
                //ノーツの重複削除
                nowInProgressDrumNotes.Distinct();
                drumNotesCount++;
            }

            //デバッグ用(タップ判定完成したら削除)
            List<GameObject> trashNotes = new List<GameObject>();
            //ノーツ拡大処理
            foreach(GameObject notes in nowInProgressDrumNotes)
            {
                notes.transform.localScale += new Vector3(Time.deltaTime, Time.deltaTime);

                //デバッグ用(タップ判定完成したら削除)
                if (notes.transform.localScale.x >= 1)
                {
                    trashNotes.Add(notes);
                    Destroy(notes);
                }
            }
            //デバッグ用(タップ判定完成したら削除)
            foreach(GameObject notes in trashNotes)
            {
                nowInProgressDrumNotes.RemoveAll(notes2 => notes2 == notes);
            }
        }


    }

    protected override void LoadNotes()
    {
        // ノーツ生成
        for (var i = 0; musicData.notes.Length > i; i++)
        {
            // リスト初期化
            NotesManager.NotesPositions.Add(new List<GameObject>()); //nex

            for (var e = 0; e < 7; e++)
            {
                NotesManager.NotesPositions[i].Add(null);
            }

            // ノーツデータを変数に代入
            var laneNum = musicData.notes[i].block;
            var notesNum = musicData.notes[i].num;

            //生成
            var GenNotes = Instantiate(NotesPrefab, new Vector3(notesGen[laneNum].transform.position.x
                                                              , notesGen[laneNum].transform.position.y
                                                              , 0)
                                       , Quaternion.identity);

            GenNotes.name = "notes_" + notesNum;
            GenNotes.transform.parent = notesGen[laneNum].transform;
            GenNotes.transform.localPosition = new Vector3(0, 0, 0);
            GenNotes.transform.localScale = Vector3.zero;
            NotesPositionAdd(GenNotes, laneNum, i);
            if (i == musicData.notes.Length - 1)
            {
                LastNotes = GenNotes;
            }

            move = new Vector3(0, 1.06f * Time.deltaTime, 0);
            NotesManager.NextNotesLine.Add(laneNum);
            Generated = true;
        }
    }
}
//