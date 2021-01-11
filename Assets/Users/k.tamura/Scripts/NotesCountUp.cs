using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using UnityScript.Steps;

/// <summary>
/// ノーツのカウントアップ処理
/// </summary>
public class NotesCountUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Notes"))
        {
            string i = gameObject.name;
            int j = int.Parse(i);

            if (NotesJudgementBase.isHold[j] != true)
            {
                NotesJudgementBase.NotesCountUp(j);
            }
        }
    }
}
