using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelExit : MonoBehaviour
{
    [SerializeField] GameObject Panel;

    public void PushButton()
    {
        SoundManager.SetVolume(1.0f, SoundType.DemoBGM);
        SoundManager.DemoBGMSoundCue(SelectMusic.selectNumber);
        Panel.SetActive(false);
        SelectMusicPanelController.isPopUp = false;
    }
}
