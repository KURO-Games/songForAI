using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoEffect : MonoBehaviour
{
    [SerializeField]
    private Animator[] tapEffect = new Animator[8];

    [SerializeField]
    private Animator[] holdEffect = new Animator[8];

    private const int _maxLaneNum = 8;
    private bool[] _isHolding = new bool[8];

    void Update()
    {
        for (int i = 0; i < _maxLaneNum; i++)
        {
            if (NotesJudgementBase.justTap[i])
            {
                tapEffect[i].SetBool("isTapped", true);
                tapEffect[i].Play("tapEffect", 0, 0);
            }

            if (NotesJudgementBase.isHold[i])
            {
                if(!_isHolding[i])
                {
                    holdEffect[i].SetBool("isTapped", true);
                    holdEffect[i].SetBool("isEnded", false);
                    holdEffect[i].Play("holdEffect", 0, 0);
                    _isHolding[i] = true;
                }
            }
            else if(_isHolding[i])
            {
                _isHolding[i] = false;
                holdEffect[i].SetBool("isEnded", true);
            }
        }

        for (int i = 0; i < _maxLaneNum; i++)
        {
            NotesJudgementBase.justTap[i] = false;
            tapEffect[i].SetBool("isTapped", false);
            holdEffect[i].SetBool("isTapped", false);
        }
    }
}
