using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ノーツのマネージャー
/// リスト管理している。
/// </summary>
public class NotesManager : MonoBehaviour
{
    public static List<List<NotesInfo>> NotesPositions = new List<List<NotesInfo>>();
    public static List<int>             NextNotesLine  = new List<int>();
}
