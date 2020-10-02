using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicChoiceInfo : MonoBehaviour
{
    // resultから持ってくるための仮置き
    [SerializeField] Text Score;
    [SerializeField] Text Musicname;
    [SerializeField] Text MaxCombo;
    [SerializeField] GameObject MusicButton;

    int _score = 100;
    int _maxcombo = 30;
    string _musicname = "MusicTemplate1";

    // Start is called before the first frame update
    void Start()
    {
        Score.text = _score.ToString();
        Musicname.text = _musicname;
        MaxCombo.text = _maxcombo.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
