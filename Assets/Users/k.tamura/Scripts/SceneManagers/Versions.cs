using UnityEngine;
using UnityEngine.UI;
#if UNITY_IOS
    using System.Runtime.InteropServices;
#endif

#if UNITY_EDITOR
    using UnityEditor;
#endif

/// <summary>
/// UnityVersionController InitializeScene実装　現状iOSのみ対応
/// </summary>
public class Versions : MonoBehaviour
{
    #if UNITY_IOS
        [DllImport("__Internal")]
        static extern string GetBundleVersion();
    #endif

    bool Tap;
	[SerializeField]
	Text version;
	[SerializeField]
	string verstr;
	// Start is called before the first frame update
	void Start()
	{
        DontDestroyOnLoad(this);
		if (verstr != "")
			version.text = "Ver: " + verstr + "." + Application.version + "." + GetBuildNumber();
		else
			version.text = "Ver: " + Application.version + "." + GetBuildNumber();

	}
	public static string GetBuildNumber()
	{
        #if UNITY_EDITOR
            return PlayerSettings.iOS.buildNumber;
        #elif UNITY_IOS
            return GetBundleVersion();
        #else
		    return null;
        #endif
	}
}
