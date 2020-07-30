using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagTest : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            int layerMask = 1;
            float maxDistance = 10f;

            Vector2 mousePosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, maxDistance, layerMask);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == ("Lane"))//レーンをクリックしたらレーン番号を取得
                {
                    string i = hit.collider.gameObject.name;//ヒットしたオブジェクトの名前を取得
                    int laneNumber = int.Parse(i);//文字列を数字に変換
                    Debug.Log(laneNumber);
                }
            }
        }
    }
}
//string i = hit.collider.gameObject.tag;//ヒットしたオブジェクトの名前を取得
//string[] notesNum = i.Split('_');// 引数の文字で分割して配列化
//int laneNumber = int.Parse(notesNum[1]);//文字列を数字に変換
