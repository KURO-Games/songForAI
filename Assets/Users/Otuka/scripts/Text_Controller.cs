using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Text_Controller : MonoBehaviour
{
    //メッセージテキスト
    [SerializeField]
    Text Message;
    //名前テキスト
    [SerializeField]
    Text Name;

    string[] Text_Message;  //一時的に１列のデータを格納する所
    string[,] Text_Words;   //ID　Name　Messageに分けて格納すること

    int Vertical_Num;   //縦列
    int Side_Num;       //横列

    int Display_Num;    //何番目の会話を表示してるか

    [SerializeField]
    float Message_Speed = 0;



    private void Start()
    {
        Display_Num = 0;
        Text_Load();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Message_Display();
        }
    }

    #region テキストロード
    void Text_Load()
    {
        //txtからロード
        TextAsset textAsset = new TextAsset();
        textAsset = Resources.Load("Text_Test", typeof(TextAsset)) as TextAsset;

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
                //保存する列を確定、保存
                Text_Words[i, n] = TempWords[n];
            }
        }
    }
    #endregion

    #region メッセージ表示
    void Message_Display()
    {
        //名前表示
        Name.text = Text_Words[Display_Num, 1];

        //メッセージ表示
        //テキストリセット
        Message.text = "";
        int Message_Count = 0;
        while (Text_Words[Display_Num, 2].Length > Message_Count)
        {
            Message.text += Text_Words[Display_Num, 2][Message_Count];
            Message_Count++;
            
        }
        Display_Num++;
    }
    #endregion
}
