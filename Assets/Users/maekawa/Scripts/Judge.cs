using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//タップ内訳？(最大コンボ)
//判定ライン若干上かも　コライダー位置調整など

public class Judge : MonoBehaviour
{
    public static int score = 0;
    public static int combo = 0;
    [SerializeField] private int point;
    [SerializeField] private float perfect;
    [SerializeField] private float great;
    [SerializeField] private float miss;
    [SerializeField] private GameObject LeftJudgeLine;
    [SerializeField] private GameObject RightJudgeLine;
    private static List<List<GameObject>> GOListArray = new List<List<GameObject>>();
    private static int[] _notesCount = { 0, 0, 0, 0, 0, 0, 0, 0 };

    //タップ判定処理
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int layerMask = 1;
            float maxDistance = 10f;

            Vector2 mousePosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, maxDistance, layerMask);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == ("Lane"))//レーンクリックでレーン番号を取得
                {
                    string i = hit.collider.gameObject.name;//ヒットしたオブジェクトの名前を取得
                    int laneNumber = int.Parse(i);//文字列を数字に変換

                    if (laneNumber <= 3)// 左レーン
                    {
                        //  GOListArray[何個目のノーツなのか[何番目のレーンの]][何番目のレーンなのか]
                        if (GOListArray[_notesCount[laneNumber]][laneNumber] != null)
                        {
                            float absTiming = JudgeDistance_L(GOListArray[_notesCount[laneNumber]][laneNumber].transform.position.y);

                            //判定分岐
                            if (absTiming <= perfect)
                                hoge(1, point, laneNumber);
                            else if (absTiming <= great)
                            {
                                combo++;
                                score += point / 2;
                                Debug.Log("コンボ" + combo);
                                Debug.Log("Great　スコア" + score);

                                Destroy(GOListArray[_notesCount[laneNumber]][laneNumber]);
                                GOListArray[_notesCount[laneNumber]][laneNumber] = null;
                                _notesCount[laneNumber]++;
                            }
                            else if (absTiming <= miss)
                            {
                                combo = 0;
                                Debug.Log("miss");

                                Destroy(GOListArray[_notesCount[laneNumber]][laneNumber]);
                                GOListArray[_notesCount[laneNumber]][laneNumber] = null;
                                _notesCount[laneNumber]++;
                            }
                        }
                    }
                    else// 右レーン
                    {
                        if (GOListArray[_notesCount[laneNumber]][laneNumber] != null)
                        {
                            float absTiming = JudgeDistance_R(GOListArray[_notesCount[laneNumber]][laneNumber].transform.position.y);

                            //判定分岐
                            if (absTiming <= perfect)
                            {
                                combo++;
                                score += point;
                                Debug.Log("コンボ" + combo);
                                Debug.Log("Perfect スコア" + score);

                                Destroy(GOListArray[_notesCount[laneNumber]][laneNumber]);
                                GOListArray[_notesCount[laneNumber]][laneNumber] = null;
                                _notesCount[laneNumber]++;
                            }
                            else if (absTiming <= great)
                            {
                                combo++;
                                score += point / 2;
                                Debug.Log("コンボ" + combo);
                                Debug.Log("Great　スコア" + score);

                                Destroy(GOListArray[_notesCount[laneNumber]][laneNumber]);
                                GOListArray[_notesCount[laneNumber]][laneNumber] = null;
                                _notesCount[laneNumber]++;
                            }
                            else if (absTiming <= miss)
                            {
                                combo = 0;
                                Debug.Log("miss");

                                Destroy(GOListArray[_notesCount[laneNumber]][laneNumber]);
                                GOListArray[_notesCount[laneNumber]][laneNumber] = null;
                                _notesCount[laneNumber]++;
                            }
                        }
                    }
                }
            }
        }
    }

    public static void ListImport()
    {
        GOListArray = NotesManager.NotesPositions;
        Debug.Log(GOListArray[0][7]);
    }

    /// <summary>
    /// 左レーンとノーツの距離算出
    /// </summary>
    /// <param name="i">ノーツのポジション</param>
    /// <returns></returns>
    float JudgeDistance_L(float i)// 左レーン距離算出
    {
        float tempTiming = LeftJudgeLine.transform.position.y - i;
        float trueTiming = Mathf.Abs(tempTiming);//絶対値に変換

        return trueTiming;
    }

    float JudgeDistance_R(float i)// 右レーン距離算出
    {
        float tempTiming = RightJudgeLine.transform.position.y - i;
        float trueTiming = Mathf.Abs(tempTiming);//絶対値に変換

        //Debug.Log("JUDGEPOINT" + RightJudgeLine.transform.position.y);
        //Debug.Log("NOTES" + i);
        //Debug.Log("TRUETIMING" + trueTiming);

        return trueTiming;
    }

    public static void NotesCountUp(string i)// ノーツ通過処理
    {
        int tempLaneNum = int.Parse(i);//文字列を数字に変換
        combo = 0;
        Debug.Log("NotesCountUp_miss");
        _notesCount[tempLaneNum]++;
    }
    private void hoge(int combos, int points, int laneNumber)
    {
        combo+=combos;
        score += points;
        Debug.Log("コンボ" + combo);
        Debug.Log("Perfect スコア" + score);

        Destroy(GOListArray[_notesCount[laneNumber]][laneNumber]);// ノーツ破棄
        GOListArray[_notesCount[laneNumber]][laneNumber] = null;// 多重タップを防ぐ
        _notesCount[laneNumber]++;//ノーツ識別カウントアップ
    }
}