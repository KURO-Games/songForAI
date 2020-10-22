using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using UnityEngine;

public class RetryButton : MonoBehaviour
{
    public static int gameType;

    public void Onclick()
    {
        //gameType = MusicDatas.gameType;
        //switch(gameType)
        //{
        //    case 0:
        //        SceneLoadManager.LoadScene("Resize_RhythmGame2");
        //        break;
        //    case 1:
        //        SceneLoadManager.LoadScene("StringRhythmGameScene");
        //        break;
        //}

        int life = PlayerPrefs.GetInt("Lifes", 3);

        if (life > 0)

        {
            PlayerPrefs.SetInt("Lifes", life--);
            SceneLoadManager.LoadScene("Resize_RhythmGame2");
        }
    }
}
