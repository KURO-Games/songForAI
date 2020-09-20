using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// シーン遷移
// マルチプラットフォーム対応
// 曲名、総ノーツ数、難易度、楽曲レベル受け渡し
public class Result : MonoBehaviour
{
    // 表示する各テキストをインスペクターでアタッチ
    // gradesにはperfect～missの順番でアタッチ
    [SerializeField] GameObject[] grades = new GameObject[5];
    [SerializeField] GameObject score;
    [SerializeField] GameObject maxCombo;
    [SerializeField] GameObject songName;
    [SerializeField] GameObject Difficulty;
    [SerializeField] GameObject Level;
    [SerializeField] GameObject fullCombo;
    [SerializeField] GameObject S_sprite;
    [SerializeField] GameObject A_sprite;
    [SerializeField] GameObject B_sprite;
    [SerializeField] GameObject C_sprite;

    // 各ランク基準
    [SerializeField] int RankS_Rimit;
    [SerializeField] int RankA_Rimit;
    [SerializeField] int RankB_Rimit;
    //
    public static int totalNotes;       // フルコンボ判定に使用する総ノーツ数
    public static string resultSongName;// 曲名表示
    public static int level;            // 楽曲レベル表示
    public static int difficulty;       // 難易度 0...easy～3...pro

    private int count = 0;
    private int arrayCount = 0;
    private bool resultAnimFlag = false;
    void Start()
    {
        // 曲名、難易度、レベル表示
        songName.GetComponent<Text>().text = resultSongName;// 曲名表示
        Level.GetComponent<Text>().text = level.ToString(); ;// 楽曲レベル表示

        // 難易度表示
        if (difficulty == 0)
        {
            Difficulty.GetComponent<Text>().text = ("Easy");
        }
        else if(difficulty == 1)
        {
            Difficulty.GetComponent<Text>().text = ("Normal");
        }
        else if (difficulty == 2)
        {
            Difficulty.GetComponent<Text>().text = ("Hard");
        }
        else if(difficulty == 3)
        {
            Difficulty.GetComponent<Text>().text = ("Pro");
        }

        // Judgeからスコア、最大コンボ、判定内訳を取得
        int resultScore = Judge.totalScore;
        int resultCombo = Judge.bestcombo;

        score.GetComponent<Text>().text = resultScore.ToString();// スコア表示
        maxCombo.GetComponent<Text>().text = resultCombo.ToString();// コンボ表示

        // フルコンボ表示
        if (resultCombo == totalNotes)
        {
            fullCombo.SetActive(true);
        }

        // スコアに応じてランク表示
        if (resultScore >= RankS_Rimit)
        {
            S_sprite.SetActive(true);
        }
        else if(resultScore >= RankA_Rimit)
        {
            A_sprite.SetActive(true);
        }
        else if(resultScore >= RankB_Rimit)
        {
            B_sprite.SetActive(true);
        }
        else 
        {
            C_sprite.SetActive(true);
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
                    int tempNum = Random.Range(0, 9);
                    grades[arrayCount].GetComponent<Text>().text = tempNum.ToString();
                    count++;
                }
                else
                {
                    // 10単位でカウントアップ、1の位はランダムで表示
                    grades[arrayCount].GetComponent<Text>().text = count.ToString() + Random.Range(0, 9);
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

        if(Input.GetMouseButtonDown(0))
        {
            SceneLoadManager.LoadScene("Home");
        }
    }
}
