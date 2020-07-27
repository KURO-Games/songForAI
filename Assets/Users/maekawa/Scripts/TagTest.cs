using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagTest : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        int layerMask = 1;
        float maxDistance = 10;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, maxDistance, layerMask);

        if (hit.collider.gameObject.tag == "Lane")
        {
            string i = hit.collider.gameObject.tag;//ヒットしたオブジェクトの名前を取得
            int laneNumber = int.Parse(i);//文字列を数字に変換
            Debug.Log(laneNumber);
        }
    }
}
