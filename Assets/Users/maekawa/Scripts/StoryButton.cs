using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoryButton : MonoBehaviour
{
    public void Onclick()
    {
        if (ExitButton.isClick)
        {
            ExitButton.isClick = false;
            if (SelectMusicScene.life > 0)
            {
                SceneLoadManager.LoadScene("Scenario");
            }
        }
    }
}
