using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawGrade : MonoBehaviour
{
    float transparentTime = 0.5f;    // UIが消えるまでの時間


    [SerializeField] Image[] gradesImage = new Image[5];// perfect ～ miss
    float[] alpha = {0, 0, 0, 0, 0 };// 不透明度
    float fps = 60;
    float transparentPerFrame = 0;   // 1フレームあたりに減算される不透明度

    void Start()
    {
        transparentPerFrame = fps * transparentTime;       
    }

    void Update()
    {
        for (int i = 0; i < gradesImage.Length; i++)
        {
            if (alpha[i] > 0)
            {
                alpha[i] -= 1.0f / transparentPerFrame;

                gradesImage[i].GetComponent<Image>().color = new Color(255, 255, 255, alpha[i]);
            }
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
