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

        if(PlayerPrefs.GetInt("Lifes") > 0)
        SceneLoadManager.LoadScene("Resize_RhythmGame2");
    }
}
