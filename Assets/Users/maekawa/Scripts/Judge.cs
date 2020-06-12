using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Judge : MonoBehaviour
{
    int[] lane_arr = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 };
    int score = 0;
    [SerializeField] int point = 1000;

    void Start()
    {

    }

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            var pos = Input.mousePosition;

            score =+ point;
            Debug.Log(score);
        }
    }
}
