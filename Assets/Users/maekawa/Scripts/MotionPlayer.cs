using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Live2D.Cubism.Framework.Motion;
using Live2D.Cubism.Rendering;

public class MotionPlayer : MonoBehaviour
{
    [SerializeField]
    private AnimationClip[] _motions = new AnimationClip[3];
    private CubismMotionController _motionController;
    private CubismRenderController _renderController;
    private int _lastMotionNum = -1;
    public static int motionNum = -1;
    public static bool isNamed = false;

    private void Start()
    {
        motionNum = -1;
        isNamed = false;
        _motionController = GetComponent<CubismMotionController>();
        _renderController = GetComponent<CubismRenderController>();
    }
    private void LateUpdate()
    {
        // フランツdisActive
        if (motionNum == -1)
            SetOpacity(0);
        // モーション再生
        else if (_lastMotionNum != motionNum)
        {
            _lastMotionNum = motionNum;
            bool isLoop = false;
            // idleならループ
            if (motionNum == 0)
                isLoop = true;
            PlayMotion(_motions[motionNum], isLoop);
        }

        // user名決定時にモーションが止まるので
        if(isNamed)
        {
            PlayMotion(_motions[0], true);
            isNamed = false;
        }
    }

    private void PlayMotion(AnimationClip animation, bool isLoop)
    {
        if ((_motionController == null) || (animation == null))
        {
            return;
        }

        SetOpacity(1);
        _motionController.StopAllAnimation();
        _motionController.PlayAnimation(animation, isLoop: isLoop);
    }

    private void SetOpacity(float alpha)
    {
        _renderController.Opacity = alpha;
    }
}
