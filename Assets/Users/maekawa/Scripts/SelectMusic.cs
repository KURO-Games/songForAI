using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMusic : MonoBehaviour
{
    //
    [SerializeField] int plateSize;// 曲プレートの生成個数
    [SerializeField] float platePositionX;// 固定
    [SerializeField] float centerPositionY;// 0曲目が生成される位置(値-generateInterval)
    [SerializeField] float generateInterval;// topPositionXからgenerateInterval毎に生成
    [SerializeField] float diagonalRate;// 曲プレートがどれくらい動くか（x方向、値と反比例)
    //
    [SerializeField] GameObject musicPlatePrefab;// 生成元
    [SerializeField] GameObject setParent;// ↑の生成先
    //
    public static GameObject[] musicPlates;// プレートを生成後格納
    private Vector3[] defaultPositions;// 各プレートの初期位置
    private float lastMousePosY = 0;
    private bool isSwiping = false;
    private int musicNumCount = 0;// 曲番号

    void Start()
    {
        musicPlates = new GameObject[plateSize];// 要素数分確保
        defaultPositions = new Vector3[plateSize];

        // 生成
        for (int i = 0; i < plateSize; i++)
        {
            // 生成
            musicPlates[i] = Instantiate(musicPlatePrefab) as GameObject;
            musicPlates[i].transform.position = new Vector3(platePositionX, centerPositionY -= generateInterval, 0);
            defaultPositions[i] = musicPlates[i].transform.position;
            musicPlates[i].name = "Plate" + i.ToString();
            musicPlates[i].transform.SetParent(setParent.transform, false);

            // 順番にPrefabに曲番号を代入
            musicPlates[i].GetComponent<MusicNumber>().musicNumber = musicNumCount;
            musicNumCount++;
            if (musicNumCount > MusicSelects.musicNames.Length - 1)
                musicNumCount = 0;
        }

        ShiftUp();
        for(int i = 0; i < musicPlates.Length; i++)
            SetPlate(i);
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10f, 1);

        // 画面左側をタップした場合処理
        if ((Input.GetMouseButtonDown(0)) && (hit))
        {
            if ((hit.transform.gameObject != null) && (hit.transform.gameObject.tag == "Left"))
            {
                lastMousePosY = Input.mousePosition.y;
                isSwiping = true;
            }
        }

        if ((Input.GetMouseButton(0)) && (isSwiping))
        {
            // スワイプの移動距離に応じてプレート移動
            float distance = lastMousePosY - Input.mousePosition.y;
            lastMousePosY = Input.mousePosition.y;

            float moveY = distance * 2.5f;
            Vector3 movePos = new Vector3(0, moveY, 0);

            for (int i = 0; i < musicPlates.Length; i++)
            {
                musicPlates[i].transform.position -= movePos;
                SetPlate(i);
            }
        }

        if (Input.GetMouseButtonUp(0))
            isSwiping = false;

        ShiftDown();
        ShiftUp();
    }

    /// <summary>
    /// y座標に応じてプレートのx座標を動かします(for文前提)
    /// </summary>
    /// <param name="i">for文のiとか</param>
    private void SetPlate(int i)
    {
        // y座標が中心から離れるほどx方向にマイナス
        // 折り返しバージョン
        float posX = Mathf.Abs((defaultPositions[0].y - musicPlates[i].transform.position.y) / diagonalRate);
        // 斜めバージョン
        //float posX = (defaultPositions[0].y - musicPlates[i].transform.position.y) / diagonalRate;
        Vector3 tempPos = new Vector3(defaultPositions[0].x - posX, musicPlates[i].transform.position.y, 0);
        musicPlates[i].transform.position = tempPos;
    }

    private void ShiftDown()
    {
        for (int i = 0; i < musicPlates.Length; i++)
        {
            double lineUp = Math.Round(musicPlates[i].transform.localPosition.y / generateInterval, MidpointRounding.AwayFromZero);
            if (lineUp > 3)// 画面外に行ったら移動
            {
                int nextNum = i - 1;
                if (nextNum < 0)
                    nextNum = musicPlates.Length - 1;
                Vector3 tempPos = musicPlates[nextNum].transform.localPosition - new Vector3(0, generateInterval, 0);
                musicPlates[i].transform.localPosition = tempPos;
            }
        }
    }

    private void ShiftUp()
    {
        for (int i = 1; i < musicPlates.Length + 1; i++)
        {
            double lineUp = Math.Round(musicPlates[musicPlates.Length - i].transform.localPosition.y / generateInterval, MidpointRounding.AwayFromZero);

            if (lineUp < -3)
            {
                int nextNum = musicPlates.Length - i + 1;
                if (nextNum >= musicPlates.Length)
                    nextNum = 0;
                Vector3 tempPos = musicPlates[nextNum].transform.localPosition + new Vector3(0, generateInterval, 0);
                musicPlates[musicPlates.Length - i].transform.localPosition = tempPos;
            }
        }
    }
}