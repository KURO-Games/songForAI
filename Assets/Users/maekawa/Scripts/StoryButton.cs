using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryButton : MonoBehaviour
{
    public void Onclick()
    {
        if (Result.isClick)
        {
            Result.isClick = false;
            SoundManager.LoadAsyncCueSheet(SoundDefine.Home, SoundType.BGM);
            SceneLoadManager.LoadScene("Scenario");
        }
    }
}
