using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
    [SerializeField] GameObject combo;
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
    //
    int[] resultGrades = { 0, 0, 0, 0, 0 };// リザルト判定内訳格納

    private int count = 0;
    private int counta = 0;
    private bool flag = true;
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
        int resultScore = Judge.score;
        int resultCombo = Judge.bestcombo;

        for(int i = 0; i < resultGrades.Length; i++)
        {
            resultGrades[i] = Judge.totalGrades[i];
        }

        score.GetComponent<Text>().text = resultScore.ToString();// スコア表示
        combo.GetComponent<Text>().text = resultCombo.ToString();// コンボ表示

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
        // 判定内訳表示
        if ((flag) && (count <= resultGrades[counta] / 10))
        {
            grades[counta].GetComponent<Text>().text = count.ToString() + Random.Range(0, 9);
            count++;
        }
        else
        {
            grades[counta].GetComponent<Text>().text = resultGrades[counta].ToString();
            count = 0;
            counta++;

            if (counta > resultGrades.Length)
            {
                flag = false;
            }
        }
    }

    // シーン遷移時データ破棄
}
