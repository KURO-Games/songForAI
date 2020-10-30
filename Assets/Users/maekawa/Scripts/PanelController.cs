using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelController : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Onclick();
        }
    }

    public void Onclick()
    {
        this.gameObject.transform.root.gameObject.SetActive(false);
    }
}
