using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicDataSettings : ScriptableObject
{
    public string MusicName;
    public float MusicBPM;
    public string Composer;
    public string NortsDesigner;

	public string GetMusicName()
	{
		return MusicName;
	}

	public float GetMusicBPM()
	{
		return MusicBPM;
	}

	public string GetComposer()
	{
		return Composer;
	}

	public string GetNortsDesigner()
	{
		return NortsDesigner;
	}
}

