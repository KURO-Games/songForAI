using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScenarioSkipButton : MonoBehaviour
{
    public void Onclick()
    {
        SceneLoadManager.LoadScene("SelectMusicV3");
    }
}
