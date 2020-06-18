using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadingScene : MonoBehaviour
{
    [SerializeField]
    private AsyncOperation async;
    [SerializeField]GameObject error;//エラーUIを指定
    //[SerializeField]
    //private GameObject loadUI;
    //[SerializeField]
    //private Slider slider;
    string nextScene;//次のシーン
    bool _error;//シーン遷移ができるかどうか
    /// <summary>
    /// 次のシーンに遷移
    /// </summary>
    public void NextScene()
    {
        //loadUI.SetActive(true);
        StartCoroutine(Loaddata());
    }
    /// <summary>
    /// シーン再生中にSceneManager.LoadSceneAsyncで次のシーンを呼ぶ
    /// </summary>
    /// <returns></returns>
    IEnumerator Loaddata()
    {
        Time.timeScale = 1.0f;
        SceneLoadManager.FadeIn();

        nextScene = SceneLoadManager.NextScene;
        async = SceneManager.LoadSceneAsync(nextScene);
        if (async != null)
        {
            while (!async.isDone)
            {
                var progressVal = Mathf.Clamp01(async.progress / 0.9f);
                //slider.value = progressVal;
                yield return null;
            }
        }
        else
        {
            Debug.LogError("シーンが見つかりません。");
            _error = true;
        }
    }
    // Use this for initialization
    void Start()
    {
        StartCoroutine(Loaddata());
    }
    // Update is called once per frame
    void Update()
    {
        if (_error)
        {
            error.SetActive(true);
        }
    }
}
