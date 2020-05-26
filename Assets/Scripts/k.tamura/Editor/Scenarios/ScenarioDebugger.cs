using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class ScenarioDebugger : EditorWindow
{
    [MenuItem("SingForAI Tools/Scenario/Debbugger")]
    private static void Create()
    {
        // 生成
        GetWindow<EditorWindows>("ScenarioDebbuger");
    }
    private void OnGUI()
    {

    }
}
