using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShiftCharactorLeft : MonoBehaviour
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
            selectCharactor.SetCharactor(false);
    }
}
