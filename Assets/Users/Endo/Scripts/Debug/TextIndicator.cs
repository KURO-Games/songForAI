using UnityEngine;

public class TextIndicator : MonoBehaviour
{
    public static string   Content;
    private       GUIStyle _style;

    private void Start()
    {
        _style = new GUIStyle
        {
            fontSize = 48
        };
    }

    private void OnGUI()
    {
        GUI.TextField(new Rect(10, 10, 100, 32), Content, _style);
    }
}
