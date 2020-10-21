using System.Collections;
using System.Collections.Generic;
using UnityEditor.iOS;
using UnityEngine;
using UnityEngine.UI;

public class DrawJacket : MonoBehaviour
{
    [SerializeField] Image jacket;
    [SerializeField] Text musicName;
    int musicNum;

    void Start()
    {
        Sprite[] jacketSprite = Resources.LoadAll<Sprite>("Jacket/");// ジャケットをすべて格納
        if(jacketSprite.Length > MusicDatas.MusicNumber)// 例外処理
        jacket.sprite = jacketSprite[musicNum];// 曲１ = musicNum 0;


        musicName.text = MusicDatas.MusicName;
    }
}
