using UnityEngine;

/// <summary>
/// ノーツのカウントアップ処理
/// </summary>
public class NotesCountUp : MonoBehaviour
{
    private int _laneNum;

    private void Start()
    {
        _laneNum = int.Parse(gameObject.name);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ノーツのみ処理
        if (!collision.gameObject.CompareTag("Notes")) return;

        (GameObject _, NotesSelector notesSel) =
            NotesJudgementBase.GOListArray[_laneNum][NotesJudgementBase.notesCount[_laneNum]];


        // 未判定ノーツで非ホールド中またはスライドノーツなら
        if ((NotesJudgementBase.isHold[_laneNum] != true ||
             notesSel.slideSection               != null) &&
            !notesSel.isJudged)
        {
            NotesJudgementBase.Instance.NotesCountUp(_laneNum, isLongStart: notesSel.endNotes != null);
        }
    }
}
