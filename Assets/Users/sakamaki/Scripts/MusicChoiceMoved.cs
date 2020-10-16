using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicChoiceMoved : MonoBehaviour
{
    /*  プランナー用スクロールスピード
        数値をすくなくすれば早くなります)   */
    [SerializeField] int ScrollSpeed;
    /******************************************************/

    // ボタンを移動させるためのMusicButtonをSrialixeFieldで登録する。
    [SerializeField] GameObject MusicButton1;
    [SerializeField] GameObject MusicButton2;
    [SerializeField] GameObject MusicButton3;

    // マウス座標を習得
    Vector3 lastmousePotision;
    Vector3 mousePotision;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            // タップした時の処理を描く

            // クリックしたときにlastmousePotisionに座標を習得し代入
            lastmousePotision = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            // マウスの動きとオブジェクトの動きを同期させる
            mousePotision = Input.mousePosition;
            Vector3 movepos = MusicButton1.transform.position;
            // Y軸をlastmousePotision - mousePotisionする
            movepos.y -= (lastmousePotision.y - mousePotision.y) / ScrollSpeed;
            // MusicButton1にアタッチされたObjectを動かす
            MusicButton1.transform.position = movepos;
        }
        // mousePotisionをlastmousePotisionに代入をおこない座標がずれないように
        lastmousePotision = mousePotision;

        if (Input.GetMouseButtonUp(0))
        {
            // 手を離したときにオブジェクトポジションを保持する
        }
    }
}
