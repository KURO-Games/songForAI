using System;
using System.Collections;
using System.Collections.Generic;
/// <summary>
/// ノーツデータクラス
/// </summary>
[Serializable]
public struct NotesJson
{
    /// <summary>
    /// NotesData
    /// </summary>
    [Serializable]
    public struct MusicData
    {

        public string name;
        public int maxBlock;
        public int BPM;
        public double offset;
        public Notes[] notes;
    }
    /// <summary>
    /// notes 
    /// </summary>
    [Serializable]
    public struct Notes
    {
        public int LPB;
        public int type;
        public int block;
        public int num;
        public Notes[] notes;
    }
}

