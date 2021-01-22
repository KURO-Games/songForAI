using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ClearDisplay : MonoBehaviour
{
    [SerializeField] Animator animator;
    bool isDisplayed;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if(SoundManager.BGMStatus()==CriAtomExPlayer.Status.PlayEnd&&!isDisplayed)
        {
            isDisplayed = true;
            //FullCombo表示
            if (NotesJudgementBase.TotalGrades[2] == 0 && NotesJudgementBase.TotalGrades[3] == 0 && NotesJudgementBase.TotalGrades[4] == 0)
            {
                animator.SetTrigger("FullCombo");
            }
            //クリア表示
            else if(NotesJudgementBase.bestCombo>0)
            {
                animator.SetTrigger("Clear");
            }
        }
    }
}
