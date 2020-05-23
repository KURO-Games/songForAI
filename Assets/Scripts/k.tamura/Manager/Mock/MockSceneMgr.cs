using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MockSceneMgr : SingletonMonoBehaviour<MockSceneMgr>
{
    private void Start()
    {
        DontDestroyOnLoad(this);
    }

    public static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

}
