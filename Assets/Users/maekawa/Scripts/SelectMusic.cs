using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMusic : MonoBehaviour
{
    [SerializeField] GameObject[] musicPlates = new GameObject[5];
    private float lastMousePosY = 0;
    private Vector3[] defaultPositions = new Vector3[5];

    void Start()
    {
        for (int i = 0; i < musicPlates.Length; i++)
            defaultPositions[i] = musicPlates[i].transform.position;
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10f, 1);

        // 画面左側をタップした場合処理
        if ((Input.GetMouseButtonDown(0)) && (hit))
        {
            if ((hit.transform.gameObject != null) && (hit.transform.gameObject.tag == "Left"))
                lastMousePosY = Input.mousePosition.y;
        }

        if (Input.GetMouseButton(0))
        {
            // スワイプの移動距離に応じて
            float distance = lastMousePosY - Input.mousePosition.y;
            lastMousePosY = Input.mousePosition.y;

            float posY = distance * 2.5f;
            Vector3 moveY = new Vector3(0, posY, 0);

            for (int i = 0; i < musicPlates.Length; i++)
            {
                musicPlates[i].transform.position -= moveY;

                float posX = Mathf.Abs((defaultPositions[1].y - musicPlates[i].transform.position.y) / 1000);

                musicPlates[i].transform.position = musicPlates[i].transform.position - new Vector3(posX, 0, 0);
            }
        }

        if (Input.GetMouseButtonUp(0))
        {

        }
    }

}