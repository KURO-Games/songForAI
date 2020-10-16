﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityScript.Steps;

// gameTypeに応じてキャラクター表示
public class Result : MonoBehaviour
{
    // プランナー用ランク基準値 *********************
    // S～Bの順で達成率(%)を入力
    [SerializeField] float[] rankLimit = new float[3];
    // **********************************************


    [SerializeField] GameObject[] grades = new GameObject[5];
    [SerializeField] GameObject score;
    [SerializeField] GameObject maxCombo;
    [SerializeField] GameObject songName;
    [SerializeField] GameObject difficulty;
    [SerializeField] GameObject level;
    [SerializeField] GameObject fullCombo;
    [SerializeField] GameObject rankS;
    [SerializeField] GameObject rankA;
    [SerializeField] GameObject rankB;
    [SerializeField] GameObject rankC;
    //
    public static int gameType;
    //
    private int count;
    private int arrayCount;
    private int rankNum;
    private bool scoreAnimeFlag;
    private bool comboAnimeFlag;
    private bool gradesAnimeFlag;

    //
    GameObject scoreGauge;
    Image rankImage;
    RectTransform rankRect;
    float x = 1500;
    float y = 1500;
    float alpha = 0.3f;
    float resultIncrease = 0;
    float timeCount;
    bool hoge = false;
    bool foo = false;

    void Start()
    {
        // 初期化
        count = 0;
        arrayCount = 0;
        rankNum = 0;// 1=C～4=S
        scoreAnimeFlag = false;
        comboAnimeFlag = false;
        gradesAnimeFlag = false;

        // 曲名表示
        songName.GetComponent<Text>().text = MusicDatas.MusicName;  // 曲名表示

        // レベル表示
        string s = String.Format("{0:00}", MusicDatas.difficultLevel);       // 2ケタ指定
        level.GetComponent<Text>().text = s;

        // 難易度表示
        switch(MusicDatas.difficultNumber)
        {
            case 0:
                difficulty.GetComponent<Text>().text = ("Easy");
                break;
            case 1:
                difficulty.GetComponent<Text>().text = ("Normal");
                break;
            case 2:
                difficulty.GetComponent<Text>().text = ("Hard");
                break;
            case 3:
                difficulty.GetComponent<Text>().text = ("Pro");
                break;
        }

        // フルコンボ表示
        if (Judge.bestCombo == MusicDatas.allNotes)
        {
            fullCombo.SetActive(true);
        }

        // スコアに応じてランク表示
        string callObj;

        if (ScoreManager.increaseAmount >= rankLimit[0])
        {
            rankS.SetActive(true);
            callObj = "S";
            rankNum = 4;
        }
        else if (ScoreManager.increaseAmount >= rankLimit[1])
        {
            rankA.SetActive(true);
            callObj = "A";
            rankNum = 3;
        }
        else if (ScoreManager.increaseAmount >= rankLimit[2])
        {
            rankB.SetActive(true);
            callObj = "B";
            rankNum = 2;
        }
        else
        {
            rankC.SetActive(true);
            callObj = "C";
            rankNum = 1;
        }

        rankImage = GameObject.Find(callObj).GetComponent<Image>();
        rankRect = GameObject.Find(callObj).GetComponent<RectTransform>();

        scoreGauge = GameObject.Find("scoreGauge");
        SaveHighScores();
    }

