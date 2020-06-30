using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField]
    string SceneName = null;
    bool PushButton=false;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.BGMSoundCue(1);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)&&!PushButton)
        {
            PushButton = true;
            SceneLoadManager.LoadScene(SceneName);
        } 
    }
}
