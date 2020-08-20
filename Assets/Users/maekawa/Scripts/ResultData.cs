using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResultData : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        int resultScore = Judge.score;
        int resultCombo = Judge.bestcombo;
        int[] resultGrades = Judge.totalGrades;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
