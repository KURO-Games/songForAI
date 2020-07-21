using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Scenario_Controller : MonoBehaviour
{
    //メッセージテキスト
    [SerializeField]
    private Text Message;
    //名前テキスト
    [SerializeField]
    private Text Name;
    //キャラクター
    [SerializeField]
    private Image Character;
    //背景
    [SerializeField]
    private Image BackGround;
    //キャラクター画像格納
    [SerializeField]
    private Sprite[] Character_Sprite;
    //背景画像格納
    [SerializeField]
    private Sprite[] BackGround_Sprite;

    private string[] Text_Message;  //一時的に１列のデータを格納する所
    private string[,] Text_Words;   //[ID,Char,Name,Message]に分けて格納すること

    private int Vertical_Num;   //縦列
    private int Side_Num;       //横列

    private int Display_Num;    //何番目の会話を表示してるか

    [SerializeField]
    private float Message_Speed;    //メッセージスピード

    private bool Message_Complete;  //メッセージが最期まで表示されたかどうか

    private void Start()
    {
        Display_Num = 1;
        Text_Load();
        StartCoroutine(Message_Display());
        Message_Display();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && Message_Complete == true)
        {
            StartCoroutine(Message_Display());
        }
    }

    #region テキストロード
    private void Text_Load()
    {
        //txtからロード
        TextAsset textAsset = new TextAsset();
        textAsset = Resources.Load("Text_Test3", typeof(TextAsset)) as TextAsset;

        //ロードしたのを一度格納
        string Text_Lines = textAsset.text;

        //行ごとに分割
        Text_Message = Text_Lines.Split('\n');

        //横列認識
        Side_Num = Text_Message[0].Split('\t').Length;
        //縦列認識
        Vertical_Num = Text_Lines.Split('\n').Length;

        //配列要素数確定させる
        Text_Words = new string[Vertical_Num, Side_Num];

        for(var i = 0; i < Vertical_Num; i++)
        {
            //保存する行を確定、保存
            string[] TempWords = Text_Message[i].Split('\t');

            for (var n = 0; n < Side_Num; n++)
            {
                Debug.Log(TempWords[n]);
                //保存する列を確定、保存
                Text_Words[i, n] = TempWords[n];
            }
        }
    }
    #endregion

    #region メッセージ、キャラクター、背景の表示
    private IEnumerator Message_Display()
    {
        if (Vertical_Num > Display_Num)
        {
            Message_Complete = false;
            //背景表示
            BackGround.sprite = BackGround_Sprite[int.Parse(Text_Words[Display_Num, 1])];
            //キャラ表示
            Character.sprite = Character_Sprite[int.Parse(Text_Words[Display_Num, 2])];
            //名前表示
            Name.text = Text_Words[Display_Num, 3];
            //メッセージ表示
            //テキストリセット
            Message.text = "";
            var Message_Count = 0;
            var t = Time.time;
            while (Text_Words[Display_Num, 4].Length > Message_Count)
            {
                Message.text += Text_Words[Display_Num, 4][Message_Count];
                Message_Count++;
                yield return new WaitForSeconds(Message_Speed);
            }
            Display_Num++;
            Message_Complete = true;
        }
        else
        {
            //シーン遷移
            //SceneLoadManager.LoadScene("");
        }
    }
    #endregion
}
