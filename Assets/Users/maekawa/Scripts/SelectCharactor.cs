using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// スワイプしたまま元に戻すとキャラが変わってしまうので後で修正
public class SelectCharactor : MonoBehaviour
{
    [SerializeField] float slideRate;// キャラクターがどれくらい動くか（x方向、値と比例）
    //
    [SerializeField] GameObject[] charactor = new GameObject[3];       // キャラUI
    [SerializeField] Sprite[] charaUI = new Sprite[3];                 // 元画像
    private SpriteRenderer[] spriteRenderers = new SpriteRenderer[3];  // 画像ソースやalpha値をコントロール
    private Vector3[] defaultPosition = new Vector3[3];                // 初期位置　移動後に使用
    private bool isSwiping = false;                                    // スワイプ中であるか
    private bool isShifted = false;                                    // 1度のスワイプで1回のみシフト
    private float lastMousePosX = 0;                                   // mousePositionとの比較用
    private int centerNum = 0;                                         // どのキャラが中央なのか(gameTypeと同義)
    private int shiftNum = 0;                                          // charaUI配列指定用

    private void Start()
    {
        for (int i = 0; i < charactor.Length; i++)
        {
            defaultPosition[i] = charactor[i].transform.position;
            spriteRenderers[i] = charactor[i].GetComponent<SpriteRenderer>();
        }
    }

    private void Update()
    {

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10f, 1);

        // 画面右側をタップした場合処理
        if ((Input.GetMouseButtonDown(0)) && (hit))
        {
            isShifted = false;
            if ((hit.transform.gameObject != null) && (hit.transform.gameObject.tag == "Right"))
            {
                lastMousePosX = Input.mousePosition.x;
                isSwiping = true;//↓の処理に入る
            }
        }

        if((Input.GetMouseButton(0)) && (isSwiping))
        {
            // スワイプの移動距離に応じてキャラ移動
            float distance = lastMousePosX - Input.mousePosition.x;
            lastMousePosX = Input.mousePosition.x;
            Vector3 movePosition = new Vector3(distance * slideRate, 0, 0);

            for (int i = 0; i < charactor.Length; i++)
                charactor[i].transform.position -= movePosition;

            AlphaController();
        }

        if (Input.GetMouseButtonUp(0))
        {
            // 選択しているキャラに応じてgameTypeを代入
            MusicDatas.gameType = (GameType)centerNum;

            shiftNum = centerNum;
            // センターから順にcharaUIに画像を代入
            for (int i = 0; i < charactor.Length; i++)
            {
                if (shiftNum == charactor.Length)
                    shiftNum = 0;

                spriteRenderers[i].sprite = charaUI[shiftNum];
                shiftNum++;

                // キャラ位置を元に戻す(UI自体は移動しない　spriteのみ変わる)
                charactor[i].transform.position = defaultPosition[i];
            }

            // センター以外を透明化
            spriteRenderers[0].color = new Color(1, 1, 1, 1);
            spriteRenderers[1].color = new Color(1, 1, 1, 0);
            spriteRenderers[2].color = new Color(1, 1, 1, 0);

            isSwiping = false;
        }
    }

    /// <summary>
    /// 引数に応じて左右にキャラをシフトする準備をします(マウスを離してキャラシフト)
    /// </summary>
    /// <param name="R">true = right, false = left</param>
    public void SetCharactor(bool R)
    {
        if(isSwiping && isShifted == false)
        {
            if (R)
            {
                centerNum++;
                if (centerNum == charactor.Length)
                    centerNum = 0;
            }
            else
            {
                centerNum--;
                if (centerNum < 0)
                    centerNum = charactor.Length - 1;
            }
            isShifted = true;
        }
    }

    /// <summary>
    /// キャラがセンターから離れるほどalpha値を下げます
    /// </summary>
    private void AlphaController()
    {
        for(int i = 0; i < charactor.Length; i++)
        {
            float moved = defaultPosition[0].x - charactor[i].transform.position.x;
            float alpha = 1 - Mathf.Abs(moved) / 1500;
            spriteRenderers[i].color = new Color(1, 1, 1, alpha);
        }
    }
}
