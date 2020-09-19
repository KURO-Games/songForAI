using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

// 最大スコアを参照してゲージ描画

public class ScoreManager : MonoBehaviour
{
    GameObject scoreText;
    GameObject scoreGauge;
    //[SerializeField] Sprite[] scoreNum = new Sprite[10];// 0～9の数字画像
    //[SerializeField] float width; // 数字の表示間隔
    //[SerializeField] GameObject[] _score;

    //int[] digit = new int[7];// 要素数 = 桁数

    void Start()
    {
        scoreText = GameObject.Find("scoreText");
        scoreGauge = GameObject.Find("scoreGauge");
    }

    /// <summary>
    /// スコア、スコアゲージの描画を行います
    /// </summary>
    /// <param name="i">totalScore</param>
    /// <param name="j">point</param>
    /// <param name="k">maxCombo</param>
    public void DrawScore(int i, int j)
    {
        string s = i.ToString();
        s = String.Format("{0:0000000}", i);// 7ケタ表示

        scoreText.GetComponent<Text>().text = s;

        // ここに最大コンボを入力
        scoreGauge.GetComponent<Image>().fillAmount += (float)j / (Judge.gradePoint[0] * 300);

        float a = j / (Judge.gradePoint[0] * 300);
        Debug.Log(a);
        //現在のスコアを破棄
        //var nums = GameObject.FindGameObjectsWithTag("ScoreNum");
        //foreach (var num in nums)
        //{
        //    if (0 <= num.name.LastIndexOf("Clone"))
        //    {
        //        Destroy(num);
        //    }
        //}

        //for (int i = 0; i < digit.Length; i++)// 桁数分だけループ
        //{
        //    digit[i] = a % 10;// 1の位を取り出す

        //    //RectTransform comboAnchor = (RectTransform)Instantiate(GameObject.Find("ScoreAnchor")).transform;
        //    //comboAnchor.SetParent(this.transform, false);
        //    //comboAnchor.localPosition = new Vector2(comboAnchor.localPosition.x - width * i, comboAnchor.localPosition.y);// widthの数値分 - x
        //    //comboAnchor.GetComponent<Image>().sprite = scoreNum[digit[i]];// i桁目の数字を配置
        //    _score[i].GetComponent<Image>().sprite = scoreNum[digit[i]];
        //    a /= 10; //次のループに入るため、1の位を切り落とす
        //}
    }
}
