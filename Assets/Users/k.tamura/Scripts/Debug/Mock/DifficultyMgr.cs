using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyMgr : MonoBehaviour
{
    bool _isTaped = false;
    // Start is called before the first frame update
    void Start()
    {
        _isTaped = false;
    }

    // Update is called once per frame
    public void ChooseDifficult()
    {
        if (!_isTaped)
        {
            _isTaped = true;
            SceneLoadManager.LoadScene("RhythmGame");
        }
    }
    public void ReturnHome()
    {
        if (!_isTaped)
        {
            _isTaped = true;
            SceneLoadManager.LoadScene("Home");
        }
    }
}
