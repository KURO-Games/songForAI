using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FooterManager : MonoBehaviour
{
    public static int difficluty;
    public static string musicName ;

    GameObject songNameText;
    [SerializeField] GameObject[] difficultyImage = new GameObject[4];

    void Start()
    {
        // MusicDatas参照
        difficluty = MusicDatas.difficultNumber;
        difficultyImage[difficluty].SetActive(true);

        songNameText = GameObject.Find("songNameText");
        musicName = MusicDatas.MusicName;
        songNameText.GetComponent<Text>().text = musicName;
    }
}
