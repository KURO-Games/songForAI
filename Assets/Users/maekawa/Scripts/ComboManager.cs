using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ComboManager : MonoBehaviour
{
    [SerializeField] Sprite[] comboNum = new Sprite[10];// 0～9の数字画像
    [SerializeField] GameObject comboImage;
    [SerializeField] GameObject combo_digit1;// 1桁の場合使用
    [SerializeField] GameObject[] combo_digit2 = new GameObject[3];// 2桁の場合使用
    [SerializeField] GameObject[] combo_digit3 = new GameObject[3];// 3桁の場合使用

    private int[] digit = new int[3];// 要素数 = 桁数
    private int tempCombo;

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



        // 桁ごとに取り出し配列に代入
        tempCombo = a;
        for (int i = 0; i < digit.Length; i++)// 桁数分だけループ
        {
            digit[i] = tempCombo % 10;// 1の位を取り出す
            tempCombo /= 10; //次のループに入るため、1の位を切り落とす
            combo_digit3[i].transform.GetComponent<Image>().sprite = comboNum[digit[i]];
            combo_digit2[i].transform.GetComponent<Image>().sprite = comboNum[digit[i]];
        }
        combo_digit1.transform.GetComponent<Image>().sprite = comboNum[digit[0]];


        // 桁に応じて表示するUIを切り替え
        comboImage.SetActive(true);
        combo_digit3[0].transform.parent.gameObject.SetActive(false);
        combo_digit2[0].transform.parent.gameObject.SetActive(false);
        combo_digit1.transform.parent.gameObject.SetActive(false);
        if (a > 99)
            combo_digit3[0].transform.parent.gameObject.SetActive(true);
        else if (a > 9)
            combo_digit2[0].transform.parent.gameObject.SetActive(true);
        else if (a > 4)
            combo_digit1.transform.parent.gameObject.SetActive(true);
        else
            comboImage.SetActive(false);
    }
}
