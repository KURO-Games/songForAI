using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharactorImageMoved : MonoBehaviour
{
    // デフォルト値とは
    // 他に命令がない時にこれが使われますよという意味で書かれる
    [SerializeField] GameObject[] CharactorImage;

    int prev = default(int);
    public static int result = default(int);


    // Start is called before the first frame update
    void Start()
    {
        prev = 0;
        prev = result;
    }

    // Update is called once per frame
    void Update()
    {
        // prev と result 変数の中身(int型)が違った場合描画処理
        if (prev != result)
        {
            // prevに保存
            prev = result;

            // キャラ一度すべてfalse
            for (int i = 0; i < CharactorImage.Length; i++)
            {
                CharactorImage[i].SetActive(false);
            }

            // result変数が0より小さい時に分岐
            if (result < 0)
            {
                // 配列 Charactor を.Lengthで最大値を取り、最大値から-1を行い result に代入
                // これにより0より小さかった時に配列の最後の番号に戻る処理が行える。
                result = CharactorImage.Length - 1;
            }
            //result 変数の中身が配列 Charactor の最大値(.Lengthで取得)以上だった時分岐
            else if (result >= CharactorImage.Length)
            {
                // out of lengthでErrorになるためresult変数に0を代入を行う
                // これによりout of lengthでErrorになることがない
                result = 0;
            }

            // そして最後にキャライメージ変更
            // result 番目のキャラを表示
            CharactorImage[result].SetActive(true);

            //リザルトで一致したキャラを表示させるために代入
            //Result.charaNum = result;
        }
    }
}
