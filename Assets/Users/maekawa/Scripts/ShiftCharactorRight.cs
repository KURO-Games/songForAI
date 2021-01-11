using System;
using UnityEngine;

public class ShiftCharactorRight : MonoBehaviour
{
    private SelectCharactor selectCharactor;

    private void Start()
    {
        GameObject obj = GameObject.Find("SelectCharactor");
        selectCharactor = obj.GetComponent<SelectCharactor>();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Input.GetMouseButtonUp(0) && collision.gameObject.tag == "Charactor") 
            selectCharactor.SetCharactor(true);
    }
}
