using UnityEngine;
/// <summary>
/// InitializeScene
/// </summary>
public class Initialize : MonoBehaviour
{
    void Start()
    {
        Application.targetFrameRate = 60;
        SceneLoadManager.LoadScene("Title");   
    }
}
