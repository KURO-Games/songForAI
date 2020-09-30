using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// ノーツのカウントアップ処理
/// </summary>
public class NotesCountUp : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Judge.NotesCountUp(this.gameObject.name);
    }
}
