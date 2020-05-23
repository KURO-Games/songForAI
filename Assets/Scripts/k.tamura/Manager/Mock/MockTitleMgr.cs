using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MockTitleMgr : MonoBehaviour
{
    public void StartPush()
    {
        MockSceneMgr.LoadScene("MockHome");
    }
}
