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
    }

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
}
