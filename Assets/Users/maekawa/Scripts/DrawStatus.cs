using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawStatus : MonoBehaviour
{
    [SerializeField] GameObject JacketEmpty;
    [SerializeField] GameObject bestRankEmpty;
    [SerializeField] Sprite[] rank = new Sprite[4];
    [SerializeField] GameObject[] achievementEmpty = new GameObject[4];
    [SerializeField] Sprite[] achievement = new Sprite[4];
    [SerializeField] Text musicName;
    [SerializeField] Text highScore;
    [SerializeField] Text maxCombo;
    [SerializeField] GameObject activeFrame;
    [SerializeField] GameObject isActiveFrame;
    private int lastMusicNum = 0;
    private int lastGameType = 0;
    private int lastDifficultNumber = 0;
    private bool lastSelected = false;
    public bool isSelected = false;

    private void Start()
    {
        lastMusicNum = gameObject.GetComponent<MusicNumber>().musicNumber;
        DrawMusicNameJacket(lastMusicNum);
        DrawHighScore(lastMusicNum, (int)MusicDatas.gameType, MusicDatas.difficultNumber);
        DrawAchievement(lastMusicNum, (int)MusicDatas.gameType);
    }

    private void Update()
    {
        // 現状曲番号を動的に変更しないので
        //if (lastMusicNum != gameObject.GetComponent<MusicNumber>().musicNumber)
        //{
        //    lastMusicNum = gameObject.GetComponent<MusicNumber>().musicNumber;
        //    DrawMusicNameJacket(lastMusicNum);
        //}

        if (lastGameType != (int)MusicDatas.gameType | lastDifficultNumber != MusicDatas.difficultNumber)
        {
            DrawHighScore(lastMusicNum, (int)MusicDatas.gameType, MusicDatas.difficultNumber);
            DrawAchievement(lastMusicNum, (int)MusicDatas.gameType);
            lastGameType = (int)MusicDatas.gameType;
            lastDifficultNumber = MusicDatas.difficultNumber;
        }

        if (lastSelected != isSelected)
        {
            isActiveFrame.SetActive(lastSelected);
            activeFrame.SetActive(isSelected);
            lastSelected = isSelected;
        }
    }

    /// <summary>
    /// 引数にあたる変数が変更される度に曲名とジャケットを描画します
    /// </summary>
    /// <param name="i">musicNumber</param>
    private void DrawMusicNameJacket(int i)
    {
        musicName.text = MusicSelects.musicNames[i];
        JacketEmpty.GetComponent<Image>().sprite = Resources.Load<Sprite>("Jacket_cut/jacket_cut_" + i);
    }

    /// <summary>
    /// 引数にあたる変数のいずれかが変更される度にアチーブメントUIを描画します
    /// </summary>
    /// <param name="i">musicNumber</param>
    /// <param name="j">gameType</param>
    private void DrawAchievement(int i, int j)
    {
        for (int difficultNum = 0; difficultNum < 4; difficultNum++)
        {
            //switch (PlayerPrefsUtil<int>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
            //         MusicSelects.musicNotesNames[i], j, difficultNum, ScoreClass.PlayerPrefsHighRank), 0))
            switch (PlayerPrefs.GetInt(string.Format(ScoreClass.PlayerPrefsFormat,
                     MusicSelects.musicNotesNames[i], j, difficultNum, ScoreClass.PlayerPrefsHighRank), 0))
            {
                // C Rankは描画しない
                case 2:// B Rank
                    achievementEmpty[difficultNum].GetComponent<Image>().sprite = achievement[0];
                    break;
                case 3:// A Rank
                    achievementEmpty[difficultNum].GetComponent<Image>().sprite = achievement[1];
                    break;
                case 4:// S Rank
                    achievementEmpty[difficultNum].GetComponent<Image>().sprite = achievement[2];
                    break;
                default:
                    achievementEmpty[difficultNum].GetComponent<Image>().sprite = achievement[3];
                    break;
            }
        }
    }

    /// <summary>
    /// 引数にあたる変数のいずれかが変更される度にベストスコア、ベストコンボ、ランクUIを描画します
    /// </summary>
    /// <param name="i">musicNumber</param>
    /// <param name="j">gameType</param>
    /// <param name="k">difficultNumber</param>
    private void DrawHighScore(int i, int j, int k)
    {
        //highScore.text = PlayerPrefsUtil<string>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
        // MusicSelects.musicNotesNames[i], j, k, ScoreClass.PlayerPrefsHighScore), "0");
        //maxCombo.text = PlayerPrefsUtil<string>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
        // MusicSelects.musicNotesNames[i], j, k, ScoreClass.PlayerPrefsMaxCombo), "0");

        // ベストスコア表示
        int tempScore = PlayerPrefs.GetInt(string.Format(ScoreClass.PlayerPrefsFormat,
         MusicSelects.musicNotesNames[i], j, k, ScoreClass.PlayerPrefsHighScore), 0);
        highScore.text = tempScore.ToString();
        // ベストコンボ表示
        int tempCombo = PlayerPrefs.GetInt(string.Format(ScoreClass.PlayerPrefsFormat,
         MusicSelects.musicNotesNames[i], j, k, ScoreClass.PlayerPrefsMaxCombo), 0);
        maxCombo.text = tempCombo.ToString();


        bestRankEmpty.GetComponent<Image>().color = new Color(1, 1, 1, 1);
        //switch (PlayerPrefsUtil<int>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
        // MusicSelects.musicNotesNames[i], j, k, ScoreClass.PlayerPrefsHighRank), 0))
        switch (PlayerPrefs.GetInt(string.Format(ScoreClass.PlayerPrefsFormat,
         MusicSelects.musicNotesNames[i], j, k, ScoreClass.PlayerPrefsHighRank), 0))
        {
            case 1:// C Rank
                bestRankEmpty.GetComponent<Image>().sprite = rank[0];
                break;
            case 2:// B Rank
                bestRankEmpty.GetComponent<Image>().sprite = rank[1];
                break;
            case 3:// A Rank
                bestRankEmpty.GetComponent<Image>().sprite = rank[2];
                break;
            case 4:// S Rank
                bestRankEmpty.GetComponent<Image>().sprite = rank[3];
                break;
            default:
                bestRankEmpty.GetComponent<Image>().color = new Color(1, 1, 1, 0);
                break;
        }
    }
}
