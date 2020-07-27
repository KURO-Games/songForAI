using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//ノーツスルー処理
//タップ内訳？
//該当レーン以外の例外処理

public class Judge : MonoBehaviour
{
    private int score = 0;
    private int combo = 0;
    [SerializeField] private int point = 1000;
    [SerializeField] private float perfect = 15;
    [SerializeField] private float great = 30;
    [SerializeField] private float miss = 40;
    [SerializeField] private GameObject JudgeLine = null;

    private static List<List<GameObject>> GOListArray = new List<List<GameObject>>();
    private int[] _notesCount = { 0, 0, 0, 0, 0, 0, 0, 0 };

    //ノーツ通過処理
    void OnTriggerEnter2D(Collider2D collider2D)//mainで呼ばなくていい
    {
        string i = collider2D.gameObject.name;//ヒットしたオブジェクトの名前を取得
        int laneNumber = int.Parse(i);//文字列を数字に変換
        combo = 0;
        _notesCount[laneNumber]++;
    }

    int GetLaneNumber()
    {
        int laneNumber = 0;

        int layerMask = 1;
        float maxDistance = 10;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, maxDistance, layerMask);

        if (hit.collider.gameObject.tag == "Lane")
        {

            string i = hit.collider.gameObject.tag;//ヒットしたオブジェクトの名前を取得
            laneNumber = int.Parse(i);//文字列を数字に変換
            Debug.Log(laneNumber);
        }

        return laneNumber;
    }

    //タップ判定処理
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int layerMask = 1;
            float maxDistance = 10;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, maxDistance, layerMask);

            if (hit.collider)// レーンをタップしたら
            {
                JudgeLine = hit.collider.gameObject;

                //if (RaycastHit2D.collider.gameObject.tag == "Lane")
                //{
                //    //string i = hit.collider.gameObject.tag;//ヒットしたオブジェクトの名前を取得
                //    //string[] notesNum = i.Split('_');// 引数の文字で分割して配列化
                //    //int laneNumber = int.Parse(notesNum[1]);//文字列を数字に変換
                //}

                // ズレを算出
                ListImport();

                int laneNumber = GetLaneNumber();
                // ノーツのy座標を取得　                           GOListArray[何個目のノーツなのか[何番目のレーンの]][何番目のレーンなのか]
                float tapTiming = JudgeLine.transform.position.y - GOListArray[_notesCount[laneNumber]][laneNumber].transform.position.y;
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
                    _notesCount[laneNumber]++;
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
