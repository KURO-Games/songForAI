using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FooterManager : MonoBehaviour
{
    public static int difficluty;
    public static string songName ;

    GameObject songNameText;
    [SerializeField] GameObject[] difficultyImage = new GameObject[4];

    void Start()
    {
        songNameText = GameObject.Find("songNameText");
        songNameText.GetComponent<Text>().text = songName;

        difficultyImage[difficluty].SetActive(true);
    }
}
