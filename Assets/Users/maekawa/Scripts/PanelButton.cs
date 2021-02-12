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

        // 演奏ランクB以上でグッドボイス、C以下でバッドボイス再生
        int cueID = Result.rankNum >= 2 ? 2 : 3;
        SoundManager.ScenarioSoundCue(cueID);
    }
}
