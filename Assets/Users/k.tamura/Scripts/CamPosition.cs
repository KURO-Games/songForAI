using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// カメラの描画距離を現在の画面サイズに合わせて描画するプログラム。
/// </summary>
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
