using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharaSelecterSample : MonoBehaviour
{
    [SerializeField]
    Sprite[] character = default(Sprite[]);
    int prevSprite = default(int);
    int nextSprite = default(int);
    Image CharaImage = default(Image);
    private void Start()
    {
        nextSprite = 0;
        prevSprite = nextSprite;
    }
    /// <summary>
    /// Update
    /// </summary>
    private void Update()
    {
        // 前のフレームと今のフレームで差があるか
        if(prevSprite != nextSprite)
        {
            //差があるので、一致させる
            prevSprite = nextSprite;
            //0よりも小さいときは配列の最後の番号を指定
            if (nextSprite < 0)
                nextSprite = character.Length - 1;
            //配列よりも大きい数字だったら0にする
            else if (nextSprite >= character.Length)
                nextSprite = 0;
            //イメージ変更
            CharaImage.sprite = character[nextSprite];
        }
    }
    /// <summary>
    /// 配列番号を変えるスクリプト(今のところ想定はボタンだが、スライドのところで呼んでも大丈夫なつくりにしてある)
    /// 一枚先なら+1
    /// 前なら-1
    /// </summary>
    /// <param name="addValue"></param>
    public void PushButton(int addValue)
    {
        nextSprite += addValue;
    }
}
