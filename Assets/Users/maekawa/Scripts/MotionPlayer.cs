using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Framework.Motion;
using Live2D.Cubism.Rendering;

public class MotionPlayer : MonoBehaviour
{
    private CubismMotionController _motionController;
    private CubismRenderController _renderController;

    private void Start()
    {
        _motionController = GetComponent<CubismMotionController>();
        _renderController = GetComponent<CubismRenderController>();
    }

    public void PlayMotion(AnimationClip animation, bool isLoop)
    {
        if ((_motionController == null) || (animation == null))
        {
            return;
        }

        _motionController.StopAllAnimation();
        _motionController.PlayAnimation(animation, isLoop: isLoop);
    }

    public void SetOpacity(float alpha)
    {
        _renderController.Opacity = alpha;
    }
}
