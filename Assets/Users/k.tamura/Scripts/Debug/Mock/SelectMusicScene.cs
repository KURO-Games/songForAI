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
    [SerializeField] GameObject PopUpPanel;

    private int lastMusicNumber = 0;
    private int lastDifficultNumber = 0;
    private int lastGameType = 0;
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
        SoundManager.SetVolume(1, SoundType.DemoBGM);
        DrawDifficulty(lastGameType, lastMusicNumber);
    }

    private void Update()
    {
        // 曲もしくは演奏方法を変更した場合
        if (lastMusicNumber != MusicDatas.MusicNumber || lastGameType != (int)MusicDatas.gameType)
        {
            lastMusicNumber = MusicDatas.MusicNumber;
            lastGameType = (int)MusicDatas.gameType;
            DrawDifficulty(lastGameType, lastMusicNumber);
        }
    }
    public void SelectMusic(Button _button)
    {
        //決定時のSE再生
        SoundManager.SESoundCue(1);
        SoundManager.SetVolume(0.0f, SoundType.DemoBGM);

        SoundManager.DemoStop();
        switch (MusicDatas.gameType)
        {
            case GameType.Piano:
                SoundManager.LoadAsyncCueSheet(SoundDefine.pare, SoundType.Scenario);
                break;
            case GameType.Violin:
                SoundManager.LoadAsyncCueSheet(SoundDefine.flanz, SoundType.Scenario);

                break;
            case GameType.Drum:
                SoundManager.LoadAsyncCueSheet(SoundDefine.eric, SoundType.Scenario);
                break;
        }
        // リザルト用　難易度レベルを保持
        MusicDatas.difficultLevel = MusicSelects.musicDifficulty[(int)MusicDatas.gameType, MusicDatas.MusicNumber, MusicDatas.difficultNumber];
        // 演奏画面用データをセット
        MusicSelects.MusicSelector((MusicNames)lastMusicNumber);
        SoundManager.UnLoadCueSheet(SoundType.BGM);
        SoundManager.LoadAsyncCueSheet(SoundDefine.musics[lastMusicNumber],SoundType.BGM);
        // 選択しているキャラに応じて遷移
        switch (MusicDatas.gameType)
        {
            case GameType.Piano:
                Panel.SetActive(true);// 遷移中の選択を無効
                SceneLoadManager.LoadScene("Piano");
                break;
            case GameType.Violin:
                Panel.SetActive(true);
                SceneLoadManager.LoadScene("ViolineDev");
                break;
            case GameType.Drum:
                // 未実装表示処理
                PopUpPanel.SetActive(true);
                SelectMusicPanelController.isPopUp = true;
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
    /// 演奏方法、曲ごとの難易度表示
    /// </summary>
    /// <param name="i">gameType</param>
    /// <param name="j">musicNumber</param>
    private void DrawDifficulty(int i, int j)
    {
        for (int k = 0; k < diffcultLevel.Length; k++)
        {
            // MusicSelects.で難易度を宣言　musicDifficulty[演奏方法, 曲番号, 難易度]
            diffcultLevel[k].text = MusicSelects.musicDifficulty[i, j, k].ToString();
            selectedDifLevel[k].text = MusicSelects.musicDifficulty[i, j, k].ToString();
        }
    }
}
