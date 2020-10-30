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
            SceneLoadManager.LoadScene("SelectMusicV3");
        }
    }
}
