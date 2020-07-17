using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class SoundManager:SingletonMonoBehaviour<SoundManager>
{
    [SerializeField]
    CriAtomSource BGMSource = null;
    [SerializeField]
    CriAtomSource SESource = null;
    private void Start()
    {
        DontDestroyOnLoad(this);
    }
    public static void BGMSoundCue(int cueID)
    {
        Instance.BGMSource.Play(cueID);
    }
    public static void SESoundCue(int cueID)
    {
        Instance.SESource.Play(cueID);
    }
}
