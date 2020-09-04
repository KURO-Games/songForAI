using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Judge : MonoBehaviour
{
    public static int[] totalGrades = { 0, 0, 0, 0, 0 };          // リザルト画面用の判定内訳
    public static int score = 0;                                  // スコア
    public static int point = 0;                                  // グレード毎の得点
    public static int combo = 0;                                  // コンボ
    public static float comboMag = 1.0f;                          // コンボに応じたスコア倍率
    public static int bestcombo = 0;                              // 最大コンボ
    private static int[] _notesCount = { 0, 0, 0, 0, 0, 0, 0, 0 };// レーンごとのノーツカウント
    private int laneNumber = 0;
    private float absTiming = 0;
    private static List<List<GameObject>> GOListArray = new List<List<GameObject>>();// ノーツ座標格納用2次元配列

    [SerializeField] private GameObject LeftJudgeLine;  // 右判定基準
    [SerializeField] private GameObject RightJudgeLine; // 左判定基準
    [SerializeField] private GameObject[] TapBG = new GameObject[8];//

    // 判定許容値**************************************
    [SerializeField] private float perfect;
    [SerializeField] private float great;
    [SerializeField] private float good;
    [SerializeField] private float bad;
    // **************************************判定許容値

    bool[] tapFlag = { false, false, false, false, false, false, false, false };// タップ背景非アクティブ切り替え用
    bool[] lastTap = { false, false, false, false, false, false, false, false };// 前フレームのタップ
    GameObject uiObj;
    ScoreManager mg1;
    ComboManager mg2;
    GameObject clickObj;

    private void Start()
    {
        // 関数を呼ぶためにスクリプトをセット
        uiObj = GameObject.Find("UICtrlCanvas");
        mg1 = uiObj.GetComponent<ScoreManager>();
        mg2 = uiObj.GetComponent<ComboManager>();
        mg1.DrawScore(score);// デフォルトでスコアのみ表示
    }

    //タップ判定処理
    void Update()
    {
        // tapFlag初期化
        for(int i = 0; i < tapFlag.Length; i++)
        {
            tapFlag[i] = false;
        }

        // マルチタップ対応
        if (0 <= Input.touchCount)
        {
            // タッチされている指の数だけ処理
            for (int i = 0; i < Input.touchCount; i++)
            {
                // タップしたレーンを取得
                laneNumber = GetLaneNumber(i);

                tapFlag[laneNumber] = true;

            }
        }

        for(int i = 0; i < tapFlag.Length; i++)
        {
            if(lastTap[i] == true && tapFlag[i] == true)
            {

            }
            else if (lastTap[i] == false && tapFlag[i] == true)
            { 
                // 判定ライン - ノーツで距離を算出
                absTiming = GetAbsTiming(i);

                // 距離に応じて判定処理
                JudgeGrade(absTiming, i);
                TapBG[i].SetActive(true);
            }
            if (lastTap[i] == true && tapFlag[i] == false)
            {
                TapBG[i].SetActive(false);
            }
        }
        lastTap = tapFlag; 
    }

    public static void ListImport()
    {
        GOListArray = NotesManager.NotesPositions;
        Debug.Log(GOListArray[0][7]);
    }

    // ノーツ通過処理
    public static void NotesCountUp(string i)
    {
        if (combo > bestcombo)// 最大コンボ記憶
            bestcombo = combo;

        combo = 0;
        totalGrades[4]++;

        int tempLaneNum = int.Parse(i);// 文字列を数字に変換
        Destroy(GOListArray[_notesCount[tempLaneNum]][tempLaneNum]);   // ノーツ破棄
        GOListArray[_notesCount[tempLaneNum]][tempLaneNum] = null;     // 多重タップを防ぐ
        _notesCount[tempLaneNum]++;                                    // 該当レーンのノーツカウント++

        // コンボ描画処理はNotesCountUpスクリプトで行う
    }

    private int GetLaneNumber(int i)
    {
        // タッチ情報をコピー
        UnityEngine.Touch t = Input.GetTouch(i);
        // タッチしたときかどうか
        if (t.phase == TouchPhase.Began)
        {
            // タップされた時の処理
            clickObj = null;

            Ray ray = Camera.main.ScreenPointToRay(t.position);

            RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10f, 1);
            if (hit)
                clickObj = hit.transform.gameObject;

            if ((clickObj != null) && (clickObj.tag == ("Lane")))// tagでレーンを識別
            {
                string s = clickObj.name;  // レーン番号を取得
                laneNumber = int.Parse(s);            // 文字列を数字に変換
            }
        }

        return laneNumber;
    }

    // タイミング誤差算出
    private float GetAbsTiming(int i)// 判定ライン　－　ノーツ
    {
        float tempTiming = 9999;// nullにしたい

        //   GOListArray[何個目のノーツなのか[何番目のレーンの]]    [何番目のレーンなのか]
        if ((GOListArray[_notesCount[i]][i] != null) && (i <= 3))      // 左レーン
        {
            tempTiming = LeftJudgeLine.transform.position.y -
                                        GOListArray[_notesCount[i]][i].transform.position.y;
        }
        else if ((GOListArray[_notesCount[i]][i] != null) && (i >= 4)) // 右レーン
        {
            tempTiming = RightJudgeLine.transform.position.y -
                                        GOListArray[_notesCount[i]][i].transform.position.y;
        }

        float trueTiming = Mathf.Abs(tempTiming);// 絶対値に変換

        return trueTiming;
    }

    /// <summary>
    /// 判定後処理
    /// </summary>
    /// <param name="combos">コンボ</param>
    /// <param name="points"></param>
    /// <param name="laneNumber"></param>
    private void JudgeGrade(float i, int j)
    {
        // 判定分岐
        if (i <= perfect)
        {
            point = 300;
            combo++;
            totalGrades[0]++;
            Debug.Log("perfect");
            SoundManager.SESoundCue(2);
        }
        else if (i <= great)
        {
            point = 200;
            combo++;
            totalGrades[1]++;
            Debug.Log("great");
            SoundManager.SESoundCue(2);
        }
        else if (i <= good)
        {
            point = 100;
            combo = 0;
            totalGrades[2]++;
            Debug.Log("good");
            SoundManager.SESoundCue(3);
        }
        else if (i <= bad)
        {
            point = 10;
            combo = 0;
            totalGrades[3]++;
            Debug.Log("bad");
            SoundManager.SESoundCue(4);
        }
        else// 空タップ
        {
            SoundManager.SESoundCue(5);
        }

        /////////////////////////////////////////////
        
        if (combo > bestcombo)//最大コンボ記憶
            bestcombo = combo;

        // 各コンボ倍率 switch文にできるかも
        if (combo > 250)
        {
            comboMag = 1.5f;
        }
        else if (combo > 150)
        {
            comboMag = 1.4f;
        }
        else if (combo > 100)
        {
            comboMag = 1.3f;
        }
        else if (combo > 50)
        {
            comboMag = 1.2f;
        }
        else
        {
            comboMag = 1.0f;
        }

        float scores = point * comboMag;
        score += (int)scores;

        mg1.DrawScore(score);  // スコア表示
        mg2.DrawCombo(combo); // コンボ表示

        Destroy(GOListArray[_notesCount[j]][j]);   // ノーツ破棄
        GOListArray[_notesCount[j]][j] = null;     // 多重タップを防ぐ
        _notesCount[j]++;                          // 該当レーンのノーツカウント++
    }
}