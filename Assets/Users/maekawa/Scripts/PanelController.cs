using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] GameObject details;
    [SerializeField] Text subText;
    private float panelWidth;
    private float panelHeight;
    private bool popUpFlag;
    void Start()
    {
        panelWidth = 0;
        panelHeight = 0.05f;
        popUpFlag = false;

        // シナリオを読んでいたら処理
        if (Scenario_Controller.isReaded)
        {
            Scenario_Controller.isReaded = false;
            Scenario_Controller.scenarioNumber++;
            subText.text = "シナリオ" + Scenario_Controller.scenarioNumber + "話が解放されました";
            popUpFlag = true;
        }
    }

    void Update()
    {
        // ポップアップアニメーション
        if(popUpFlag && Result.isEnded)
        {
            panel.SetActive(true);
            panel.transform.GetChild(0).localScale = new Vector3(panelWidth, panelHeight, 0);

            if (panelWidth < 1)
            {
                panelWidth += 0.1f;
            }
            else if (panelHeight < 1)
            {
                panelHeight += 0.1f;
            }
            else
            {
                details.SetActive(true);
                popUpFlag = false;
            }
        }
    }
}
