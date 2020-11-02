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
    [SerializeField]
    private GameObject[] Lifes;
    //[SerializeField]
    //int LifeNum = 0;

    public static int life = 2;
    private enum Difficults
    {
        EASY,
        NORMAL,
        HARD,
        PRO
    }
    private int[] Level = {3,7,12,16 };// 後で変更
    private void Start()
    {
        //PlayerPrefs.SetInt("Lifes", 2);
        //LifeNum = PlayerPrefs.GetInt("Lifes", 3);

        // DifficultsNum = -1;

        // デフォルトでproを表示
        SoundManager.BGMStop();
        PushDifficult(0);
        _isTap = false;
        MusicDatas.cueMusic = 0;
        LifeDraw();
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
            SceneLoadManager.LoadScene("Resize_RhythmGame2");
        }
    }
    public void PushDifficult(int i)
    {
        DifficultsNum = i;
        MusicDatas.difficultNumber = i;
        MusicDatas.difficultLevel = Level[i];// 変数化する
        Highlight();
    }
    private void Highlight()
    {
        for(int i=0;i<ChooseHighlight.Length;i++)
        {
            ChooseHighlight[i].gameObject.SetActive(false);
        }
        ChooseHighlight[DifficultsNum].gameObject.SetActive(true);
        MusicDatas.difficultNumber = DifficultsNum;
    }
    public void PButton(int i)
    {
        MusicDatas.cueMusic = i;
    }
    private void LifeDraw()
    {
        for (int i = 0; i < Lifes.Length; i++)
            Lifes[i].gameObject.SetActive(false);
        for (int i = 0; i < life; i++)
            Lifes[i].gameObject.SetActive(true);
    }
}
