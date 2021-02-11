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
    public NotesInfo prevSlideNotes;
    public NotesInfo nextSlideNotes;

    public SlideNotesSection? slideSection = null; // スライドノーツの種別
    public NotesType          notesType;
    public GameObject         endNotes;
}

public class NotesInfo
{
    public GameObject    GameObject;
    public NotesSelector Selector;

    public NotesInfo(GameObject obj, NotesSelector selector)
    {
        GameObject = obj;
        Selector = selector;
    }
}
