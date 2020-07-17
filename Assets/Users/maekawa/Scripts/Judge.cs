using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

//ノーツスルー処理
//タップ内訳？

public class Judge : MonoBehaviour
{
    private int score = 0;
    private int combo = 0;
    [SerializeField] private int point = 1000;
    [SerializeField] private float perfect = 15;
    [SerializeField] private float great = 30;
    [SerializeField] private float miss = 40;
    [SerializeField] private GameObject JudgeLine = null;
    //[SerializeField] GameObject note = null;//仮ノーツ
    //[SerializeField] private GameObject[] Lanes = new GameObject[8];

    //[SerializeField] private GameObject[] noTap = new GameObject[8];
    private List<GameObject>[] GOListArray = new List<GameObject>[8];
    private int[] notesCount = { 0, 0, 0, 0, 0, 0, 0, 0 };

    //float GapDistance()//距離を算出
    //{
    //    float tapTiming = JudgeLine.transform.position.y - note.transform.position.y; //仮ノーツ
    //    float absTiming = Mathf.Abs(tapTiming);//絶対値に変換
    //    return absTiming;
    //}

    //ノーツが通り過ぎたら
    void OnTriggerEnter2D(Collider2D collider2D)
    {
        string i = collider2D.gameObject.name;//ヒットしたオブジェクトの名前を取得
        int laneNumber = int.Parse(i);//文字列を数字に変換
        combo = 0;
        notesCount[laneNumber]++;
    }

    void Update()//判定
    {

        if (Input.GetMouseButtonDown(0))
        {
            int layerMask = 1;
            float maxDistance = 10;

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, maxDistance, layerMask);

            if (hit.collider)// レーンをタップしたら
            {
                //Debug.Log("ノーツy座標" + notes.transform.position.y);//
                //Debug.Log("判定ラインy座標" + JudgeLine.transform.position.y);//
                //Debug.Log("ノーツx座標" + note.transform.position.x);//
                //Debug.Log("レーンx座標" + hit.collider.transform.position.x);//

                JudgeLine = hit.collider.gameObject;

                string i = hit.collider.gameObject.name;//ヒットしたオブジェクトの名前を取得
                int laneNumber = int.Parse(i);//文字列を数字に変換

                float tapTiming = JudgeLine.transform.position.y - GOListArray[laneNumber][notesCount[laneNumber]].transform.position.y;
                float absTiming = Mathf.Abs(tapTiming);//絶対値に変換

                //判定分岐
                if (absTiming <= perfect)
                {
                    combo++;
                    score += point;
                    Debug.Log("コンボ" + combo);
                    Debug.Log("Perfect スコア" + score);
                    notesCount[laneNumber]++;
                }
                else if (absTiming <= great)
                {
                    combo++;
                    score += point / 2;
                    Debug.Log("コンボ" + combo);
                    Debug.Log("Great　スコア" + score);
                    notesCount[laneNumber]++;
                }
                else if (absTiming <= miss)
                {
                    combo = 0;
                    notesCount[laneNumber]++;
                }
            }
        }
    }
}
