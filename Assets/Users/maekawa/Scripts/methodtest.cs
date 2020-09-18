using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class methodtest : MonoBehaviour
{
    public static int Test(int i)
    {
        int laneNum = -1;// 例外処理用
        GameObject clickObj = null; // 都度初期化

        // タッチ情報を取得
        Touch t = Input.GetTouch(i);

        // タップ時処理
        Ray ray = Camera.main.ScreenPointToRay(t.position);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10f, 1);


        if (hit)
        {
            Debug.Log("ok");
            clickObj = hit.transform.gameObject;
            Debug.Log("clear");
            if ((clickObj != null) && (clickObj.tag == ("Lane")))// tagでレーンを識別
            {
                string s = clickObj.name;  // レーン番号を取得
                laneNum = int.Parse(s);    // 文字列を数字に変換
            }
        }

        return laneNum;
    }
}
