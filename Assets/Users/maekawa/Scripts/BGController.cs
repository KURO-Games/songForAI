using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BGController : MonoBehaviour
{
    [SerializeField] Image backGround;

    void Start()
    {
        Sprite[] jacketSprite = Resources.LoadAll<Sprite>("BackGround/");// ジャケットをすべて格納
        if (jacketSprite.Length > MusicDatas.MusicNumber)// 例外処理
            backGround.sprite = jacketSprite[MusicDatas.MusicNumber];// 曲１ = musicNum 0;
    }
}
