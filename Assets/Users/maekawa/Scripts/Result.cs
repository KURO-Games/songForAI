using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// シーン遷移
// マルチプラットフォーム対応
// 曲名、総ノーツ数、難易度、楽曲レベル受け渡し
public class Result : MonoBehaviour
{
    // プランナー用ランク基準値 *********************
    // S～Bの順で達成率(%)を入力
    [SerializeField] float[] rankLimit = new float[3];
    // **********************************************


    [SerializeField] GameObject[] grades = new GameObject[5];
    [SerializeField] GameObject score;
    [SerializeField] GameObject bestCombo;
    [SerializeField] GameObject songName;
    [SerializeField] GameObject difficulty;
    [SerializeField] GameObject level;
    [SerializeField] GameObject fullCombo;
    [SerializeField] GameObject rankS;
    [SerializeField] GameObject rankA;
    [SerializeField] GameObject rankB;
    [SerializeField] GameObject rankC;
    //
    public static int maxCombo;       // フルコンボ判定に使用する総ノーツ数
    public static string _songName;   // 曲名表示
    public static int _level;         // 楽曲レベル表示
    public static int _difficulty;    // 難易度 0...easy～3...pro

    private int count;
    private int arrayCount;
    private bool resultAnimFlag;
    void Start()
    {
        // 初期化
        count = 0;
        arrayCount = 0;
        resultAnimFlag = false;

        // ここに曲データを代入


        // 曲名、レベル、難易度表示
        songName.GetComponent<Text>().text = _songName;       // 曲名表示

        string s = String.Format("{0:00}", _level);            // 2ケタ指定
        level.GetComponent<Text>().text = s; ;                // 楽曲レベル表示

        // 難易度表示
        if (_difficulty == 0)
        {
            difficulty.GetComponent<Text>().text = ("Easy");
        }
        else if(_difficulty == 1)
        {
            difficulty.GetComponent<Text>().text = ("Normal");
        }
        else if (_difficulty == 2)
        {
            difficulty.GetComponent<Text>().text = ("Hard");
        }
        else if(_difficulty == 3)
        {
            difficulty.GetComponent<Text>().text = ("Pro");
        }

        // Judgeからスコア、最大コンボ、判定内訳を取得
        int resultScore = Judge.totalScore;
        int resultCombo = Judge.bestcombo;

        score.GetComponent<Text>().text = resultScore.ToString();// スコア表示
        bestCombo.GetComponent<Text>().text = resultCombo.ToString();// コンボ表示

        // フルコンボ表示
        if (resultCombo == maxCombo)
        {
            fullCombo.SetActive(true);
        }

        // スコアに応じてランク表示
        if (ScoreManager.increaseAmount >= rankLimit[0])
        {
            rankS.SetActive(true);
        }
        else if (ScoreManager.increaseAmount >= rankLimit[1])
        {
            rankA.SetActive(true);
        }
        else if (ScoreManager.increaseAmount >= rankLimit[2])
        {
            rankB.SetActive(true);
        }
        else
        {
            rankC.SetActive(true);
        }
    }

    void Update()
    {
        // リザルト表示アニメーション
        if (resultAnimFlag != true)
        {
            if (arrayCount > 4)
            {
                resultAnimFlag = true;// データを表示しきったら終了
            }
            // アニメーション①
            else if ((count <= Judge.totalGrades[arrayCount] / 10) && (Judge.totalGrades[arrayCount] > 10))
            {
                if(count == 0)
                {
                    // 0 + 0～9　と表示させないための処理
                    int tempNum = UnityEngine.Random.Range(0, 9);
                    grades[arrayCount].GetComponent<Text>().text = tempNum.ToString();
                    count++;
                }
                else
                {
                    // 10単位でカウントアップ、1の位はランダムで表示
                    grades[arrayCount].GetComponent<Text>().text = count.ToString() + UnityEngine.Random.Range(0, 9);
                    count++;
                }
            }
            // アニメーション②
            else if ((count <= Judge.totalGrades[arrayCount]) && (Judge.totalGrades[arrayCount] < 10))
            {
                // 表示する値が1桁の場合、普通にカウントアップ
                grades[arrayCount].GetComponent<Text>().text = count.ToString();
                count++;
            }
            // カウントが終了したなら
            else
            {
                grades[arrayCount].GetComponent<Text>().text = Judge.totalGrades[arrayCount].ToString();// 実際の値を表示
                count = 0;   // カウントリセット
                arrayCount++;// 次の配列へ
            }
        }

        // タップでリザルトアニメーションをスキップ
        if (Input.GetMouseButtonDown(0))
        {
            resultAnimFlag = true;

            for (int i = 0; i < grades.Length; i++)
            {
                grades[i].GetComponent<Text>().text = Judge.totalGrades[i].ToString();
            }
        }
    }
}
