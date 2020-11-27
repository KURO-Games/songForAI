using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCharactor : MonoBehaviour
{
    [SerializeField] GameObject[] charactor = new GameObject[3];
    [SerializeField] Sprite[] charaUI = new Sprite[3];
    private Vector3[] defaultPosition = new Vector3[3];
    private float lastMousePosX = 0;
    private bool isSwiping = false;
    private int centerNum = 0;// gameTypeと同義
    private int shiftNum = 0;

    private void Start()
    {
        for (int i = 0; i < charactor.Length; i++)
            defaultPosition[i] = charactor[i].transform.position;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10f, 1);

        if ((Input.GetMouseButtonDown(0)) && (hit))
        {
            if ((hit.transform.gameObject != null) && (hit.transform.gameObject.tag == "Right"))
            {
                lastMousePosX = Input.mousePosition.x;
                isSwiping = true;
            } 
        }

        if((Input.GetMouseButton(0)) && (isSwiping))
        {
            float distance = lastMousePosX - Input.mousePosition.x;

            Vector3 movePosition = new Vector3(distance * 2.5f, 0, 0);

            for (int i = 0; i < charactor.Length; i++)
            {
                charactor[i].transform.position -= movePosition;
            }

            lastMousePosX = Input.mousePosition.x;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isSwiping = false;

            for (int i = 0; i < charactor.Length; i++)
            {
                charactor[i].transform.position = defaultPosition[i];

                if (shiftNum == charactor.Length)
                    shiftNum = 0;

                SpriteRenderer sr = charactor[i].GetComponent<SpriteRenderer>();
                sr.sprite = charaUI[shiftNum];
                shiftNum++;
            }
        }
    }

    public void SetCharactor(bool LorR)
    {
        if(LorR)
        {
            centerNum++;

            if (centerNum == charactor.Length)
                centerNum = 0;
            shiftNum = centerNum;
        }
        else
        {
            centerNum--;

            if (centerNum < 0)
                centerNum = charactor.Length - 1;
            shiftNum = centerNum;
        }
    }
}
