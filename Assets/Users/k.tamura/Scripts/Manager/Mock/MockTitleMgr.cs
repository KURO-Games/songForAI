using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// モック版用スクリプト
/// </summary>
public class MockTitleMgr : MonoBehaviour
{
    public void StartPush()
    {
        MockSceneMgr.LoadScene("MockHome");
    }
}
