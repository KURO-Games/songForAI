using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class NotesCountUp : MonoBehaviour
{
    [SerializeField] int gameType; 

    GameObject obj;
    ComboManager cm;

    private void Start()
    {
        obj = GameObject.Find("UICtrlCanvas");
        cm = obj.GetComponent<ComboManager>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Judge.NotesCountUp(this.gameObject.name, gameType);
        //cm.DrawCombo(0);
    }
}
