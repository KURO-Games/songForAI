using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectMusicPanelController : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    //[SerializeField] private Text mainText;
    [SerializeField] private Animator animator;
    [SerializeField] private Image displayImage;
    [SerializeField] private Sprite[] helps;
    private int displayNum = 0;
    public static bool isPopUp;

    private void Start()
    {
        isPopUp = false;
    }

    public void PlayUIAnimation()
    {
        panel.SetActive(true);
        isPopUp = true;
        SoundManager.SESoundCue(8);
    }

    public void PushRight()
    {
        displayNum++;
        if (displayNum > helps.Length - 1)
            displayNum = 0;
        displayImage.sprite = helps[displayNum];
        SoundManager.SESoundCue(8);
    }

    public void PushLeft()
    {
        displayNum--;
        if (displayNum < 0)
            displayNum = helps.Length - 1;
        displayImage.sprite = helps[displayNum];
        SoundManager.SESoundCue(8);
    }

    public void Exit()
    {
        displayNum = 0;
        displayImage.sprite = helps[0];
        panel.SetActive(false);
        isPopUp = false;
    }
}
