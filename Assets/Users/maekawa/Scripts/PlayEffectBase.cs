using UnityEngine;

public abstract class PlayEffectBase : MonoBehaviour
{
    [SerializeField]
    protected Animator[] tapEffect;

    protected int MaxLaneNum;

    protected static readonly int IsTapped = Animator.StringToHash("isTapped");

    /// <summary>
    /// エフェクトの再生処理
    /// </summary>
    protected abstract void PlayEffect();

    protected virtual void Start()
    {
        MaxLaneNum = NotesGeneratorBase.MusicData.maxBlock;
    }

    private void Update()
    {
        PlayEffect();
    }
}
