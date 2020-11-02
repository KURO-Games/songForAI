using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawJacket : MonoBehaviour
{
    [SerializeField] Image jacket;
    [SerializeField] Text musicName;

    void Start()
    {
        Sprite[] jacketSprite = Resources.LoadAll<Sprite>("Jacket/");// ジャケットをすべて格納
        if (jacketSprite.Length > MusicDatas.MusicNumber)// 例外処理
            jacket.sprite = jacketSprite[MusicDatas.MusicNumber];// 曲１ = musicNum 0;
        musicName.text = MusicDatas.MusicName;
    }
}
