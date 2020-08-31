using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectMusicScene : MonoBehaviour
{
    string _name;
    Button PushButton;
    bool _isTap = false;
    public static int DifficultsNum;
    [SerializeField]
    private Image[] ChooseHighlight;
    private enum Difficults
    {
        EASY,
        NORMAL,
        HARD,
        PRO
    }
    private int[] Level = {3,7,12,16 };
    private void Start()
    {
        DifficultsNum = -1;
        _isTap = false;
        MusicDatas.cueMusic = -1;
    }
    public void BackHome()
    {
        if (!_isTap)
        {
            _isTap = true;
            SceneLoadManager.LoadScene("Home");
        }
    }
    public void SelectMusic(Button _button)
    {
        if (!_isTap&&DifficultsNum!=-1)
        {
            _isTap = true;
            _name=_button.name;
            UnityEngine.SceneManagement.SceneManager.LoadScene("SelectCaracter");
        }
    }
    public void PushDifficult(int i)
    {
        DifficultsNum = i;
        MusicDatas.difficultNumber = i;
        MusicDatas.difficultLevel = Level[i];
        Highlight();
    }
    private void Highlight()
    {
        for(int i=0;i<ChooseHighlight.Length;i++)
        {
            ChooseHighlight[i].gameObject.SetActive(false);
        }
        ChooseHighlight[DifficultsNum].gameObject.SetActive(true);
    }
    public void PButton(int i)
    {
        MusicDatas.cueMusic = i;
    }


}
