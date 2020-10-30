using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public static bool isClick;

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
            {
                //PlayerPrefs.SetInt("Lifes", 3);
                SceneLoadManager.LoadScene("PlayEnd");
            }
            else
            {
                SceneLoadManager.LoadScene("SelectMusicV3");
            }
        }
    }
}
