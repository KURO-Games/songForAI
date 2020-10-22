using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicSelectPlate : MonoBehaviour
{
    string[] mArray = MusicSelects.MusicNameArray();
    Dictionary<string, MusicNames> musicdict = MusicSelects.MusicDict();

    [SerializeField] GameObject[] MusicSelect;

    // とりあえずここで設定
    private float[] highSpeeds = {0.3f, 0.4f, 0.3f};
    private float[] Speeds = { 0.256f, 0.075f, 0.335f };
    private float[] offsets = { 0, -8.0f, -9.0f };
    void Start()
    {
        mArray = MusicSelects.MusicNameArray();
        musicdict = MusicSelects.MusicDict();

        // デフォルトで0番目を選択
        for (int i = 0; i < MusicSelect.Length; i++)
        {
            MusicSelect[i].SetActive(false);
        }
        MusicSelect[0].SetActive(true);
        NotesGenerater.NotesSpeed = highSpeeds[0];
        NotesGenerater.speed = Speeds[0];
        NotesGenerater.offset = offsets[0];

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

        NotesGenerater.NotesSpeed = highSpeeds[j];
        NotesGenerater.speed = Speeds[j];
        NotesGenerater.offset = offsets[j];
    }
}
