using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectMusicScene : MonoBehaviour
{
    string _name;
    Button PushButton;
    bool _isTap = false;
    private void Start()
    {
        _isTap = false;
    }
    public void BackHome()
    {
        if (!_isTap)
        {
            _isTap = true;
            SceneLoadManager.LoadScene("MockHome");
        }
    }
    public void SelectMusic(Button _button)
    {
        if (!_isTap)
        {
            _isTap = true;
            _name=_button.name;
            UnityEngine.SceneManagement.SceneManager.LoadScene("SelectCaracter");
        }
    }

}
