using UnityEngine;

public class PianoPlayEffect : PlayEffectBase
{
    [SerializeField]
    private Animator[] holdEffect;

    private bool[] _isHolding;

    protected override void Start()
    {
        base.Start();

        _isHolding = new bool[MaxLaneNum];
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

            if (NotesJudgementBase.isHold[i])
            {
                if (!_isHolding[i])
                {
                    holdEffect[i].SetBool(IsTapped, true);
                    holdEffect[i].SetBool(IsTapped, false);
                    holdEffect[i].Play("holdEffect", 0, 0);
                    _isHolding[i] = true;
                }
            }
            else if (_isHolding[i])
            {
                _isHolding[i] = false;
                holdEffect[i].SetBool(IsEnded, true);
            }
        }

        for (int i = 0; i < MaxLaneNum; i++)
        {
            NotesJudgementBase.justTap[i] = false;
            tapEffect[i].SetBool(IsTapped, false);
            holdEffect[i].SetBool(IsTapped, false);
        }
    }
}
