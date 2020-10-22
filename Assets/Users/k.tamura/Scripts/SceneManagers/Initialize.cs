using UnityEngine;
/// <summary>
/// 初期化シーン
/// </summary>
public class Initialize : MonoBehaviour
{
    void Start()
    {
        //　フレームレート60FPSに固定
        Application.targetFrameRate = 60;
        PlayerPrefs.SetInt("Life", 3);
        SceneLoadManager.LoadScene("Splash");   
    }
}
