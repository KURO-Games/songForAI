using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectMusicPanelController : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private GameObject details;
    [SerializeField] private Text mainText;
    [SerializeField] private Animator animator;
    [SerializeField] private Image displayTips;
    [SerializeField] private Sprite[] tips;
    [SerializeField] private GameObject RightButton;
    [SerializeField] private GameObject LeftButton;
    private int displayNum = 0;
    public static bool isPopUp;

    private void Start()
    {
        isPopUp = false;
    }

    private void Update()
    {

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
        if (displayNum > tips.Length - 1)
            displayNum = 0;
        displayTips.sprite = tips[displayNum];
    }

    public void PushLeft()
    {
        displayNum--;
        if (displayNum < 0)
            displayNum = tips.Length - 1;
        displayTips.sprite = tips[displayNum];
    }

    public void Exit()
    {
        displayNum = 0;
        displayTips.sprite = tips[0];
        panel.SetActive(false);
        isPopUp = false;
    }
}
