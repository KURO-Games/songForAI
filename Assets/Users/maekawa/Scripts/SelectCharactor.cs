using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCharactor : MonoBehaviour
{
    [SerializeField] GameObject[] charactor = new GameObject[3];
    private Vector2[] charaPosition = new Vector2[3];
    private float mousePositionX;
    private float lastMousePosX;
    private bool isSwiping;

    public static int gameType;

    private void Start()
    {
        gameType = 0;

        for (int i = 0; i < charactor.Length; i++)
            charaPosition[i] = charactor[i].transform.position;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10f, 1);

        if (Input.GetMouseButtonDown(0))
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

            Vector3 movePosition = new Vector3(distance * 3, 0, 0);

            for (int i = 0; i < charactor.Length; i++)
            {
                charactor[i].transform.position -= movePosition;
            }

            lastMousePosX = Input.mousePosition.x;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isSwiping = false;

            if(charactor[0].transform.localPosition.x < 150)
            {

            }
            else if(charactor[0].transform.localPosition.x > 850)
            {

            }
            else
            {
                for(int i = 0; i < charactor.Length; i++)
                    charactor[i].transform.position = charaPosition[i];
            }
        }
    }

    public void SetCharactor(int CharaNumber)
    {
    }
}
