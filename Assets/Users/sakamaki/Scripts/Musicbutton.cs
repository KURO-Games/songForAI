using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Musicbutton : MonoBehaviour
{
    [SerializeField]
    private Text _musicName;
    [SerializeField]
    private Image _jacketImage;

    public string musicName;
    public string jacketImage;

    public void Initialized()
    {
        _musicName.text = musicName;
        _jacketImage.sprite = Resources.Load<Sprite>("Jacket/" + jacketImage);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
