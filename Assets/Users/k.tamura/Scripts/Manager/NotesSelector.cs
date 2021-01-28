using UnityEngine;

public enum SlideNotesSection
{
    Head = 0,
    Middle,
    Foot
}

public class NotesSelector : MonoBehaviour
{
    // ノーツのレーン番号
    [HideInInspector]
    public int laneNum;

    // グレード判定が行われたか（スライドノーツ）
    // [HideInInspector]
    public bool isJudged;

    // スライドノーツの前後の一連オブジェクト
    public (GameObject gameObject, NotesSelector selector) prevSlideNotes;
    public (GameObject gameObject, NotesSelector selector) nextSlideNotes;

    public SlideNotesSection? slideSection = null; // スライドノーツの種別
    public NotesType          notesType;
    public GameObject         endNotes;
}
