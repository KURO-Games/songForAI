using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// ノーツの移動管理クラス
/// </summary>
public class SpeedMgr:NotesManager
{
    [SerializeField]
    CriAtom CriClass=null;
    double TimeAssy=0;
    public static float BPM { get; set; }//BPMを入れるように
    /// <summary>
    /// 初期化
    /// </summary>
    private void Start()
    {
        
    }
    /// <summary>
    /// 監視
    /// </summary>
    private void Update()
    {
        //Debug.LogError("経過時間 : "+TimeAssy);
    }
    /// <summary>
    /// 一定時間に呼ばれる関数。秒数計測用
    /// </summary>
    private void FixedUpdate()
    {
        TimeAssy+=Time.deltaTime;
    }
    /// <summary>
    /// 曲とFPSの監視関数
    /// </summary>
    private void SurvNotes()
    {
        
    }
}
