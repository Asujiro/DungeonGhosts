using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class DragObject : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputAction mouseClick;
    [SerializeField] private InputAction mouseWheelDown;
    [SerializeField] private InputAction mouseWheelUp;
    [SerializeField] private InputAction rotateControl;
    [SerializeField] private InputAction rotateControlLeft;
    [SerializeField] private InputAction mouseControl;
    private Camera mainCamera;
    private WaitForFixedUpdate waitForFixedUpdate;
    private Vector3 rotation;
    private Vector2 lastCursorPos;
    
    
    [Header("Values")]
    [SerializeField] private float mouseDragSpeed = 10f;
    // [SerializeField] private float rotSpeed = 1f;
    [SerializeField] private float scrollSpeed;
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        rotateControl.Enable();
        mouseWheelUp.Enable();
        mouseWheelDown.Enable();
        mouseClick.Enable();
        mouseControl.Enable();
        rotateControl.Enable();
        mouseClick.performed += MousePressed;
        rotateControlLeft.Enable();
    }

    private void OnDisable()
    {
        rotateControl.Disable();
        mouseControl.Disable();
        mouseWheelUp.Disable();
        mouseWheelDown.Disable();
        mouseClick.Disable();
        rotateControl.Disable();
        mouseClick.performed -= MousePressed;
        rotateControlLeft.Disable();
    }

    private void Update()
    {
        
        LastCursorPosition();
    }

    private void MousePressed(InputAction.CallbackContext obj)
    {
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());

        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider != null && hit.collider.gameObject.CompareTag("Draggable"))
            {
                StartCoroutine(DragUpdate(hit.collider.gameObject));
            }
        }
    }
    private IEnumerator DragUpdate(GameObject clickedObject)
    {
        clickedObject.TryGetComponent<IDrag>(out var iDragComponent);
        iDragComponent?.OnDragStart();
        float initialDistance = Vector3.Distance(clickedObject.transform.position, mainCamera.transform.position);
        clickedObject.TryGetComponent<Rigidbody>(out var rb);
        while (mouseClick.ReadValue<float>() != 0)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (rb != null)
            {
                Vector3 direction = ray.GetPoint(initialDistance) - clickedObject.transform.position;
                rb.velocity = direction * mouseDragSpeed;
                yield return waitForFixedUpdate;
            }
            
            if (mouseWheelDown.triggered)
            {
                initialDistance = Vector3.Distance(clickedObject.transform.position - new Vector3(0, 0, scrollSpeed),
                    mainCamera.transform.position);
            }

            else if (mouseWheelUp.triggered)
            {
                initialDistance = Vector3.Distance(clickedObject.transform.position + new Vector3(0, 0, scrollSpeed),
                    mainCamera.transform.position);
            }
            else if (rotateControl.triggered)
            {     
                    CursorControl.SetPosition(lastCursorPos);
                    
                   // float rotx = mouseControl.ReadValue<Vector2>().x * rotSpeed * Mathf.Deg2Rad;
                   // float roty = mouseControl.ReadValue<Vector2>().y * rotSpeed * Mathf.Deg2Rad;
                   // clickedObject.transform.RotateAround(Vector3.up, -rotx);
                   // clickedObject.transform.RotateAround(Vector3.right, roty);
                   
                   clickedObject.transform.Rotate(0.0f, 45f, 0.0f, Space.World);
            }
            else if (rotateControlLeft.triggered)
            {
                    clickedObject.transform.Rotate(0.0f, -45f, 0.0f, Space.World);
            }
        }
        iDragComponent?.OnDragEnd();
    }

    private void LastCursorPosition()
    {
        if (rotateControl.triggered)
        {
            lastCursorPos = CursorControl.GetPosition();
        }
    }
    
    

}
