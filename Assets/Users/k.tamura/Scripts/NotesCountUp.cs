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
        if(collision.gameObject.tag == "Notes")
        {
            string i = this.gameObject.name;
            int j = int.Parse(i);

            if (KeyJudge.isHold[j] != true)
            {
                Judge.NotesCountUp(j);
            }
        }
    }
}
