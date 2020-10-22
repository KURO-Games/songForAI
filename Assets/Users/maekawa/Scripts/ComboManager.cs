using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.ResourceManagement.Util;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
    [SerializeField] Sprite[] comboNum = new Sprite[10];// 0～9の数字画像
    //[SerializeField] float width; // 数字の表示間隔
    [SerializeField] GameObject comboImage;
    [SerializeField] GameObject[] combo;

    int[] digit = new int[4];// 要素数 = 桁数
    int tempCombo;

    private void Start()
    {
        for(int i = 0; i < combo.Length;i++)
        {
            combo[i].SetActive(false);
        }

        comboImage.SetActive(false);
        tempCombo = 0;
    }
    /// <summary>
    /// 引数:コンボ
    /// </summary>
    /// <param name="a">combo</param>
    public void DrawCombo(int a)
    {
        //現在のスコアを破棄
        //var nums = GameObject.FindGameObjectsWithTag("ComboNum");
        //foreach (var num in nums)
        //{
        //    if (0 <= num.name.LastIndexOf("Clone"))
        //    {
        //        Destroy(num);
        //    }
        //}

        comboImage.SetActive(true);
        tempCombo = a;

        for (int i = 0; i < digit.Length; i++)// 桁数分だけループ
        {
            combo[i].SetActive(true);

            digit[i] = a % 10;// 1の位を取り出す

            //RectTransform comboAnchor = (RectTransform)Instantiate(obj).transform;
            //comboAnchor.SetParent(this.transform, false);
            //comboAnchor.localPosition = new Vector2(obj.GetComponent<RectTransform>().localPosition.x - width * i, obj.GetComponent<RectTransform>().localPosition.y);// widthの数値分 - x
            //comboAnchor.GetComponent<Image>().sprite = comboNum[digit[i]];// i桁目の数字を配置
            //comboAnchor.transform.parent = _combo.transform;

            combo[i].transform.GetChild(0).GetComponent<Image>().sprite = comboNum[digit[i]];
            a /= 10; //次のループに入るため、1の位を切り落とす
        }

        if (tempCombo < 1000)
        {
            combo[3].SetActive(false);
        }
        if (tempCombo < 100)
        {
            combo[2].SetActive(false);
        }
        if (tempCombo < 10)
        {
            combo[1].SetActive(false);
        }
        if (tempCombo < 5)
        {
            combo[0].SetActive(false);
            comboImage.SetActive(false);
        }
    }
}
