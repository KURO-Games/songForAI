using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelButton : MonoBehaviour
{
    public void Onclick()
    {
        this.gameObject.transform.root.gameObject.SetActive(false);
    }
}
