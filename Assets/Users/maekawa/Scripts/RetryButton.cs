using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryButton : MonoBehaviour
{
    public void Onclick()
    {
        if (Result.isClick)
        {
            if (SelectMusicScene.life > 0)
            {
                Result.isClick = false;
                SceneLoadManager.LoadScene("Resize_RhythmGame2");
            }
        }
    }
}
