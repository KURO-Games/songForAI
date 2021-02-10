using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEnd : MonoBehaviour
{
    private bool isClick;
    void Start()
    {
        isClick = true;
    }
    void Update()
    {
        if(Input.GetMouseButton(0) && isClick)
        {
            isClick = false;
            Scenario_Controller.scenarioNumber = 0;
            SelectMusicScene.life = 2;
            SoundManager.BGMStop();
            SoundManager.LoadAsyncCueSheet(SoundDefine.Title,SoundType.BGM);
            SceneLoadManager.LoadScene("Title");
        }
    }
}
