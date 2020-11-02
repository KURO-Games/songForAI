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
        SoundManager.AllBGMSoundStop();
    }
    public void ReturnHome()
    {
        if (!_isTaped)
        {
            SoundManager.AllBGMSoundStop();
            _isTaped = true;
            SceneLoadManager.LoadScene("MockHome");
        }
    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Debug.Log("出力テスト");
            SoundManager.SESoundCue(0);
        }
    }
}
