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

    int tempNumber;// タップ背景非アクティブ切り替え用
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
        //田村デバッグ
        //if (GOListArray[0][0].transform.position.y<=LeftJudgeLine.transform.position.y)
        //{
        //    Debug.Log("notePos " + GOListArray[_notesCount[0]][0].transform.position.y);
        //    Debug.Log("leftJudgeLinePos " + LeftJudgeLine.transform.position.y);
        //    UnityEditor.EditorApplication.isPaused = true;
        //}

        // レーン背景OFF
        if (Input.GetMouseButtonUp(0))
        {
            TapBG[tempNumber].SetActive(false);
        }

        // マルチタップ対応
        if (0 <= Input.touchCount)
        {
            // タッチされている指の数だけ処理
            for (int i = 0; i < Input.touchCount; i++)
            {
                // タッチ情報をコピー
                UnityEngine.Touch t = Input.GetTouch(i);
                // タッチしたときかどうか
                if (t.phase == TouchPhase.Began)
                {
                    // タップされた時の処理
                    clickObj = null;

                    int layerMask = 1;
                    float maxDistance = 10f;

                    Vector2 mousePosition = Input.mousePosition;

                    Ray ray = Camera.main.ScreenPointToRay(mousePosition);

                    RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, maxDistance, layerMask);
                    if (hit)
                        clickObj = hit.transform.gameObject;

                    if ((clickObj != null) && (clickObj.tag == ("Lane")))// tagでレーンを識別
                    {
                        string s = clickObj.name;  // レーン番号を取得
                        int laneNumber = int.Parse(s);            // 文字列を数字に変換
                        float absTiming = 9999;                   // nullにしたい

                        // レーン背景ON
                        if (Input.GetMouseButtonDown(0))
                        {
                            TapBG[laneNumber].SetActive(true);
                            tempNumber = laneNumber;
                        }

                        //   GOListArray[何個目のノーツなのか[何番目のレーンの]]    [何番目のレーンなのか]
                        if ((GOListArray[_notesCount[laneNumber]][laneNumber] != null) && (laneNumber <= 3))     // 左レーン
                        {
                            absTiming = JudgeDistance(LeftJudgeLine.transform.position.y,
                                                      GOListArray[_notesCount[laneNumber]][laneNumber].transform.position.y);
                        }
                        else if ((GOListArray[_notesCount[laneNumber]][laneNumber] != null) && (laneNumber >= 4)) // 右レーン
                        {
                            absTiming = JudgeDistance(RightJudgeLine.transform.position.y,
                                                      GOListArray[_notesCount[laneNumber]][laneNumber].transform.position.y);
                        }

                        // 判定分岐
                        if (absTiming <= perfect)
                        {
                            point = 300;
                            combo++;
                            totalGrades[0]++;
                            Debug.Log("perfect");
                            JudgeGrade(combo, point, laneNumber);
                            SoundManager.SESoundCue(2);
                        }
                        else if (absTiming <= great)
                        {
                            point = 200;
                            combo++;
                            totalGrades[1]++;
                            Debug.Log("great");
                            JudgeGrade(combo, point, laneNumber);
                            SoundManager.SESoundCue(2);
                        }
                        else if (absTiming <= good)
                        {
                            point = 100;
                            combo = 0;
                            totalGrades[2]++;
                            Debug.Log("good");
                            JudgeGrade(combo, point, laneNumber);
                            SoundManager.SESoundCue(3);
                        }
                        else if (absTiming <= bad)
                        {
                            point = 10;
                            combo = 0;
                            totalGrades[3]++;
                            Debug.Log("bad");
                            JudgeGrade(combo, point, laneNumber);
                            SoundManager.SESoundCue(4);
                        }
                        else// 空タップ
                        {
                            SoundManager.SESoundCue(5);
                        }
                    }
                }
            }
        }

        //if (Input.GetMouseButtonDown(0))
        //{

        //}
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

    // タイミング誤差算出
    private float JudgeDistance(float i, float j)// 判定ライン　－　ノーツ
    {
        float tempTiming = i - j;
        float trueTiming = Mathf.Abs(tempTiming);// 絶対値に変換

        return trueTiming;
    }

    // 判定後処理
    private void JudgeGrade(int combos, int points, int laneNumber)
    {
        if (combos > bestcombo)//最大コンボ記憶
            bestcombo = combos;

        // 各コンボ倍率 switch文にできるかも
        if (combos > 250)
        {
            comboMag = 1.5f;
        }
        else if (combos > 150)
        {
            comboMag = 1.4f;
        }
        else if (combos > 100)
        {
            comboMag = 1.3f;
        }
        else if (combos > 50)
        {
            comboMag = 1.2f;
        }
        else
        {
            comboMag = 1.0f;
        }
        float a = points * comboMag;
        score += (int)a;

        mg1.DrawScore(score);  // スコア表示
        mg2.DrawCombo(combos); // コンボ表示

        Destroy(GOListArray[_notesCount[laneNumber]][laneNumber]);   // ノーツ破棄
        GOListArray[_notesCount[laneNumber]][laneNumber] = null;     // 多重タップを防ぐ
        _notesCount[laneNumber]++;                                   // 該当レーンのノーツカウント++
    }
}