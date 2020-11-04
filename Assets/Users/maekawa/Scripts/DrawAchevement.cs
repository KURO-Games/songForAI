using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DrawAchevement : MonoBehaviour
{
    [SerializeField] GameObject[] achievementEmpty = new GameObject[4];
    [SerializeField] Sprite[] achievement = new Sprite[3];
    [SerializeField] int MusicNumber;
    void Start()
    {
        // test
        //PlayerPrefs.DeleteAll();
        //PlayerPrefsUtil<int>.Save(string.Format(ScoreClass.PlayerPrefsFormat,
        //MusicSelects.musicNotesNames[2], 0, 3, ScoreClass.PlayerPrefsHighRank), 4);

        for (int difficultNum = 0; difficultNum < 4; difficultNum++)
        {
            switch (PlayerPrefsUtil<int>.Load(string.Format(ScoreClass.PlayerPrefsFormat,
                     MusicSelects.musicNotesNames[MusicNumber], 0, difficultNum, ScoreClass.PlayerPrefsHighRank), 0))
            {
                case 2:// Brank
                    achievementEmpty[difficultNum].GetComponent<Image>().sprite = achievement[0];
                    break;
                case 3:// Arank
                    achievementEmpty[difficultNum].GetComponent<Image>().sprite = achievement[1];
                    break;
                case 4:// Srank
                    achievementEmpty[difficultNum].GetComponent<Image>().sprite = achievement[2];
                    break;
                default:
                    break;
            }
        }
    }
}
