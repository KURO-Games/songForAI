using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score_Controller : MonoBehaviour
{
    private int Score;          //合計得点
    private int add_Score;      //通常追加得点
    private int combo;          //コンボ数
    private int combo_Score;    //コンボボーナス
    private int perfect_Score;  //パーフェクトボーナス

    //数字スプライト
    [SerializeField]
    Sprite[] Score_sprite;  //0~9の画像格納
    [SerializeField]
    Sprite[] Combo_sprite;  //0~9の画像格納

    //イメージ
    [SerializeField]
    Image[] Score_Image;    //{十万の桁、万の桁、千の桁、百の桁、十の桁、一の桁}
    [SerializeField]
    Image[] Combo_Image;    //{百の桁、十の桁、一の桁}

    //各桁の数字確認用
    private int[] Score_Num = new int[6] { 0, 0, 0, 0, 0, 0 };  //{十万の桁、万の桁、千の桁、百の桁、十の桁、一の桁}
    private int[] Combo_Num = new int[3] { 0, 0, 0 };           //{百の桁、十の桁、一の桁}


    void Start()
    {
        Score = 0;
        combo = 0;
        combo_Score = 0;
        //今は数値が決まってないので適当に
        add_Score = 1;
        perfect_Score = 5;
    }

    #region ノーツ判定(これを呼んで使ってほしい)

    //完璧
    public void Perfect()
    {
        Add_Combo();
        Perfect_Add_Score();
        Score_Display();
        Combo_Display();
    }

    //成功
    public void Hit()
    {
        Add_Combo();
        Add_Score();
        Score_Display();
        Combo_Display();
    }
    //失敗
    public void Miss()
    {
        Reset_Combo();
        Combo_Display();
    }

    #endregion

    #region 得点関連

    //コンボ処理
    private void Add_Combo()
    {
        combo++;
        combo_Score += combo;
    }
    private void Reset_Combo()
    {
        combo = 0;
        combo_Score = 0;
    }

    //ポイント追加処理
    private void Add_Score()
    {
        Score += add_Score + combo_Score;
    }
    private void Perfect_Add_Score()
    {
        Score += perfect_Score + combo_Score;
    }

    #endregion

    #region 表示処理

    private void Score_Display()
    {
        #region AddScore　その1
        //点数を確認
        /*#region 100000の位
        Score_Num[5] = Score / 100000;
        #endregion

        #region 10000の位
        Score_Num[4] = (Score - (Score_Num[5] * 100000)) / 10000;
        #endregion

        #region 1000の位
        Score_Num[3] = (Score - (Score_Num[5] * 100000 + Score_Num[4] * 10000)) / 1000;
        #endregion

        #region 100の位
        Score_Num[2] = (Score - (Score_Num[5] * 100000 + Score_Num[4] * 10000 + Score_Num[3] * 1000)) / 100;
        #endregion

        #region 10の位
        Score_Num[1] = (Score - (Score_Num[5] * 100000 + Score_Num[4] * 10000 + Score_Num[3] * 1000 + Score_Num[2] * 100)) / 10;
        #endregion

        #region 1の位
        Score_Num[0] = (Score - (Score_Num[5] * 100000 + Score_Num[4] * 10000 + Score_Num[3] * 1000 + Score_Num[2] * 100 + Score_Num[1] * 10));
        #endregion*/
        //適切なスプライトを選択してイメージに反映
        /*Score_Image[5].sprite = Score_sprite[Score_Num[5]];
        Score_Image[4].sprite = Score_sprite[Score_Num[4]];
        Score_Image[3].sprite = Score_sprite[Score_Num[3]];
        Score_Image[2].sprite = Score_sprite[Score_Num[2]];
        Score_Image[1].sprite = Score_sprite[Score_Num[1]];
        Score_Image[0].sprite = Score_sprite[Score_Num[0]];*/
        #endregion

        #region AddScore　その2
        //それぞれの桁の数字を確認し適切なスプライトを選択
        for (var score_num = 5; score_num >= 0; score_num--)
        {
            var score_del = 0;        //引く
            var score_div = 100000;   //割る
            var score_del_Num = 5;    //呼び出すスコアnum
            //それぞれの桁の数字を確認
            for (var score_rep = 4 - score_num; score_rep >= 0; score_rep--)
            {
                score_del += Score_Num[score_del_Num] * score_div;
                score_div /= 10;
                score_del_Num--;
            }
            Score_Num[score_num] = (Score - score_del) / score_div;
            //適切なスプライトを選択してイメージに反映
            Score_Image[score_num].sprite = Score_sprite[Score_Num[score_num]];
        }

        #endregion
    }

    private void Combo_Display()
    {
        //コンボを確認
        #region 100の位
        Combo_Num[2] = combo / 100;
        Debug.Log(Combo_Num[2]);
        #endregion

        #region 10の位
        Combo_Num[1] = (combo - (Combo_Num[2] * 100)) / 10;
        Debug.Log(Combo_Num[1]);
        #endregion

        #region 1の位
        Combo_Num[0] = (combo - (Combo_Num[2] * 100 + Combo_Num[1] * 10));
        Debug.Log(Combo_Num[0]);
        #endregion
        //適切なスプライトを選択してイメージに反映
        Combo_Image[2].sprite = Combo_sprite[Combo_Num[2]];
        Combo_Image[1].sprite = Combo_sprite[Combo_Num[1]];
        Combo_Image[0].sprite = Combo_sprite[Combo_Num[0]];
    }

    #endregion
}
