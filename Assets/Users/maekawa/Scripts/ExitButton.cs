using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
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
            SelectMusicScene.life--;
            int life = SelectMusicScene.life;//PlayerPrefs.GetInt("Lifes", 3);
            if (life <= 0)
            {
                //PlayerPrefs.SetInt("Lifes", 3);
                SceneLoadManager.LoadScene("PlayEnd");
            }
            else
            {
                //PlayerPrefs.SetInt("Lifes", life--);

                SceneLoadManager.LoadScene("SelectMusicV3");
            }
        }
    }
}
