using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//ノーツスルー処理
//タップ内訳？

public class Judge : MonoBehaviour
{
    private static int score = 0;
    private static int combo = 0;
    [SerializeField] private int point;
    [SerializeField] private float perfect;
    [SerializeField] private float great;
    [SerializeField] private float miss;
    [SerializeField] private GameObject LeftJudgeLine;
    [SerializeField] private GameObject RightJudgeLine;
    [SerializeField] private GameObject[] IgnoreDetection = new GameObject[8];
    private static List<List<GameObject>> GOListArray = new List<List<GameObject>>();
    private static int[] _notesCount = { 0, 0, 0, 0, 0, 0, 0, 0 };

    //ノーツ通過処理
    public static void NotesCountUp(string i)//mainで呼ばなくていい
    {
        //string i = col.gameObject.name;//ヒットしたオブジェクトの名前を取得
        Debug.Log(i);
        int tempLaneNum = int.Parse(i);//文字列を数字に変換
        combo = 0;
        Debug.Log("miss");
        _notesCount[tempLaneNum]++;
    }

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

                    if(laneNumber <= 3)// 左レーン
                    {
                        if (GOListArray[_notesCount[laneNumber]][laneNumber] != null)
                        {
                            // ノーツのy座標を取得　                                GOListArray[何個目のノーツなのか[何番目のレーンの]][何番目のレーンなのか]
                            float tapTiming = LeftJudgeLine.transform.position.y - GOListArray[_notesCount[laneNumber]][laneNumber].transform.position.y;
                            float absTiming = Mathf.Abs(tapTiming);//絶対値に変換

                            //判定分岐
                            if (absTiming <= perfect)
                            {
                                combo++;
                                score += point;
                                Debug.Log("コンボ" + combo);
                                Debug.Log("Perfect スコア" + score);
                                _notesCount[laneNumber]++;
                            }
                            else if (absTiming <= great)
                            {
                                combo++;
                                score += point / 2;
                                Debug.Log("コンボ" + combo);
                                Debug.Log("Great　スコア" + score);
                                _notesCount[laneNumber]++;
                            }
                            else if (absTiming <= miss)
                            {
                                combo = 0;
                                Debug.Log("miss");
                                _notesCount[laneNumber]++;
                            }
                        }
                    }
                    else// 右レーン
                    {
                        if (GOListArray[_notesCount[laneNumber]][laneNumber] != null)
                        {
                            // ノーツのy座標を取得　                                GOListArray[何個目のノーツなのか[何番目のレーンの]][何番目のレーンなのか]
                            float tapTiming = RightJudgeLine.transform.position.y - GOListArray[_notesCount[laneNumber]][laneNumber].transform.position.y;
                            float absTiming = Mathf.Abs(tapTiming);//絶対値に変換

                            //判定分岐
                            if (absTiming <= perfect)
                            {
                                combo++;
                                score += point;
                                Debug.Log("コンボ" + combo);
                                Debug.Log("Perfect スコア" + score);
                                _notesCount[laneNumber]++;
                            }
                            else if (absTiming <= great)
                            {
                                combo++;
                                score += point / 2;
                                Debug.Log("コンボ" + combo);
                                Debug.Log("Great　スコア" + score);
                                _notesCount[laneNumber]++;
                            }
                            else if (absTiming <= miss)
                            {
                                combo = 0;
                                Debug.Log("miss");
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
}