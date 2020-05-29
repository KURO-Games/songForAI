using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NortsTester : MonoBehaviour
{
    [SerializeField]
    float positiony=0.1f;
    private void Update()
    {
        this.transform.position -= new Vector3(0, positiony, 0);
    }
}
