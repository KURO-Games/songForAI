using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RhithmGameManager : MonoBehaviour
{
    bool _isTaped = false;
    // Start is called before the first frame update
    void Start()
    {
        _isTaped = false;
        SoundManager.BGMSoundStop();
    }
    public void ReturnHome()
    {
        if (!_isTaped)
        {
            SoundManager.BGMSoundStop();
            _isTaped = true;
            SceneLoadManager.LoadScene("MockHome");
        }
    }
}
