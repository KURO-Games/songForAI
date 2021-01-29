using UnityEngine;

public class ClearDisplay : SingletonMonoBehaviour<ClearDisplay>
{
    [SerializeField]
    private Animator animator;

    private static          bool isDisplayed;
    private static readonly int  FullCombo = Animator.StringToHash("FullCombo");
    private static readonly int  Clear     = Animator.StringToHash("Clear");

    private void Start()
    {
        isDisplayed = false;
    }

    /// <summary>
    /// クリア・フルコンボアニメーションを表示する
    /// </summary>
    public static void Show()
    {
        if (isDisplayed) return;

        isDisplayed = true;

        //FullCombo表示
        if (NotesJudgementBase.TotalGrades[2] == 0 &&
            NotesJudgementBase.TotalGrades[3] == 0 &&
            NotesJudgementBase.TotalGrades[4] == 0)
        {
            Instance.animator.SetTrigger(FullCombo);
        }
        //クリア表示
        else if (NotesJudgementBase.bestCombo > 0)
        {
            Instance.animator.SetTrigger(Clear);
        }
    }
}
