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
            Scenario_Controller.scenarioNumber++;
            SceneLoadManager.LoadScene("Scenario");
        }
    }
}
