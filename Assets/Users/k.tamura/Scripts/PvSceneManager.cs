using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class PvSceneManager :MonoBehaviour
{
    [SerializeField]
    VideoPlayer VideoPlayer = default(VideoPlayer);
    bool isPlaying = false;
    [SerializeField]
    int OffsetFrame = 10;
    int isFrame = default(int);

    // Start is called before the first frame update
    public void PlayPV()
    {
        VideoPlayer.Play();
        isPlaying = true;

    }

    // Update is called once per frame
    private void Start()
    {
        SoundManager.BGMStop();
        isFrame = 0;
    }
    void Update()
    {
        
        if (!isPlaying)
        {
            if (SceneLoadManager.Loading) return;
            PlayPV();
            return;
        }
        else if (!VideoPlayer.isPlaying&&isFrame>OffsetFrame&&isPlaying)
        {
            isPlaying = false;
            SceneLoadManager.LoadScene("Init");
            isFrame = 0;
        }
        else if(Input.GetMouseButtonDown(0)&& isPlaying)
        {
            isPlaying = false;
            SceneLoadManager.LoadScene("Title");
            isFrame = 0;
        }
        isFrame++;

    }
}
