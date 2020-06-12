using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundPlays : MonoBehaviour
{
    [SerializeField]
    int SoundNumBGM=0, SoundNumSE=0;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            OnButtonPlaySE();
        }
    }
    public void OnButtonPlayBGM()
    {
        SoundManager.BGMSoundCue(SoundNumBGM);
    }
    private void OnButtonPlaySE()
    {
        SoundManager.SESoundCue(SoundNumSE);
    }


}
