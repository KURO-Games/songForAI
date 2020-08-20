using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCXharaMgr : MonoBehaviour
{
    bool _isTaped = false;
    // Start is called before the first frame update
    void Start()
    {
        _isTaped = false;
    }

    // Update is called once per frame
    public void ChooseCharacter()
    {
        if (!_isTaped)
        {
            _isTaped = true;
            SceneLoadManager.LoadScene("Resize_RhythmGame2");
        }
    }
    public void ReturnHome()
    {
        if(!_isTaped)
        {
            _isTaped = true;
            SceneLoadManager.LoadScene("Home");
        }
    }
}
