using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawGrade : MonoBehaviour
{
    float transparentTime = 0.5f;                        // UIが消えるまでの時間（秒）


    [SerializeField] Image[] gradesImage = new Image[5]; // perfect ～ miss
    private float[] alpha = new float [5];               // 各不透明度
    private float fps = 60;                              // 60fpsを前提
    private float transparentPerFrame = 0;               // 1フレームあたりに減算される不透明度

    void Start()
    {
        transparentPerFrame = fps * transparentTime;       

        for(int i = 0; i < alpha.Length; i++)
        {
            alpha[i] = 0;
        }
    }

    void Update()
    {
        // 透過処理
        for (int i = 0; i < gradesImage.Length; i++)
        {
            gradesImage[i].GetComponent<Image>().color = new Color(255, 255, 255, alpha[i]);

            alpha[i] -= 1.0f / transparentPerFrame;
        }
    }

    /// <summary>
    /// 判定に応じたUIを描画します
    /// </summary>
    /// <param name="i">grade</param>
    public void DrawGrades(int i)
    {
        alpha[i] = 1;
    }
}
