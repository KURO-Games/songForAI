using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockHome : MonoBehaviour
{
    bool pushButton=false;
    // Start is called before the first frame updat
    public void OnButton()
    {
        if (!pushButton)
        {
            pushButton = true;
            SceneLoadManager.LoadScene("SelectMusic");
        }
    }

}
