using UnityEngine;
using UnityEditor;

public class EditorWindows : EditorWindow
{
    [MenuItem("SingForAI Tools/Norts/Create")]
    private static void Create()
    {
        // 生成
        GetWindow<EditorWindows>("NortsCreator");
    }
    private void OnGUI()
    {
        
    }
}