using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelExit : MonoBehaviour
{
    [SerializeField] GameObject Panel;

    public void PushButton()
    {
        Panel.SetActive(false);
        SelectMusicPanelController.isPopUp = false;
    }
}
