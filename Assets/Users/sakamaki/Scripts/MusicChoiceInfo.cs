using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicChoiceInfo : MonoBehaviour
{
    [SerializeField] public GameObject[] Score;
    [SerializeField] public GameObject[] MusicNameTitle;
    [SerializeField] public GameObject[] JacketImage;
    [SerializeField] public string[] MusicName;
    [SerializeField] public GameObject[] MaxCombo;
    [SerializeField] public GameObject[] Rank;
    [SerializeField] public GameObject[] RankImage = new GameObject[4];
   

    // ボタンを移動させるためのMusicButtonをSrialixeFieldで登録する。
    [SerializeField] GameObject MusicButton1;
    [SerializeField] GameObject MusicButton2;
    [SerializeField] GameObject MusicButton3;

    // マウス座標を習得
    Vector3 lastmousePotision;
    Vector3 mousePotision;

    List<string> musicName = new List<string>();

    float Moving_ds = 0;
    int listCount = 1;

    // ボタンがクリックされた時の挙動どうしようか悩み中。
    [SerializeField] GameObject MusicButton;

    int _score = 0;
    int _maxcombo = 0;
    int _rank = 0;

    public void Jacket()
    {
        ////曲名表示
        //MusicNameTitle.text = MusicName;
        //// 動的にResources/Jacket/の中のMusicnameが一致する画像をJacketImageとして表示
        //JacketImage.sprite = Resources.Load<Sprite>("Jacket/" + MusicName);
    }

    // Start is called before the first frame update
    void Start()
    {

        musicName.Add("Song1morimori");
        musicName.Add("Song2");
        musicName.Add("Song3");
        musicName.Add("Song4");
        musicName.Add("Song5");

        // Resultから習得したPlayerplefsを使い曲選択にhighscore,maxcombo,rankを持っていく

        // スコア習得
        //Score.text = PlayerPrefsUtil<int>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
        //    MusicDatas.NotesDataName, Judge.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsHighScore)).ToString();
        //// マックスコンボ習得
        //MaxCombo.text = PlayerPrefsUtil<int>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
        //    MusicDatas.NotesDataName, Judge.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsMaxCombo)).ToString();
        // ランク習得しswitchでactiveするランク画像を分岐
        switch (PlayerPrefsUtil<int>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
            MusicDatas.NotesDataName, Judge.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsHighRank)))
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
        if (Input.GetMouseButtonDown(0))
        {
            // タップした時の処理を描く

            // クリックしたときにlastmousePotisionに座標を習得し代入
            lastmousePotision = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            // マウスの動きとオブジェクトの動きを同期させる
            mousePotision = Input.mousePosition;
            // Y軸をlastmousePotision - mousePotisionする
            float movepos = (lastmousePotision.y - mousePotision.y);

            Moving_ds += movepos;
            if (Moving_ds > 300)
            {
                if (MusicName.Length - 1 > listCount)
                listCount++;
                Moving_ds = 0;
            }

            if (Moving_ds < -300)
            {
                if (MusicName.Length +1 > listCount)
                listCount--;
                Moving_ds = 0;
            }

            Debug.Log(Moving_ds);
            // mousePotisionをlastmousePotisionに代入をおこない座標がずれないように
            lastmousePotision = mousePotision;
        }



        if (Input.GetMouseButtonUp(0))
        {
            // 手を離したときにオブジェクトポジションを保持する
            Moving_ds = 0;
        }

        MusicNameTitle[0].GetComponent<Text>().text = musicName[listCount-1];
        MusicNameTitle[1].GetComponent<Text>().text = musicName[listCount];
        MusicNameTitle[2].GetComponent<Text>().text = musicName[listCount+1];

    }
}
