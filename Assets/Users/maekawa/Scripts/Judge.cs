using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Judge : MonoBehaviour
{
    [SerializeField] private int point = 1000;
    [SerializeField] private float perfect = 15;
    [SerializeField] private float great = 30;
    [SerializeField] private float miss = 40;

    int score = 0;
    int combo = 0;
    int[] laneArray = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
    //float judgeLinePosY = -316;
    //float notePosY = -300;
    GameObject white;
    white.transform.position.y;

    void Start()
    {

    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var pos = Input.mousePosition;
            if ()//レーンが一致していれば
            {
                float tapTiming = //judgeLinePosY - notePosY;
                float absTiming = Mathf.Abs(tapTiming);

                if (absTiming <= perfect)
                {
                    score =+ point;
                    combo ++;
                    Debug.Log(score);
                }
                else if(absTiming <= great)
                {
                    score =+ point / 2;
                    combo ++;
                }
                else if(absTiming <= miss)
                {
                    combo = 0;
                }
            }
        }
    }
}