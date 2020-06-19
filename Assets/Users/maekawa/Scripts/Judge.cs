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
    [SerializeField] private GameObject[] Lanes = new GameObject[8];

    int score = 0;
    int combo = 0;
    int life = 1000;
    float notePosY = -300; //仮
    GameObject JudgeLine;

    void Start()
    {
        var io = JudgeLine.gameObject.transform.position.x;
    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            Vector2 tapPos = Input.mousePosition;
            if (tapPos = Lanes[])//レーンが一致していれば
            {
                float tapTiming = JudgeLine.transform.y - notePosY; //ズレの値
                float absTiming = Mathf.Abs(tapTiming); //絶対値に変換

                if (absTiming <= perfect)
                {
                    score =+ point;
                    combo ++;
                    Debug.Log(score);
                    Debug.Log(combo);
                }
                else if(absTiming <= great)
                {
                    score =+ point / 2;
                    combo ++;
                    Debug.Log(score);
                    Debug.Log(combo);
                }
                else if(absTiming <= miss)
                {
                    combo = 0;
                }
            }
        }
    }
}