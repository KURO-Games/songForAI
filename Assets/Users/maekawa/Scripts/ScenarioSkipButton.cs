using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioSkipButton : MonoBehaviour
{
    private bool isClick;

    private void Start()
    {
        isClick = true;
    }
    public void Onclick()
    {
       if(isClick)
        {
            isClick = false;
            if (SelectMusicScene.life <= 0)
                SceneLoadManager.LoadScene("PlayEnd");
            else
            SceneLoadManager.LoadScene("SelectMusicV3");
        }
    }
}
