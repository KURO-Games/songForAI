using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayEnd : MonoBehaviour
{
    void Update()
    {
        if(Input.GetMouseButton(0))
        {
            SceneLoadManager.LoadScene("Title");
        }
    }
}
