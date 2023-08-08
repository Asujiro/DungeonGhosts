using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class DragObject : MonoBehaviour
{
    private Vector3 mOffset;
    private float mZCoord;
    bool isColliding = false;
    bool mouseDragging = false;



    void OnMouseDown()

    {
        mZCoord = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        
        // Store offset = gameobject world pos - mouse world pos
        mOffset = gameObject.transform.position - GetMouseAsWorldPoint();
    }



    private Vector3 GetMouseAsWorldPoint()
    {
        // Pixel coordinates of mouse (x,y)
        Vector3 mousePoint = Input.mousePosition;
        
        // z coordinate of game object on screen
        mousePoint.z = mZCoord;
        
        // Convert it to world points
        return Camera.main.ScreenToWorldPoint(mousePoint);

    }





    void OnMouseDrag()
    {
        mouseDragging = true;
        if (!isColliding)
        {
            transform.position = GetMouseAsWorldPoint() + mOffset;
        }
    }



    private void OnCollisionEnter(Collision collision)
    {
        if (mouseDragging)
        {
            isColliding = true;
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }



    private void OnMouseUp()

    {

        mouseDragging = false;

        isColliding = false;

    }

}
