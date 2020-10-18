using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharactorImageMoved : MonoBehaviour
{
    // デフォルト値とは
    // 他に命令がない時にこれが使われますよという意味で書かれる
    [SerializeField] GameObject CharactorImage = default(GameObject);
    [SerializeField] Sprite[] Charactor = default(Sprite[]);

    [SerializeField] Button RightButton = default(Button);
    [SerializeField] Button LeftButton = default(Button);

    int prev = default(int);
    int result = default(int);
    int intButton = default(int);


    // Start is called before the first frame update
    void Start()
    {
        prev = 0;
        prev = result;
    }

    // Update is called once per frame
    void Update()
    {
        // prev と result 変数の中身(int型)が違った場合分岐
        if (prev != result)
        {
            // まず先にずれているので prev, result変数の中身を代入し一致させる
            prev = result;
            // result変数が0より小さい時に分岐
            if (result < 0)
            {
                // 配列 Charactor を.Lengthで最大値を取り、最大値から-1を行い result に代入
                // これにより0より小さかった時に配列の最後の番号に戻る処理が行える。
                result = Charactor.Length - 1;
            }
            //result 変数の中身が配列 Charactor の最大値(.Lengthで取得)以上だった時分岐
            else if (result >= Charactor.Length)
            {
                // out of lengthでErrorになるためresult変数に0を代入を行う
                // これによりout of lengthでErrorになることがない
                result = 0;
            }

            // そして最後にキャライメージ変更
            // Image型のCharactorImageから.spriteでsprite習得
            // spriteを習得したCharactorImageに[SerializeField]した配列 Charactor 
            // []の部分に数値を調整した result 変数の数を動かす
            CharactorImage.GetComponent<Image>().sprite = Charactor[result];
        }
    }

    /// <summary>
    /// ボタンで配列の中身の数値を変える関数
    /// public なのでどこでも呼び込める(ハズ)
    /// </summary>
    /// <param name ="addValue"></param>
    public void PushButtonRight(int value)
    {
        // result = result + value;
        result += value;
    }

    public void PushButtonLeft(int addValue)
    {
        result -= addValue;
    }

    // ...?
}
