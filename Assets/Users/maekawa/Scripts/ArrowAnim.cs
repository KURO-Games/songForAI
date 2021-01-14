using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArrowAnim : MonoBehaviour
{
    [SerializeField] Animator rightArrowAnim;
    [SerializeField] Animator leftArrowAnim;

    private float time = 0;
    void Start()
    {
        //rightArrowAnim.SetBool("isElapse", true);
        //rightArrowAnim.Play("rightArrow");
    }

    void Update()
    {
        ////bool isElapse = rightArrowAnim.GetBool("isElapse");
        //if (time > 3)
        //{
        //    time = 0;
        //    rightArrowAnim.SetBool("isElapse", true);
        //    leftArrowAnim.SetBool("isElapse", true);
        //}
        //else
        //{
        //    //rightArrowAnim.SetBool("isElapse", false);
        //    //leftArrowAnim.SetBool("isElapse", false);
        //    time += Time.deltaTime;
        //}
    }
}
