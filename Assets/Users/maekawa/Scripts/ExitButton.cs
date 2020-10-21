using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitButton : MonoBehaviour
{
    public void Onclick()
    {
        int life = PlayerPrefs.GetInt("Lifes");

        if (life == 0)
            SceneLoadManager.LoadScene("PlayEnd");
        else
        {
            PlayerPrefs.SetInt("Lifes", life--);
            SceneLoadManager.LoadScene("SelectMusicV2");
        }
    }
}
