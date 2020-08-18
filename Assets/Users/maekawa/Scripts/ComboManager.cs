using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class ComboManager : MonoBehaviour
{
    int combo;

    void Update()
    {
        this.combo = Judge.combo;
        this.gameObject.GetComponent<Text>().text = "COMBO" + combo.ToString();
    }
}
