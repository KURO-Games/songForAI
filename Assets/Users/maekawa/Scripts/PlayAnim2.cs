using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAnim2 : MonoBehaviour
{
    // エフェクトテスト用
    [SerializeField] GameObject obj;
    Animator animator;
    void Start()
    {
        animator = obj.GetComponent<Animator>();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = 10.0f;
            Vector3 objPos = Camera.main.ScreenToWorldPoint(mousePos);

            obj.transform.position = objPos;
            animator.Play("holdEffect", 0, 0);
            animator.SetBool("isEnded", false);
        }
        if (Input.GetMouseButtonUp(1))
        {
            animator.SetBool("isEnded", true);
        }
    }
}
