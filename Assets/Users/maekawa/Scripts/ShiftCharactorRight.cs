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

    private void OnTriggerEnter2D(Collider2D collision)
    {
        selectCharactor.SetCharactor(true);
    }
}
