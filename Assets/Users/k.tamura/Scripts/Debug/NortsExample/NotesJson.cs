using System;
using System.Collections;
using System.Collections.Generic;
[Serializable]
public struct NotesJson
{
    [Serializable]
    public struct MusicData
    {

        public string name;
        public int maxBlock;
        public int BPM;
        public double offset;
        public Notes[] notes;
    }
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

