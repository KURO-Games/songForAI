using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// シナリオシーン中のユーザー名入力管理クラス
/// </summary>
public class UserNameInputs : MonoBehaviour
{
    [SerializeField]
    InputField UserNames;
    Scenario_Controller _ScenarioControler;
    private bool onePush = false;
    // Start is called before the first frame update
    private void Start()
    {
        Scene scene = SceneManager.GetSceneByName("Scenario");
        onePush = false;
        foreach (var rootGameObject in scene.GetRootGameObjects())
        {
            _ScenarioControler = rootGameObject.GetComponent<Scenario_Controller>();
            if (_ScenarioControler != null)
            {
                break;
            }
        }
    }
    /// <summary>
    /// 決定ボタンが押されたときの処理
    /// </summary>
    public void pushButton()
    {
        if(UserNames.text.Length < 11)
        {
            if (UserNames.text != "" && !onePush)
            {

                PlayerPrefs.SetString("PlayerName", UserNames.text);
                PlayerPrefs.Save();
                onePush = true;
                for (float i = 1; i >= 0; i -= 0.01f)
                {
                    this.gameObject.GetComponent<CanvasGroup>().alpha = i;
                }
                Scenario_Controller.isUserInputs = false;
                _ScenarioControler.StartCoroutineDisplay();

            }
        }
    }
}
