using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectMusicScene : MonoBehaviour
{
    // life
    public static int life = 2;
    [SerializeField]private GameObject[] Lifes;

    // 難易度選択/表示
    [SerializeField] private Image[] ChooseHighlight = new Image[4];
    [SerializeField] private Text[] diffcultLevel = new Text[4];
    [SerializeField] private Text[] selectedDifLevel = new Text[4];
    // OKボタンで起動
    [SerializeField] GameObject Panel;

    private int lastMusicNumber = 0;
    private int lastDifficultNumber = 0;
    private enum Difficults
    {
        EASY,
        NORMAL,
        HARD,
        PRO
    }

    private void Start()
    {
        SoundManager.BGMStop();

        LifeDraw();
        Highlight();
        DrawDifficulty(lastMusicNumber);
    }

    private void Update()
    {
        // 曲を変更した場合
        if (lastMusicNumber != MusicDatas.MusicNumber)
        {
            lastMusicNumber = MusicDatas.MusicNumber;
            DrawDifficulty(lastMusicNumber);
        }
    }
    public void SelectMusic(Button _button)
    {
        SoundManager.SESoundCue(1);

        // リザルト用　難易度レベルを保持
        MusicDatas.difficultLevel = MusicSelects.musicDifficulty[MusicDatas.MusicNumber,MusicDatas.difficultNumber];
        // 演奏画面用データをセット
        MusicSelects.MusicSelector((MusicNames)lastMusicNumber);

        // 選択しているキャラに応じて遷移
        switch ((int)MusicDatas.gameType)
        {
            case 0:
                Panel.SetActive(true);// 遷移中の選択を無効
                SceneLoadManager.LoadScene("Piano");
                break;
            case 1:
                Panel.SetActive(true);// 遷移中の選択を無効
                SceneLoadManager.LoadScene("ViolineDev");
                break;
            case 2:
                break;
            default:
                break;
        }    
    }

    //public void PButton(int i)
    //{
    //    MusicDatas.cueMusic = i;
    //}
    private void LifeDraw()
    {
        for (int i = 0; i < Lifes.Length; i++)
            Lifes[i].gameObject.SetActive(false);
        for (int i = 0; i < life; i++)
            Lifes[i].gameObject.SetActive(true);
    }

    public void PushDifficult(int i)
    {
        if(lastDifficultNumber != i)
        {
            lastDifficultNumber = i;
            Highlight();
        }
    }
    private void Highlight()
    {
        for (int i = 0; i < ChooseHighlight.Length; i++)
            ChooseHighlight[i].gameObject.SetActive(false);

        ChooseHighlight[lastDifficultNumber].gameObject.SetActive(true);
        ChooseHighlight[lastDifficultNumber].gameObject.transform.parent.transform.SetSiblingIndex(99);// 最前面に表示
        MusicDatas.difficultNumber = lastDifficultNumber;
    }
    /// <summary>
    /// 難易度表示
    /// </summary>
    /// <param name="i">musicNumber</param>
    private void DrawDifficulty(int i)
    {
        for (int j = 0; j < diffcultLevel.Length; j++)
        {
            // バイオリン未対応
            // MusicSelects.で難易度を宣言　musicDifficulty[曲番号, 難易度]
            diffcultLevel[j].text = MusicSelects.musicDifficulty[i, j].ToString();
            selectedDifLevel[j].text = MusicSelects.musicDifficulty[i, j].ToString();
        }
    }
}
