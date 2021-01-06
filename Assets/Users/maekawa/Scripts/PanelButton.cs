using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelButton : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] GameObject details;
    public void Onclick()
    {
        details.SetActive(false);
        panel.SetActive(false);
    }
}
