using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChange : MonoBehaviour
{
    [SerializeField]
    string SceneName = null;
    bool PushButton=false;
    [SerializeField]
    float PvSceneTime = default(float);
    [SerializeField]
    float Times = default(float);
    // Start is called before the first frame update
    void Start()
    {
        PushButton = false;
        SoundManager.BGMSoundCue(1);
        PlayerPrefs.DeleteAll();
        Times = 0;
    }

    // Update is called once per frame
    void Update()
    {
        Times += Time.deltaTime;
        if(Input.GetMouseButtonDown(0)&&!PushButton)
        {
            PushButton = true;
            SceneLoadManager.LoadScene(SceneName);
            SoundManager.SESoundCue(7);
        } 
        if(Times >= PvSceneTime && !PushButton)
        {
            PushButton = true;
            SceneLoadManager.LoadScene("PV");
        }


    }
}
