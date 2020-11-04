using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSelectPlate : MonoBehaviour
{
    string[] mArray = MusicSelects.MusicNameArray();
    Dictionary<string, MusicNames> musicdict = MusicSelects.MusicDict();

    [SerializeField] GameObject[] MusicSelect;
    [SerializeField] GameObject[] MusicSelectDark;
    [SerializeField] int ChoiceButtonNow = default(int);
    [SerializeField] Text[] diffNumTexts;

     int diffnum;
     void Start()
    {
        mArray = MusicSelects.MusicNameArray();
        musicdict = MusicSelects.MusicDict();

        for (int i = 0; i < MusicSelect.Length; i++)
        {
            MusicSelect[i].SetActive(false);
            MusicSelectDark[i].SetActive(true);
        }
        // デフォルトで0番目を選択
        MusicSelect[0].SetActive(true);
        MusicSelectDark[0].SetActive(false);
        ChoiceButtonNow = 0;
        // 曲名に基づいてenumを出力させる
        MusicNames musicNames = musicdict[mArray[0]];
        // enumを送って曲名などを保存
        MusicSelects.MusicSelector(musicNames);
        MusicDatas.MusicNumber = 0;
        SoundManager.DemoBGMSoundCue(0);
        
        DiffNum(diffnum);
    }

    /// <summary>
    /// 曲名クリック時処理(ハイライト等)
    /// </summary>
    /// <param name="j">曲名番号</param>
    public void OnClick(int j)
    {
        for (int i = 0; i < MusicSelect.Length; i++)
        {
            MusicSelect[i].SetActive(false);
        }
        MusicSelect[j].SetActive(true);

        // 曲名に基づいてenumを出力させる
        MusicNames musicNames = musicdict[mArray[j]];
        // enumを送って曲名などを保存
        MusicSelects.MusicSelector(musicNames);

        MusicDatas.MusicNumber = j;
    }

    /// <summary>
    /// 曲名クリック時選択されていない曲を暗くする
    /// </summary>
    /// <param name="j">曲名番号</param>
    public void OnClickDark(int j)
    {
        if (j.Equals(ChoiceButtonNow)) return;
        for (int i = 0; i < MusicSelectDark.Length; i++)
        {
            MusicSelectDark[i].SetActive(true);
            SoundManager.DemoBGMSoundCue(j);
        }
        MusicSelectDark[j].SetActive(false);
        ChoiceButtonNow = j;
    }

    /// <summary>
    /// 難易度数値表示関数
    /// </summary>
    public void DiffNum(int j)
    {
        //if (ChoiceButtonNow == 1)
        //{
        diffNumTexts[0].text = 2.ToString();
        diffNumTexts[1].text = 2.ToString();
        diffNumTexts[2].text = 8.ToString();
        diffNumTexts[3].text = 8.ToString();
        diffNumTexts[4].text = 12.ToString();
        diffNumTexts[5].text = 12.ToString();
        diffNumTexts[6].text = 17.ToString();
        diffNumTexts[7].text = 17.ToString();
        //}
        //else
        //{
        //    Debug.LogWarning("難易度を表示する値がありません");
        //}

        // すごい雑実装...修正途中です
    }
}
