using System;
using System.Collections;
using System.Collections.Generic;
[Serializable]
public struct NotesJson
{
    [Serializable]
    public struct MusicData
    {

        public string characterCode;
        public int miType;
        public string musicName;
        public int bpm;
        public string composer;
        public string notesCreator;
        public double offset;
        public Notes[] notes;
    }
    [Serializable]
    public struct Notes
    {
        public int type;
        public int lane;
        public int num;
        public Notes[] notes;
    }
}

