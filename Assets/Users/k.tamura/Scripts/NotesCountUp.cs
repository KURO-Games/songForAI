﻿using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

public class NotesCountUp : MonoBehaviour
{
    Judge _judge;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        Judge.NotesCountUp(this.gameObject.name);

        //Destroy(collision.gameObject);
    }
}