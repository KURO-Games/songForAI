using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedMusicPlate : MonoBehaviour
{
    [SerializeField] GameObject activeFrame;
    [SerializeField] GameObject InactiveFrame;

    public bool isSelected = false;
    private bool lastIsSelected = false;

    private void Start()
    {
        isSelected = false;
    }

    private void Update()
    {
        if ((lastIsSelected == false) && (isSelected == true))
        {
            activeFrame.SetActive(true);
            InactiveFrame.SetActive(false);
        }
        else if((lastIsSelected == true) && (isSelected == false))
        {
            activeFrame.SetActive(false);
            InactiveFrame.SetActive(true);
        }

        lastIsSelected = isSelected;
    }

    public void OnClick()
    {
        //isSelected = true;
    }
}
