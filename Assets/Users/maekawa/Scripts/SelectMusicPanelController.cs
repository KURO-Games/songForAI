using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SelectMusicPanelController : MonoBehaviour
{
    [SerializeField] GameObject panel;
    [SerializeField] GameObject details;
    [SerializeField] Text mainText;
    private float panelWidth = 0;
    private float panelHeight = 0.05f;
    public static bool popUpFlag = false;
    private int lastGameType;
    public static string notPlayableType;
    void Start()
    {
        popUpFlag = false;
    }

    void Update()
    {
        if (lastGameType != (int)MusicDatas.gameType)
        {
            switch ((int)MusicDatas.gameType)
            {
                case 1:
                    notPlayableType = "バイオリン";
                    break;
                case 2:
                    notPlayableType = "ドラム";
                    break;
                default:
                    break;
            }
            mainText.text = notPlayableType + "モードは現在制作中です";
            lastGameType = (int)MusicDatas.gameType;
        }

        // ポップアップアニメーション
        if (popUpFlag)
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
                panelHeight = 0.05f;
                panelWidth = 0;
                popUpFlag = false;
            }
        }
    }
}
