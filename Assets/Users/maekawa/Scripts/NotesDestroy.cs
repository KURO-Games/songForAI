using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NotesDestroy : MonoBehaviour
{
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            int layerMask = 1;
            float maxDistance = 10f;

            Vector2 mousePosition = Input.mousePosition;

            Ray ray = Camera.main.ScreenPointToRay(mousePosition);

            RaycastHit2D hit = Physics2D.Raycast((Vector2)ray.origin, (Vector2)ray.direction, maxDistance, layerMask);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.tag == ("Notes"))
                {
                    GameObject obj = hit.collider.gameObject;
                    Destroy(obj);
                }
            }
        }
    }
}
