using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ScoreManager : MonoBehaviour
{
    float score;

    void Update()
    {
        this.score = Judge.score;
        this.gameObject.GetComponent<Text>().text = "SCORE" + score.ToString();
    }
}
