using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectCharactor : MonoBehaviour
{
    [SerializeField] GameObject[] charaAnchor = new GameObject[3];

    private float lastMousePosX;

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, 10f, 1);

        if(hit)
        {
            GameObject clickObj = hit.transform.gameObject;

            if ((clickObj != null) && (clickObj.tag == "Right"))
            {
                float distance = lastMousePosX - Input.mousePosition.x;

                Vector3 movePosition = new Vector3(distance, 0, 0);

                for (int i = 0; i < 3; i++)
                {
                    charaAnchor[i].transform.position += movePosition;
                }

                lastMousePosX = Input.mousePosition.x;
            }
        }
    }
}
