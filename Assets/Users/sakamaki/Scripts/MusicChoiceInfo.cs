using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicChoiceInfo : MonoBehaviour
{
    [SerializeField] Text Score;
    [SerializeField] Text Musicname;
    [SerializeField] Image JacketImage;
    [SerializeField] Text MaxCombo;
    [SerializeField] GameObject MusicButton;

    int _score = 0;
    int _maxcombo = 0;
    string _musicname = MusicDatas.MusicName;

    public void Jacket()
    {
        //曲名表示
        Musicname.text = _musicname;
        // 動的にResources/Jacket/の中のMusicnameが一致する画像をJacketImageとして表示
        JacketImage.sprite = Resources.Load<Sprite>("Jacket/" + JacketImage);
    }

    // Start is called before the first frame update
    void Start()
    {

        // Resultから習得したPlayerplefsを使い曲選択にhighscore,maxcombo,rankを持っていく
        // Rank習得がきっとできてないので修正予定(WIP)
        const string HIGH_SCORE = "highscore";
        const string MAXCOMBO = "maxcombo";
        const string HIGH_RANK = "highrank";

        _score = PlayerPrefs.GetInt(HIGH_SCORE, -1);
        _maxcombo = PlayerPrefs.GetInt(MAXCOMBO, -1);

        // scoreが0より小さい時は0を代入し、0じゃなかった場合は_scoreにハイスコアを代入する
        if (_score < 0)
        {
            Score.text = 0.ToString();
        }
        else
        {
            Score.text = _score.ToString();
        }
        // Maxcombo処理も同様
        if (_maxcombo < 0)
        {
            MaxCombo.text = 0.ToString();
        }
        else
        {
            MaxCombo.text = _maxcombo.ToString();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
