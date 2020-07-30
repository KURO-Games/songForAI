using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockHome : MonoBehaviour
{
    bool pushButton=false;

    private void Start()
    {
        pushButton = false;
        SoundManager.BGMSoundCue(2);
    }
    public void OnButton()
    {
        if (!pushButton)
        {
            pushButton = true;
            SceneLoadManager.LoadScene("SelectMusicV2");
        }
    }
    public void ScenarioScene()
    {
        if (!pushButton)
        {
            pushButton = true;
            SceneLoadManager.LoadScene("Scenario");
        }
    }
}
