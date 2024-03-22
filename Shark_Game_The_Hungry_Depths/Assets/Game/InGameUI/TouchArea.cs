using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchArea : MonoBehaviour, IPointerDownHandler
{

    public bool pointerDown = false;

    public void OnPointerDown(PointerEventData eventData)
    {
        pointerDown = true;
    }


    private void OnMouseDown()
    {
        pointerDown = true;
    }

    private void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            pointerDown = false;
        }
    }
}
