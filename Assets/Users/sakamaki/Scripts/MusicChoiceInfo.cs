using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicChoiceInfo : MonoBehaviour
{
    [SerializeField] Text Score;
    [SerializeField] Text MusicNameTitle;
    [SerializeField] Image JacketImage;
    [SerializeField] string MusicName;
    [SerializeField] Text MaxCombo;
    [SerializeField] GameObject Rank;
    [SerializeField] GameObject[] RankImage = new GameObject[4];

    // ボタンがクリックされた時の挙動どうしようか悩み中。
    [SerializeField] GameObject MusicButton;

    int _score = 0;
    int _maxcombo = 0;
    int _rank = 0;

    public void Jacket()
    {
        //曲名表示
        MusicNameTitle.text = MusicName;
        // 動的にResources/Jacket/の中のMusicnameが一致する画像をJacketImageとして表示
        JacketImage.sprite = Resources.Load<Sprite>("Jacket/" + MusicName);
    }

    // Start is called before the first frame update
    void Start()
    {
         // Resultから習得したPlayerplefsを使い曲選択にhighscore,maxcombo,rankを持っていく

        // スコア習得
        Score.text = PlayerPrefsUtil<int>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
            MusicDatas.NotesDataName, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsHighScore)).ToString();
        // マックスコンボ習得
        MaxCombo.text = PlayerPrefsUtil<int>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
            MusicDatas.NotesDataName, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsMaxCombo)).ToString();
        // ランク習得しswitchでactiveするランク画像を分岐
        switch(PlayerPrefsUtil<int>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
            MusicDatas.NotesDataName, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsHighRank)))
        {
            case 4:
                RankImage[3].SetActive(true);
                break;
            case 3:
                RankImage[2].SetActive(true);
                break;
            case 2:
                RankImage[1].SetActive(true);
                break;
            case 1:
                RankImage[0].SetActive(true);
                break;
            default:
                break;
        }

        // 曲名表示、ジャケット習得
        Jacket();

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
