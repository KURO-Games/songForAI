using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesManager : MonoBehaviour
{
    public static List<List<GameObject>> NotesPositions = new List<List<GameObject>>();
    public static List<int> NextNotesLine = new List<int>();
    private void Awake()
    {
        for (int i = 0; i < 8; i++)
        {
            NotesPositions.Add(new List<GameObject>());
        }
        Debug.Log(NotesPositions);
    }
    void Update()
    {
        
    }
}
