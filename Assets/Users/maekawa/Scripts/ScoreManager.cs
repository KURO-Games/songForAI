using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] Sprite[] scoreNum = new Sprite[10];// 0～9の数字画像
    //[SerializeField] float width; // 数字の表示間隔
    [SerializeField] GameObject[] _score;

    int[] digit = new int[7];// 要素数 = 桁数

    /// <summary>
    /// 引数:スコア
    /// </summary>
    /// <param name="a"></param>
    public void DrawScore(int a)
    {
        //現在のスコアを破棄
        //var nums = GameObject.FindGameObjectsWithTag("ScoreNum");
        //foreach (var num in nums)
        //{
        //    if (0 <= num.name.LastIndexOf("Clone"))
        //    {
        //        Destroy(num);
        //    }
        //}

        for (int i = 0; i < digit.Length; i++)// 桁数分だけループ
        {
            digit[i] = a % 10;// 1の位を取り出す

            //RectTransform comboAnchor = (RectTransform)Instantiate(GameObject.Find("ScoreAnchor")).transform;
            //comboAnchor.SetParent(this.transform, false);
            //comboAnchor.localPosition = new Vector2(comboAnchor.localPosition.x - width * i, comboAnchor.localPosition.y);// widthの数値分 - x
            //comboAnchor.GetComponent<Image>().sprite = scoreNum[digit[i]];// i桁目の数字を配置
            _score[i].GetComponent<Image>().sprite = scoreNum[digit[i]];
            a /= 10; //次のループに入るため、1の位を切り落とす
        }
    }
}
