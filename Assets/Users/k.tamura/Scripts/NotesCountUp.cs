using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class NotesCountUp : MonoBehaviour
{
    Judge _judge;

    GameObject obj;
    ComboManager cm;

    private void Start()
    {
        obj = GameObject.Find("UICtrlCanvas");
        cm = obj.GetComponent<ComboManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Judge.NotesCountUp(this.gameObject.name);
        cm.DrawCombo(0);
        //Destroy(collision.gameObject);
    }
}
