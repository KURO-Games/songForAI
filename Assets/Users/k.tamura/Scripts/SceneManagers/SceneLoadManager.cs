using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// シーン遷移Manager
/// </summary>
public class SceneLoadManager : MonoBehaviour
{
    #region Singleton
        private static SceneLoadManager instance = null;
        public static SceneLoadManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindObjectOfType<SceneLoadManager>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject(typeof(SceneLoadManager).Name);
                        instance = obj.AddComponent<SceneLoadManager>();
                    }
                }
                return instance;
            }
        }

        void Awake()
        {
            if (CheckInstance())
            {
                DontDestroyOnLoad(this);
            }
        }

        bool CheckInstance()
        {
            if (instance == null)
            {
                instance = this;
                return true;
            }
            else if (Instance == this)
            {
                return true;
            }

            enabled = false;
            DestroyImmediate(gameObject);
            return false;
        }
    #endregion


    static string nextScene = "";
    public static string NextScene { get { return nextScene; } }

    static bool isFading = false;
    float fadeAlpha = 0;

    public float fadeTime = 2f;
    public Color fadeColor = Color.white;

    IEnumerator fadeOut;
    IEnumerator fadeIn;
    /// <summary>
    /// 画面反転用
    /// </summary>
    public void OnGUI()
    {
        if (isFading)
        {
            fadeColor.a = fadeAlpha;
            GUI.color = fadeColor;
            GUI.DrawTexture(new Rect(0, 0, Screen.width, Screen.height), Texture2D.whiteTexture);
        }
    }
    /// <summary>
    /// 次のシーンを呼ぶ
    /// </summary>
    /// <param name="sceneName">呼ぶシーンの名前を指定する</param>
    public static void LoadScene(string sceneName)
    {
        nextScene = sceneName;
        Instance.StartCoroutine(Instance.FadeOutScene(Instance.fadeTime, () =>
                {
                    SceneManager.LoadScene("Loading");
                }
            )
        );
    }
    /// <summary>
    /// FadeOut
    /// </summary>
    /// <param name="time"></param>
    /// <param name="callback"></param>
    public static void FadeOut(float time, System.Action callback = null)
    {
        if (Instance.fadeIn != null) Instance.StopCoroutine(Instance.fadeIn);
        Instance.fadeOut = Instance.FadeOutScene(time, callback);
        Instance.StartCoroutine(Instance.fadeOut);
    }/// <summary>
    /// FadeOut
    /// </summary>
    /// <param name="callback"></param>
    public static void FadeOut(System.Action callback)
    {
        FadeOut(Instance.fadeTime, callback);
    }
    /// <summary>
    /// FadeOut
    /// </summary>
    public static void FadeOut()
    {
        FadeOut(Instance.fadeTime);
    }
    /// <summary>
    /// FadeIn
    /// </summary>
    /// <param name="time"></param>
    /// <param name="callback"></param>
    public static void FadeIn(float time, System.Action callback = null)
    {
        if (Instance.fadeOut != null) Instance.StopCoroutine(Instance.fadeOut);
        Instance.fadeIn = Instance.FadeInScene(time, callback);
        Instance.StartCoroutine(Instance.fadeIn);
    }
    /// <summary>
    /// FadeIn
    /// </summary>
    /// <param name="callback"></param>
    public static void FadeIn(System.Action callback)
    {
        FadeIn(Instance.fadeTime, callback);
    }
    /// <summary>
    /// FadeIn
    /// </summary>
    /// <param name="time"></param>
    public static void FadeIn(float time)
    {
        FadeIn(time, null);
    }
    /// <summary>
    /// FadeIn
    /// </summary>
    public static void FadeIn()
    {
        FadeIn(Instance.fadeTime);
    }
    /// <summary>
    /// FadeOut
    /// </summary>
    /// <param name="interval"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    IEnumerator FadeOutScene(float interval, System.Action callback)
    {
        isFading = true;
        float time = 0;
        while (time <= interval)
        {
            fadeAlpha = Mathf.Lerp(0f, 1f, time / interval);
            time += Time.unscaledDeltaTime;
            yield return 0;
        }

        if (callback != null)
        {
            callback();
        }
    }
    /// <summary>
    /// FadeIn
    /// </summary>
    /// <param name="interval"></param>
    /// <param name="callback"></param>
    /// <returns></returns>
    IEnumerator FadeInScene(float interval, System.Action callback)
    {
        isFading = true;
        float time = 0;
        while (time <= interval)
        {
            fadeAlpha = Mathf.Lerp(1f, 0f, time / interval);
            time += Time.unscaledDeltaTime;
            yield return 0;
        }

        if (callback != null)
        {
            callback();
        }
        isFading = false;
    }

}
