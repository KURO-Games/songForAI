using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestOnTriggerEnter2D : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D col)//mainで呼ばなくていい
    {
        string i = this.name;//ヒットしたオブジェクトの名前を取得
        Debug.Log(i + "_" + "OK");
    }

    void Update()
    {

    }
}
