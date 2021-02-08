using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityScript.Steps;

// gameTypeに応じてキャラクター表示
public class Result : MonoBehaviour
{
    // プランナー用ランク基準値
    // S～Bの順で達成率(%)を入力
    [SerializeField] float[] rankLimit = new float[3];

    [SerializeField] GameObject[] grades = new GameObject[5];
    [SerializeField] GameObject[] difficulty = new GameObject[4];
    [SerializeField] GameObject[] character = new GameObject[3];
    [SerializeField] GameObject score;
    [SerializeField] GameObject maxCombo;
    [SerializeField] GameObject songName;
    [SerializeField] GameObject level;
    [SerializeField] GameObject fullCombo;
    [SerializeField] GameObject rankS;
    [SerializeField] GameObject rankA;
    [SerializeField] GameObject rankB;
    [SerializeField] GameObject rankC;
    [SerializeField] Image jacket;
    //
    //public static int gameType;
    //public static int charaNum = 0;// キャラ表示用
    //
    private int count;
    private int arrayCount;
    private int rankNum;
    private bool scoreAnimeFlag;
    private bool comboAnimeFlag;
    private bool gradesAnimeFlag;
    public static bool isEnded;// リザルトアニメーション全体の終了

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

    public static bool isClick;// ボタン用
    void Start()
    {
        SelectMusicScene.life--;
        isClick = true;

        // 初期化
        resultIncrease = 0;
        count = 0;
        arrayCount = 0;
        rankNum = 0;// 1=C～4=S
        scoreAnimeFlag = false;
        comboAnimeFlag = false;
        gradesAnimeFlag = false;
        isEnded = false;
        SoundManager.LoadAsyncCueSheet("Result", SoundType.BGM);
        SoundManager.BGMSoundCue();
        // 曲名表示
        songName.GetComponent<Text>().text = MusicDatas.MusicName;// 曲名表示;

        // レベル表示
        string s = String.Format("{0:00}", MusicDatas.difficultLevel);// 2ケタ指定
        level.GetComponent<Text>().text = s;

        for(int i = 0; i < 4; i++)
        {
            difficulty[i].SetActive(false);
        }

        // 難易度表示
        switch(MusicDatas.difficultNumber)
        {
            case 0:
                difficulty[0].SetActive(true);
                break;
            case 1:
                difficulty[1].SetActive(true);
                break;
            case 2:
                difficulty[2].SetActive(true);
                break;
            case 3:
                difficulty[3].SetActive(true);
                break;
            default:
                break;
        }

        // フルコンボ表示
        //if (NotesJudgementBase.bestCombo == MusicDatas.allNotes)
        //{
        //    fullCombo.SetActive(true);
        //}

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
        scoreGauge.GetComponent<Image>().fillAmount = 0;

        SaveHighScores();

        // ジャケット表示
        Sprite[] jacketSprite = Resources.LoadAll<Sprite>("Sprite/UI/Result/resultJacket");// ジャケットをすべて格納
        if (jacketSprite.Length > MusicDatas.MusicNumber)// 例外処理
            jacket.sprite = jacketSprite[MusicDatas.MusicNumber];// 曲１ = musicNum 0;

        // キャラ表示
        character[(int)MusicDatas.gameType].SetActive(true);
    }

