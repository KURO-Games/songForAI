using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectMusic : MonoBehaviour
{
    [SerializeField] int plateSize;// 曲プレートの生成個数
    [SerializeField] float platePositionX;// 固定
    [SerializeField] float centerPositionY;// 0曲目が生成される位置(値-generateInterval)
    [SerializeField] float generateInterval;// topPositionXからgenerateInterval毎に生成
    [SerializeField] float diagonalRate;// 曲プレートがどれくらい動くか（x方向、値と反比例)
    [SerializeField] Transform topOrigin;
    [SerializeField] Transform topAlphaStart;
    [SerializeField] Transform bottomOrigin;
    [SerializeField] Transform bottomAlphaStart;
    //
    [SerializeField] GameObject musicPlatePrefab;// 生成元
    [SerializeField] GameObject setParent;// ↑の生成先
    //
    public static GameObject[] musicPlates;// プレートを生成後格納
    private Vector3[] defaultPositions;// 各プレートの初期位置
    private float lastMousePosY = 0;
    private bool isSwiping = false;
    private int selectNumber = 0;
    private DrawStatus[] drawStatus;
    private bool[] tempDrawStatus;
    private MusicNumber[] musicNumber;
    private CanvasGroup[] canvasGroups;
    private float fadeAreaDistanceAtTop;
    private float fadeAreaDistanceAtBottom;

    private void Start()
    {
        fadeAreaDistanceAtTop = Vector3.Distance(topOrigin.position, topAlphaStart.position);
        fadeAreaDistanceAtBottom = Vector3.Distance(bottomOrigin.position, bottomAlphaStart.position);

        musicPlates = new GameObject[plateSize];// 要素数分確保
        defaultPositions = new Vector3[plateSize];
        drawStatus = new DrawStatus[plateSize];
        tempDrawStatus = new bool[plateSize];
        musicNumber = new MusicNumber[plateSize];
        canvasGroups = new CanvasGroup[plateSize];

        // 生成
        int musicNum = 0;
        for (int i = 0; i < plateSize; i++)
        {
            // 生成
            musicPlates[i] = Instantiate(musicPlatePrefab) as GameObject;
            // 順番に並べる
            musicPlates[i].transform.position = new Vector3(platePositionX, centerPositionY -= generateInterval, 0);
            // 各初期位置を保持
            defaultPositions[i] = musicPlates[i].transform.position;
            musicPlates[i].name = "Plate" + i.ToString();
            musicPlates[i].transform.SetParent(setParent.transform, false);

            // 順番にPrefabに曲番号を代入
            musicPlates[i].GetComponent<MusicNumber>().musicNumber = musicNum;
            musicNum++;
            if (musicNum > MusicSelects.musicNames.Length - 1)
                musicNum = 0;
            // 制御スクリプトを取得
            drawStatus[i] = musicPlates[i].GetComponent<DrawStatus>();
            musicNumber[i] = musicPlates[i].GetComponent<MusicNumber>();
            canvasGroups[i] = musicPlates[i].GetComponent<CanvasGroup>();
        }

        // 配置を再調整
        ShiftUp();
        for(int i = 0; i < musicPlates.Length; i++)
            SetPlate(i);

        // コピペ
        for (int i = 0; i < musicPlates.Length; i++)
        {
            drawStatus[i].isSelected = false;
            double lineUp = GetLineUpOfMusicPlate(i);

            switch ((int)lineUp)
            {
                // 一番中心に近い曲を選択状態
                case 0:
                    drawStatus[i].isSelected = true;
                    MusicDatas.MusicNumber = musicNumber[i].musicNumber;
                    SoundManager.DemoBGMSoundCue(musicNumber[i].musicNumber);
                    selectNumber = musicNumber[i].musicNumber;
                    break;

                case 1:
                case -1:
                    canvasGroups[i].alpha = 1;
                    break;

                default:
                    canvasGroups[i].alpha = 0;
                    break;
            }
        }
    }

    private void Update()
    {
        if (SceneLoadManager.Loading) return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10f, 1);

        // 画面左側をタップした場合処理
        if ((Input.GetMouseButtonDown(0)) && (hit) && !SelectMusicPanelController.isPopUp)
        {
            if ((hit.transform.gameObject != null) && (hit.transform.gameObject.CompareTag("Left")))
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
                Transform musicPlateTrf = musicPlates[i].transform;
                drawStatus[i].isSelected = false;
                double lineUp = GetLineUpOfMusicPlate(i);

                if (lineUp == 0)
                    drawStatus[i].isSelected = true;

                if(drawStatus[i].isSelected!=tempDrawStatus[i])
                {
                    SoundManager.SESoundCue(6);
                }

                musicPlateTrf.position -= movePos;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            isSwiping = false;

            for(int i = 0; i < musicPlates.Length; i++)
            {
                drawStatus[i].isSelected = false;
                double lineUp = GetLineUpOfMusicPlate(i);

                switch ((int) lineUp)
                {
                    // 一番中心に近い曲を選択状態
                    case 0:
                        drawStatus[i].isSelected = true;

                        // 曲番号が違うならdemo再生
                        if (selectNumber != musicNumber[i].musicNumber)
                        {
                            MusicDatas.MusicNumber = musicNumber[i].musicNumber;
                            SoundManager.DemoBGMSoundCue(musicNumber[i].musicNumber);
                            selectNumber = musicNumber[i].musicNumber;
                        }

                        break;

                    case 1:
                    case -1:
                        break;

                    default:
                        break;
                }
            }
            // 整列したい
            for (int i = 0; i < musicPlates.Length; i++)
            {
                double lineUp = GetLineUpOfMusicPlate(i);

                if (lineUp == 0)
                {
                    float distance = defaultPositions[0].y - musicPlates[i].transform.localPosition.y;
                    for(int j = 0; j < musicPlates.Length; j++)
                    {
                        musicPlates[j].transform.position += new Vector3(0, distance);
                    }
                }
            }
        }

        ShiftUp();
        ShiftDown();
        for(int i = 0; i < musicPlates.Length; i++)
            SetPlate(i);

        FadeMusicPlates();
        for (int i = 0;i<drawStatus.Length;i++)
        {
            tempDrawStatus[i] = drawStatus[i].isSelected;
        }
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

    /// <summary>
    /// 指定インデックスのプレートの、中心からのインデックスを返す
    /// </summary>
    /// <param name="index">Index of musicPlates</param>
    /// <returns></returns>
    private double GetLineUpOfMusicPlate(int index)
    {
        return Math.Round(musicPlates[index].transform.localPosition.y / generateInterval,
                          MidpointRounding.AwayFromZero);
    }

    /// <summary>
    /// 指定プレートの期待される透明度（アルファ値）を取得する
    /// </summary>
    /// <param name="target">対象プレートのトランスフォーム</param>
    /// <returns>アルファ値</returns>
    private float GetExpectedAlphaOfMusicPlate(Transform target)
    {
        Vector3 targetPlatePos             = new Vector3(0, target.position.y);
        float   distanceFromTopToTarget    = Vector3.Distance(topOrigin.position, targetPlatePos);
        float   distanceFromBottomToTarget = Vector3.Distance(bottomOrigin.position, targetPlatePos);
        float   alphaValue                 = 1;

        // プレートのフェード効果範囲内への侵入率によってアルファ値決定
        // 上部
        if (fadeAreaDistanceAtTop >= distanceFromTopToTarget)
        {
            alphaValue = distanceFromTopToTarget / fadeAreaDistanceAtTop;
        }
        // 下部
        else if (fadeAreaDistanceAtBottom >= distanceFromBottomToTarget)
        {
            alphaValue = distanceFromBottomToTarget / fadeAreaDistanceAtBottom;
        }

        return alphaValue;
    }

    /// <summary>
    /// 各プレートのフェードアニメーションを行う
    /// </summary>
    private void FadeMusicPlates()
    {
        // スワイプ中
        if (isSwiping)
        {
            for (int i = 0; i < musicPlates.Length; i++)
            {
                int lineUp = (int) GetLineUpOfMusicPlate(i);

                switch (lineUp)
                {
                    // 中央3つのプレート
                    case 0:
                    case 1:
                    case -1:
                        // 常に表示するため、不透明になるまでフェードイン
                        if (canvasGroups[i].alpha < 1)
                        {
                            canvasGroups[i].alpha += Time.deltaTime * 2;
                        }

                        break;

                    // 外側のプレート
                    default:
                        float musicPlateAlphaRatio = GetExpectedAlphaOfMusicPlate(musicPlates[i].transform);

                        // 期待透明度に満たない場合
                        if (canvasGroups[i].alpha < musicPlateAlphaRatio)
                        {
                            Vector3 musicPlatePos = musicPlates[i].transform.position;

                            // 計算の都合上、画面端に差し掛かるプレートは逆にフェードインしてしまうので、その範囲では透明に
                            if (musicPlatePos.y >= topOrigin.position.y ||
                                musicPlatePos.y <= bottomOrigin.position.y)
                            {
                                canvasGroups[i].alpha = 0;
                            }
                            // 通常は期待値を満たすまでフェードイン
                            else
                            {
                                canvasGroups[i].alpha += Time.deltaTime * 2;
                            }
                        }
                        // 透明度が期待値を超えていたら戻す
                        // フェードアウトにすると、満たない場合の処理とのループになるのでそのまま代入
                        else if (canvasGroups[i].alpha > musicPlateAlphaRatio)
                        {
                            canvasGroups[i].alpha = musicPlateAlphaRatio;
                        }

                        break;
                }
            }
        }
        // 非スワイプ中
        else
        {
            for (int i = 0; i < musicPlates.Length; i++)
            {
                int lineUp = (int) GetLineUpOfMusicPlate(i);

                switch (lineUp)
                {
                    // 中央3つプレートは常に表示
                    case 0:
                    case 1:
                    case -1:
                        canvasGroups[i].alpha += Time.deltaTime * 2;

                        break;

                    // 外側のプレートはフェードアウト
                    default:
                        canvasGroups[i].alpha -= Time.deltaTime;

                        break;
                }
            }
        }
    }
}
