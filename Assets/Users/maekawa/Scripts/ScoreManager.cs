using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

// 最大スコアを参照してゲージ描画

public class ScoreManager : MonoBehaviour
{
    public static float      increaseAmount; // スコアゲージ増加量
    private       int        lastScore;      // totalScoreとの比較用
    public static int        maxScore;       // 内部用天井点
    private       GameObject scoreTextObj;
    private       GameObject scoreGaugeObj;
    private       Text       scoreText;
    private       Image      scoreGaugeImg;
    //[SerializeField] Sprite[] scoreNum = new Sprite[10];// 0～9の数字画像
    //[SerializeField] float width; // 数字の表示間隔
    //[SerializeField] GameObject[] _score;

    //int[] digit = new int[7];// 要素数 = 桁数

    private void Start()
    {
        increaseAmount = 0;
        lastScore = 0;

        scoreTextObj  = GameObject.Find("scoreText");
        scoreGaugeObj = GameObject.Find("scoreGauge");
        scoreText     = scoreTextObj.GetComponent<Text>();
        scoreGaugeImg = scoreGaugeObj.GetComponent<Image>();
    }

    /// <summary>
    /// スコア、スコアゲージの描画を行います
    /// </summary>
    /// <param name="i">totalScore</param>
    public void DrawScore(int i)
    {
        // スコア表示
        string s = $"{i:0000000}"; // 7ケタ表示
        scoreText.text = s;

        // スコアゲージ増加
        int points = i - lastScore;// 前回のスコアとの差で増えたポイントを算出
        lastScore = i;// 今回のスコアを記憶

        increaseAmount += (float)points / maxScore;// ポイント / 天井点　でゲージ増加量を算出
        scoreGaugeImg.fillAmount = increaseAmount;

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
