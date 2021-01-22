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
        SoundManager.LoadAsyncCueSheet(SoundDefine.Title, SoundType.BGM);
    }
    [RuntimeInitializeOnLoadMethod]
    static void Init()
    {
        GameObject obj = (GameObject)Resources.Load("SoundManager");
        GameObject instance = (GameObject)Instantiate(
            obj, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
    }
}
