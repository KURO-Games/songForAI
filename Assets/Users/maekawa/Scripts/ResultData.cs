using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

// シーン遷移
// マルチプラットフォーム対応
// 曲名、総ノーツ数、難易度、楽曲レベル受け渡し
public class ResultData : MonoBehaviour
{
    // 表示する各テキストをインスペクターでアタッチ
    // gradesにはperfect～missの順番でアタッチ
    [SerializeField] GameObject[] grades = new GameObject[5];
    [SerializeField] GameObject scoreText;
    [SerializeField] GameObject comboText;
    [SerializeField] GameObject songNameText;
    [SerializeField] GameObject DifficultyText;
    [SerializeField] GameObject LevelText;
    [SerializeField] GameObject fullComboText;
    //[SerializeField] GameObject clear;// 試遊会では無し
    [SerializeField] GameObject S_sprite;// 各ランク画像
    [SerializeField] GameObject A_sprite;
    [SerializeField] GameObject B_sprite;
    [SerializeField] GameObject C_sprite;

    // 各ランク基準
    [SerializeField] int RankS_Rimit;
    [SerializeField] int RankA_Rimit;
    [SerializeField] int RankB_Rimit;

    public static int totalNotes;       // フルコンボ判定に使用する総ノーツ数
    public static string resultSongName;// 曲名表示
    public static int level;            // 楽曲レベル表示
    public static int difficulty;       // 難易度 0...easy～3...pro

    // リザルト画面データ表示
    void Start()
    {
        // 曲名、難易度、レベル表示
        songNameText.GetComponent<Text>().text = resultSongName;// 曲名表示
        LevelText.GetComponent<Text>().text = level.ToString(); ;// 楽曲レベル表示

        // 難易度表示
        if (difficulty == 0)
        {
            DifficultyText.GetComponent<Text>().text = ("Easy");
        }
        else if(difficulty == 1)
        {
            DifficultyText.GetComponent<Text>().text = ("Normal");
        }
        else if (difficulty == 2)
        {
            DifficultyText.GetComponent<Text>().text = ("Hard");
        }
        else if(difficulty == 3)
        {
            DifficultyText.GetComponent<Text>().text = ("Pro");
        }

        // Judgeからスコア、最大コンボ、判定内訳を取得
        int resultScore = Judge.score;
        int resultCombo = Judge.bestcombo;
        int[] resultGrades = Judge.totalGrades;
        scoreText.GetComponent<Text>().text = resultScore.ToString();// スコア表示
        comboText.GetComponent<Text>().text = resultCombo.ToString();// コンボ表示

        // 判定内訳表示
        for(int i = 0; i < grades.Length; i++)
        {
            grades[i].GetComponent<Text>().text = resultGrades[i].ToString();
        }

        // フルコンボ表示
        if (resultCombo == totalNotes)
        {
            fullComboText.SetActive(true);
        }
        //else
        //{
        //    clear.SetActive(true);
        //}

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
        
    }
}
