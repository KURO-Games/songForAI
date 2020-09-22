using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryButton : MonoBehaviour
{
    public void Onclick()
    {
        SceneLoadManager.LoadScene("Resize_RhythmGame2");
    }
}
