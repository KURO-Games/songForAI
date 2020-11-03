using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void Onclick()
    {
        if(Result.isClick)
        {
            if (SelectMusicScene.life <= 0)
            {
                Result.isClick = false;
                SceneLoadManager.LoadScene("PlayEnd");
            }
            else
            {
                Result.isClick = false;
                SceneLoadManager.LoadScene("SelectMusicV3");
            }
        }
    }
}
