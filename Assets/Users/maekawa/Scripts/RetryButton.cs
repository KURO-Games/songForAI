using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryButton : MonoBehaviour
{
    public void Onclick()
    {
        if (Result.isClick && SelectMusicScene.life > 0)
        {
            switch (MusicDatas.gameType)
            {
                case GameType.Piano:
                    Result.isClick = false;
                    SceneLoadManager.LoadScene("Piano");
                    break;
                case GameType.Violin:
                    Result.isClick = false;
                    SceneLoadManager.LoadScene("ViolineDev");
                    break;
                default:
                    break;
            }

        }
    }
}