    void Update()
    {
        timeCount += Time.deltaTime;
        //if(timeCount >= 1.5)
        {
            if (x > 250 && hoge != true)
            {
                x -= 175;
                y -= 175;
                if(x < 250)
                {
                    hoge = true;
                }
            }
            else if(x <290)
            {
                x += 50;
                y += 50;
            }
            rankImage.GetComponent<Image>().color = new Color(255, 255, 255, alpha);
            if (alpha < 1)
            {
                alpha += 0.1f;
            }
            rankRect.sizeDelta = new Vector2(x, y);
        }
        

        // リザルト表示アニメーション
        // スコアアニメーション
        if (scoreAnimeFlag != true)
        {
            if (count < Judge.totalScore / 100)
            {
                if (count == 0)
                {
                    // 0 + 0～9　と表示させないための処理
                    int tempA = UnityEngine.Random.Range(0, 9);
                    int tempB = UnityEngine.Random.Range(0, 9);
                    string i = tempA.ToString() + tempB.ToString();

                    int tempInt = int.Parse(i);
                    score.GetComponent<Text>().text = String.Format("{0:0000000}", tempInt);
                    count++;
                }
                else
                {
                    int tempA = UnityEngine.Random.Range(0, 9);
                    int tempB = UnityEngine.Random.Range(0, 9);
                    string i = count.ToString() + tempA.ToString() + tempB.ToString();

                    int tempInt = int.Parse(i);

                    score.GetComponent<Text>().text = String.Format("{0:0000000}", tempInt);
                    count++;
                }
            }
            else
            {
                score.GetComponent<Text>().text = String.Format("{0:0000000}", Judge.totalScore);
                count = 0;   // カウントリセット
                scoreAnimeFlag = true;
            }
        }
        // コンボアニメーション
        else if(comboAnimeFlag != true)
        {
            if (count <= Judge.bestCombo / 10 && Judge.bestCombo >= 11)
            {
                if (count == 0)
                {
                    // 0 + 0～9　と表示させないための処理
                    int tempNum = UnityEngine.Random.Range(0, 9);
                    maxCombo.GetComponent<Text>().text = tempNum.ToString();
                    count++;
                }
                else
                {
                    maxCombo.GetComponent<Text>().text = count.ToString() + UnityEngine.Random.Range(0, 9);
                    count++;
                }
            }
            else if (count <= Judge.bestCombo && Judge.bestCombo <= 10)
            {
                maxCombo.GetComponent<Text>().text = count.ToString();
                count++;
            }
            else
            {
                maxCombo.GetComponent<Text>().text = Judge.bestCombo.ToString();
                count = 0;
                comboAnimeFlag = true;
            }
        }
        // グレード内訳アニメーション
        else if(gradesAnimeFlag != true)
        {
            if (arrayCount > 4)
            {
                gradesAnimeFlag = true;// データを表示しきったら終了
            }
            // アニメーション①
            else if ((count <= Judge.totalGrades[arrayCount] / 10) && (Judge.totalGrades[arrayCount] >= 11))
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
            else if ((count <= Judge.totalGrades[arrayCount]) && (Judge.totalGrades[arrayCount] <= 10))
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

        if (scoreAnimeFlag == true && resultIncrease < ScoreManager.increaseAmount)
        {
            resultIncrease += 0.001f;
            scoreGauge.GetComponent<Image>().fillAmount += resultIncrease;
        }
        
        // 2020/10/13 Edited Sakamaki ------------

        

        //------------------------------------------------------------

        // タップでリザルトアニメーションをスキップ
        if (Input.GetMouseButtonDown(0))
        {
            // スコア表示
            string s = string.Format("{0:0000000}", Judge.totalScore);// 7ケタ指定
            score.GetComponent<Text>().text = s;

            // コンボ表示
            maxCombo.GetComponent<Text>().text = Judge.bestCombo.ToString();

            // グレード内訳表示
            for (int i = 0; i < grades.Length; i++)
            {
                grades[i].GetComponent<Text>().text = Judge.totalGrades[i].ToString();
            }

            scoreAnimeFlag = true;
            comboAnimeFlag = true;
            gradesAnimeFlag = true;
        }
    }
    private void SaveHighScores()
    {
        int intScore = Judge.totalScore;
        int intMaxcombo = Judge.bestCombo;

        // string HIGH_SCORE,HIGH_MAXCOMBOが今までの数値を超えていたらifで分岐しスコアセーブ
        // ハイスコア習得
        if (PlayerPrefsUtil<int>.Load(string.Format(ScoreClass.PlayerPrefsFormat, 
            MusicDatas.NotesDataName, Judge.gameType, MusicDatas.difficultNumber,ScoreClass.PlayerPrefsHighScore)) < intScore)
        {
            PlayerPrefsUtil<int>.Save(string.Format(ScoreClass.PlayerPrefsFormat, 
                MusicDatas.NotesDataName, Judge.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsHighScore), intScore);
        }
        // マックスコンボ習得
        if (PlayerPrefsUtil<int>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
            MusicDatas.NotesDataName, Judge.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsMaxCombo)) < intMaxcombo)
        {
            PlayerPrefsUtil<int>.Save(string.Format(ScoreClass.PlayerPrefsFormat, 
                MusicDatas.NotesDataName, Judge.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsMaxCombo), intMaxcombo);
        }
        // rankNum追加
        if (PlayerPrefsUtil<int>.Load(string.Format(ScoreClass.PlayerPrefsFormat, 
            MusicDatas.NotesDataName, Judge.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsHighRank)) < rankNum)
        {
            PlayerPrefsUtil<int>.Save(string.Format(ScoreClass.PlayerPrefsFormat, 
                MusicDatas.NotesDataName, Judge.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsHighRank), rankNum);
        }
    }
}
