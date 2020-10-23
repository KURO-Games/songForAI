using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting.Lifetime;
using UnityEngine;

public class RetryButton : MonoBehaviour
{
    private bool isClick;

    private void Start()
    {
        isClick = true;
    }
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

        if (isClick)
        {
            isClick = false;
            SelectMusicScene.life--;
            int life = SelectMusicScene.life;//PlayerPrefs.GetInt("Lifes", 3);
            if (life > 0)
            {
                //PlayerPrefs.SetInt("Lifes", life--);
                SceneLoadManager.LoadScene("Resize_RhythmGame2");
            }
        }
    }
}
