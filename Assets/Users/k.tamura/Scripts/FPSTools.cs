using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSTools : MonoBehaviour
{

    // 変数
    int frameCount;
    float prevTime;
    float fps;

    // 初期化処理
    void Start()
    {
        // 変数の初期化
        frameCount = 0;
        prevTime = 0.0f;
    }

    // 更新処理
    void Update()
    {
        frameCount++;
        float time = Time.realtimeSinceStartup - prevTime;

        if (time >= 0.5f)
        {
            fps = frameCount / time;

            frameCount = 0;
            prevTime = Time.realtimeSinceStartup;
        }
    }

    // 表示処理
    private void OnGUI()
    {
        GUILayout.Label("FPS:"+fps.ToString());
    }
}