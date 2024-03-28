using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShark : MonoBehaviour
{
    float moveSpeed = 5;   

    private void FixedUpdate()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 20;
        Vector3 mousePosWorld = Camera.main.ScreenToWorldPoint(mousePos);
        

        Vector3 dir = (mousePosWorld - transform.position).normalized;
        dir.y = 0;

        if (Input.GetMouseButton(0))
        {
            dir.y = 0.5f;
        }else if (Input.GetMouseButton(1))
        {
            dir.y = -0.5f;
        }

        transform.position += dir * Time.fixedDeltaTime * moveSpeed;
        transform.LookAt(transform.position + dir);
    }


}
