using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ノーツのマネージャー
/// リスト管理している。
/// </summary>
public class NotesManager : MonoBehaviour
{
    public static List<List<(GameObject gameObject, NotesSelector selector)>> NotesPositions =
        new List<List<(GameObject gameObject, NotesSelector selector)>>();

    public static List<int> NextNotesLine = new List<int>();

    private void Update()
    {
    }
}
