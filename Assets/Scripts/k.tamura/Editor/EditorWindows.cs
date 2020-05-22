using UnityEngine;
using UnityEditor;

public class EditorWindows : EditorWindow
{
    [MenuItem("Editor/EditorWindows")]
    private static void Create()
    {
        // 生成
        GetWindow<EditorWindows>("EditorWindows");
    }
}