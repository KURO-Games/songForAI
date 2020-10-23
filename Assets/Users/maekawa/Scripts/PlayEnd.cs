using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEnd : MonoBehaviour
{
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            SelectMusicScene.life = 2;
            SceneLoadManager.LoadScene("Title");
        }
    }
}
