using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Text_Controller : MonoBehaviour
{
    [SerializeField]
    Text text;
    string[] Cat_Text;
    int Text_Num;

    /// <summary>
    /// テキストに区切りをつける
    /// 順番にテキスト表示
    /// 
    /// </summary>


    
    private void Start()
    {
        Text_Num = 0;
        Text_Cat();
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Next_Text();
        }
    }

    void Load_txt()
    {

    }

    void Text_Cat()
    {
        
    }

    void Next_Text()
    {
        if (Text_Num < Cat_Text.Length)
        {
            text.text = Cat_Text[Text_Num];
            Text_Num++;
        }
        else
        {
            text.text = "次のシーンへ";
        }
        
    }
}
