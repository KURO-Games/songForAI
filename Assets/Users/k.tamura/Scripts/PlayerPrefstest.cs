using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefstest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefsUtil<int>.Save("huga", 100);
        int hoge = PlayerPrefsUtil<int>.Load("huga", -1);
        Debug.LogWarning(hoge);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
