using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

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

    private bool _isEnded;
    GameObject InputCanvas;
    [SerializeField]
    CanvasGroup _userNameInputs;
    public static bool isUserInputs = false;
    private void Awake()
    {
        SceneLoadManager.SceneAdd("UserInputs");
    }
    private void Start()
    {
        
        //会話の一行目を読ませるための初期化
        Display_Num = 3;
        
        Text_Load("sinario_0");
        StartCoroutine(Message_Display());
        Message_Display();
        _isEnded = false;

    }
    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && Message_Complete == true&&isUserInputs==false)
        {
            StartCoroutine(Message_Display());
        }
    }

    public void StartCoroutineDisplay()
    {
        StartCoroutine(Message_Display());
    }

    /// <summary>
    /// 
    /// </summary>
    #region テキストロード
    private void Text_Load(string fileName)
    {
        //txtからロード
        TextAsset textAsset = new TextAsset();
        //                              ↓読み込むテキスト名  後にswitch分で進行度（？）ごとに読み込むシナリオを変えれるようにするかも？
        textAsset = Resources.Load("Text_Test3", typeof(TextAsset)) as TextAsset;

        FileInfo info = new FileInfo(Application.streamingAssetsPath + "/Scenario/"+fileName+".csv");
        StreamReader reader = new StreamReader(info.OpenRead());
        string Text_Lines = reader.ReadToEnd();

        //行ごとに分割
        Text_Message = Text_Lines.Split('\n');

        //横列認識
        Side_Num = Text_Message[2].Split(',').Length;
        //縦列認識
        Vertical_Num = Text_Lines.Split('\n').Length-1;

        //配列要素数確定させる
        Text_Words = new string[Vertical_Num, Side_Num];

        for(var i = 0; i < Vertical_Num; i++)
        {
            //保存する行を確定、保存
            string[] TempWords = Text_Message[i].Split(',');

            for (var n = 0; n < Side_Num; n++)
            {
                //Debug.Log(TempWords[n]);
                //保存する列を確定、保存
                Text_Words[i, n] = TempWords[n];
            }
        }
    }
    #endregion

    #region メッセージ、キャラクター、背景の表示
    public IEnumerator Message_Display()
    {
        //表示行数がtxtの行数以下なら
        if (Vertical_Num > Display_Num)
        {
            Message_Complete = false;
            //背景表示
            BackGround.sprite = BackGround_Sprite[int.Parse(Text_Words[Display_Num, 1])];
            //キャラ表示
            Character.sprite = Character_Sprite[int.Parse(Text_Words[Display_Num, 2])];
            //名前表示
            if(Text_Words[Display_Num, 3]=="null")
            {
                Name.text = "";
            }else if (Text_Words[Display_Num, 3] == "user")
            {
                Name.text = PlayerPrefs.GetString("PlayerName");
            }
            else
                Name.text = Text_Words[Display_Num, 3];
            //メッセージ表示
            //テキストリセット
            Message.text = "";
            var Message_Count = 0;
            var t = Time.time;
            while (Text_Words[Display_Num, 4].Length > Message_Count)
            {
                if (Text_Words[Display_Num, 4][Message_Count] == '\\')
                {

                    if (Text_Words[Display_Num, 4][Message_Count + 1] == 'n')
                    {
                        Message.text += "\n";
                        Message_Count += 1;
                    }
                    else if(Text_Words[Display_Num, 4][Message_Count + 1] == '\"')
                    {
                        Message.text += "\"";
                        Message_Count += 1;
                    }
                    else if(Text_Words[Display_Num, 4][Message_Count + 1] == 'u')
                    {
                        for (float i = 0; i <= 1; i += 0.01f)
                            _userNameInputs.alpha = i;
                        isUserInputs = true;
                        Message_Count += 1;
                        yield return new WaitForSeconds(Message_Speed);
                    }
                }
                else if (Text_Words[Display_Num, 4][Message_Count] == '\"')
                {
                    
                    yield return new WaitForSeconds(Message_Speed);
                }
                else
                {
                    
                    Message.text += Text_Words[Display_Num, 4][Message_Count];
                }
                Message_Count++;
                yield return new WaitForSeconds(Message_Speed);
            }
            Display_Num++;
            Message_Complete = true;
        }
        else if (!_isEnded)
        {
            //シーン遷移
            _isEnded = true;
            SceneLoadManager.LoadScene("SelectMusicV2");
        }
    }
    #endregion
}

// 今後の変更点
// Resources からの動的読み込みに変更。
//Toolを作成、CSVからバイナリに変更をするウインドウ作成予定。
//
