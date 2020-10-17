using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class MusicChoiceInfo : MonoBehaviour
{

    // listで動かすように配列化させたAttribute群
    [SerializeField] public GameObject Score;
    [SerializeField] public GameObject[] MusicNameTitle;
    [SerializeField] public GameObject[] JacketImage;
    [SerializeField] public string[] MusicName;
    [SerializeField] public GameObject[] MaxCombo;
    [SerializeField] public GameObject[] Rank;
    [SerializeField] public GameObject[] RankImage = new GameObject[4];

    // マウス座標を習得
    Vector3 lastmousePotision;
    Vector3 mousePotision;

    // 曲名を動かすためのリスト作成
    List<string> musicName = new List<string>();

    float Moving_ds = 0;
    int listCount = 1;

    // ボタンがクリックされた時の挙動どうしようか悩み中。
    [SerializeField] GameObject MusicButton;

    int prev = 0;


    // Start is called before the first frame update
    void Start()
    {

        prev = listCount;
        musicName.Add("Song1");
        musicName.Add("Song2");
        musicName.Add("Song3");
        musicName.Add("Song4");
        musicName.Add("Song5");

        ChangeMusicText();
        // Resultから習得したPlayerplefsを使い曲選択にhighscore,maxcombo,rankを持っていく

        // スコア習得
        Score.GetComponent<Text>().text = PlayerPrefsUtil<string>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
            MusicDatas.NotesDataName, Judge.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsHighScore));
        //// マックスコンボ習得
        MaxCombo[0].GetComponent<Text>().text = PlayerPrefsUtil<string>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
            MusicDatas.NotesDataName, Judge.gameType, MusicDatas.difficultNumber, ScoreClass.PlayerPrefsMaxCombo),"0");
        // ランク習得しswitchでactiveするランク画像を分岐

        // switch文で曲選択画面時のRank画像表示分岐
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
        MusicListMove();
    }

    /// <summary>
    /// ジャケット絵習得関数
    /// </summary>
    public void Jacket()
    {
        ////曲名表示
        //MusicNameTitle.text = MusicName;
        //// 動的にResources/Jacket/の中のMusicnameが一致する画像をJacketImageとして表示
        //JacketImage.sprite = Resources.Load<Sprite>("Jacket/" + MusicName);
    }

    /// <summary>
    /// 曲選択UI リスト移動用関数
    /// </summary>
    private void MusicListMove()
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

            // マウスクリックされ続けているときに、moveposからMoving_dsに+=で代入
            Moving_ds += movepos;

            // Moving_dsの値が 300 より大きいときに、MusicNameの最大値から -1 され listcount より大きかった時に
            // listcountを+する、そしてmoving_dsを初期化
            if (Moving_ds > 300)
            {
                Debug.Log("test");
                //if (MusicName.Length - 1 > listCount)
                listCount--;
                Moving_ds = 0;
            }
            // 上記と基本的には同じ
            else if (Moving_ds < -300)
            {
                //if (MusicName.Length + 1 > listCount)
                listCount++;
                Moving_ds = 0;
            }

            Debug.Log(Moving_ds);
            // mousePotisionをlastmousePotisionに代入をおこない座標がずれないように
            lastmousePotision = mousePotision;
        }

        if (Input.GetMouseButtonUp(0))
        {
            // 手を離したときに Moving_ds を初期化する
            Moving_ds = 0;
        }

        if (prev != listCount)
        {
            ChangeMusicText();
        }
    }
    private void ChangeMusicText()
    {
        prev = listCount;
        // 要素超えたとき
        for (int i = 0; i <= MusicNameTitle.Length - 1; i++)
        {
            if (listCount <= 0)
            {

                listCount = 0;
                if (i == 0)
                {
                    MusicNameTitle[i].GetComponent<Text>().text = musicName[musicName.Count - 1];
                    continue;
                }

            }
            else
            {
                MusicNameTitle[0].gameObject.SetActive(true);
                //continue;
            }
            MusicNameTitle[i].GetComponent<Text>().text = musicName[listCount + i - 1];
            //MusicNameTitle[0].GetComponent<Text>().text = musicName[listCount];
            //MusicNameTitle[1].GetComponent<Text>().text = musicName[listCount + 1];
            //MusicNameTitle[2].GetComponent<Text>().text = musicName[listCount + 2];
        }
    }
}
