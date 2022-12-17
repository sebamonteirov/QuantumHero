using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class materials : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = 0.73f;

        transform.position = Camera.main.ScreenToWorldPoint(mousePosition);
    }
}