    void Update()
    {
        int   bestCombo   = NotesJudgementBase.bestCombo;
        int[] totalGrades = NotesJudgementBase.TotalGrades;
        int   totalScore  = NotesJudgementBase.totalScore;

        //timeCount += Time.deltaTime;
        ////if(timeCount >= 1.5)
        //{
        //    if (x > 250 && hoge != true)
        //    {
        //        x -= 175;
        //        y -= 175;
        //        if(x < 250)
        //        {
        //            hoge = true;
        //        }
        //    }
        //    else if(x <290)
        //    {
        //        x += 50;
        //        y += 50;
        //    }
        //    rankImage.GetComponent<Image>().color = new Color(255, 255, 255, alpha);
        //    if (alpha < 1)
        //    {
        //        alpha += 0.1f;
        //    }
        //    rankRect.sizeDelta = new Vector2(x, y);
        //}

        // リザルト表示アニメーション
        // スコアアニメーション
        if (scoreAnimeFlag != true)
        {
            if (count < totalScore / 100)
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
                score.GetComponent<Text>().text = String.Format("{0:0000000}", totalScore);
                count                           = 0; // カウントリセット
                scoreAnimeFlag                  = true;
            }
        }
        // コンボアニメーション
        else if(comboAnimeFlag != true)
        {
            if (count <= bestCombo / 10 && bestCombo >= 11)
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
            else if (count <= bestCombo && bestCombo <= 10)
            {
                maxCombo.GetComponent<Text>().text = count.ToString();
                count++;
            }
            else
            {
                maxCombo.GetComponent<Text>().text = bestCombo.ToString();
                count                              = 0;
                comboAnimeFlag                     = true;
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
            else if ((count <= totalGrades[arrayCount] / 10) && (totalGrades[arrayCount] >= 11))
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
            else if ((count <= totalGrades[arrayCount]) && (totalGrades[arrayCount] <= 10))
            {
                // 表示する値が1桁の場合、普通にカウントアップ
                grades[arrayCount].GetComponent<Text>().text = count.ToString();
                count++;
            }
            // カウントが終了したなら
            else
            {
                grades[arrayCount].GetComponent<Text>().text = totalGrades[arrayCount].ToString(); // 実際の値を表示
                count                                        = 0;                                  // カウントリセット
                arrayCount++;                                                                      // 次の配列へ
            }
        }

        if (scoreAnimeFlag && resultIncrease < ScoreManager.increaseAmount)
        {
            resultIncrease += 0.02f;
            scoreGauge.GetComponent<Image>().fillAmount = resultIncrease;
        }


        // タップでリザルトアニメーションをスキップ
        if (Input.GetMouseButtonDown(0))
        {
            // スコア表示
            string s = string.Format("{0:0000000}", totalScore); // 7ケタ指定
            score.GetComponent<Text>().text = s;

            // コンボ表示
            maxCombo.GetComponent<Text>().text = bestCombo.ToString();

            // グレード内訳表示
            for (int i = 0; i < grades.Length; i++)
            {
                grades[i].GetComponent<Text>().text = totalGrades[i].ToString();
            }

            scoreAnimeFlag = true;
            comboAnimeFlag = true;
            gradesAnimeFlag = true;
        }

        // ポップアップ表示
        if (scoreAnimeFlag && comboAnimeFlag && gradesAnimeFlag)
        {
            isEnded = true;
        }
    }

    private void SaveHighScores()
    {
        int bestCombo  = NotesJudgementBase.bestCombo;
        int totalScore = NotesJudgementBase.totalScore;

        //if (PlayerPrefsUtil<int>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
        //    MusicDatas.NotesDataName, (int) MusicDatas.gameType, MusicDatas.difficultNumber,ScoreClass.PlayerPrefsHighScore), 0) < totalScore)
        //{
        //    PlayerPrefsUtil<int>.Save(string.Format(ScoreClass.PlayerPrefsFormat,
        //        MusicDatas.NotesDataName, (int) MusicDatas.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsHighScore), totalScore);
        //}
        //if (PlayerPrefsUtil<int>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
        //    MusicDatas.NotesDataName, (int) MusicDatas.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsMaxCombo), 0) < bestCombo)
        //{
        //    PlayerPrefsUtil<int>.Save(string.Format(ScoreClass.PlayerPrefsFormat,
        //        MusicDatas.NotesDataName, (int) MusicDatas.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsMaxCombo), bestCombo);
        //}
        //if (PlayerPrefsUtil<int>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
        //    MusicDatas.NotesDataName, (int) MusicDatas.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsHighRank), 0) < rankNum)
        //{
        //    PlayerPrefsUtil<int>.Save(string.Format(ScoreClass.PlayerPrefsFormat,
        //        MusicDatas.NotesDataName, (int) MusicDatas.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsHighRank), rankNum);
        //}

        if (PlayerPrefs.GetInt(string.Format(ScoreClass.PlayerPrefsFormat,
            MusicDatas.NotesDataName, (int) MusicDatas.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsHighScore), 0) < totalScore)
        {
            PlayerPrefs.SetInt(string.Format(ScoreClass.PlayerPrefsFormat,
                MusicDatas.NotesDataName, (int) MusicDatas.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsHighScore), totalScore);
        }
        if (PlayerPrefs.GetInt(string.Format(ScoreClass.PlayerPrefsFormat,
            MusicDatas.NotesDataName, (int) MusicDatas.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsMaxCombo), 0) < bestCombo)
        {
            PlayerPrefs.SetInt(string.Format(ScoreClass.PlayerPrefsFormat,
                MusicDatas.NotesDataName, (int) MusicDatas.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsMaxCombo), bestCombo);
        }
        if (PlayerPrefs.GetInt(string.Format(ScoreClass.PlayerPrefsFormat,
            MusicDatas.NotesDataName, (int) MusicDatas.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsHighRank), 0) < rankNum)
        {
            PlayerPrefs.SetInt(string.Format(ScoreClass.PlayerPrefsFormat,
                MusicDatas.NotesDataName, (int) MusicDatas.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsHighRank), rankNum);
        }
    }
}
