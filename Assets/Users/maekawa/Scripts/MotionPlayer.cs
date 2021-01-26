using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Framework.Motion;

public class MotionPlayer : MonoBehaviour
{
    CubismMotionController _motionController;

    private void Start()
    {
        _motionController = GetComponent<CubismMotionController>();
    }

    public void PlayMotion(AnimationClip animation)
    {
        if ((_motionController == null) || (animation == null))
        {
            return;
        }

        _motionController.PlayAnimation(animation, isLoop: false);
    }
}
