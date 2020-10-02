﻿using System.Collections;
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
    [SerializeField]
    int LifeNum = 0;
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
        //PlayerPrefs.SetInt("Lifes", 2);
        LifeNum = PlayerPrefs.GetInt("Lifes");
        DifficultsNum = -1;
        _isTap = false;
        MusicDatas.cueMusic = -1;
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
    private void LifeDraw()
    {
        for (int i = 0; i < Lifes.Length; i++)
            Lifes[i].gameObject.SetActive(false);
        for (int i = 0; i < LifeNum ; i++)
            Lifes[i].gameObject.SetActive(true);


    }



}
