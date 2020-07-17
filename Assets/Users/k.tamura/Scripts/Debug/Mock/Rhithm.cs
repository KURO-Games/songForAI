using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rhithm : MonoBehaviour
{
    bool _isTaped = false;
    // Start is called before the first frame update
    void Start()
    {
        _isTaped = false;
    }
    public void ReturnHome()
    {
        if (!_isTaped)
        {
            _isTaped = true;
            SceneLoadManager.LoadScene("MockHome");
        }
    }
}
