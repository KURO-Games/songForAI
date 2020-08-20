using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamPosition : MonoBehaviour
{
    [SerializeField]
    //private Camera _camera;
    //[SerializeField]
    private float baseWidth = 16.0f;
    [SerializeField]
    private float baseHeight = 9.0f;

    void Awake()
    {
        // 幅固定+高さ可変
        var scaleWidth = (Screen.height / this.baseHeight) * (this.baseWidth / Screen.width);
        this.gameObject.GetComponent<Camera>().orthographicSize *= scaleWidth;
    }
}
