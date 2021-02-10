public class ViolinPlayEffect : PlayEffectBase
{
    protected override void Start()
    {
        // シングルノーツレーンのみのため
        MaxLaneNum = 4;
    }

    protected override void PlayEffect()
    {
        for (int i = 0; i < MaxLaneNum; i++)
        {
            if (NotesJudgementBase.justTap[i])
            {
                tapEffect[i].SetBool(IsTapped, true);
                tapEffect[i].Play("tapEffect", 0, 0);
            }

            NotesJudgementBase.justTap[i] = false;
            tapEffect[i].SetBool(IsTapped, false);
        }
    }
}
