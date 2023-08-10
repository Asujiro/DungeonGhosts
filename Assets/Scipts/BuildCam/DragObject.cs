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
    [SerializeField] private InputAction rotateRight;
    [SerializeField] private InputAction rotateUp;
    [SerializeField] private InputAction mouseControl;
    private Camera mainCamera;
    private WaitForFixedUpdate waitForFixedUpdate;
    private Vector3 rotation;
    private Vector2 lastCursorPos;
    
    [Header("Values")]
    [SerializeField]private float mouseDragSpeed = 10f;
    [SerializeField] private float rotSpeed = 10f;
    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        rotateUp.Enable();
        mouseWheelUp.Enable();
        mouseWheelDown.Enable();
        mouseClick.Enable();
        mouseControl.Enable();
        rotateRight.Enable();
        mouseClick.performed += MousePressed;
    }

    private void OnDisable()
    {
        rotateUp.Disable();
        mouseControl.Disable();
        mouseWheelUp.Disable();
        mouseWheelDown.Disable();
        mouseClick.Disable();
        rotateRight.Disable();
        mouseClick.performed -= MousePressed;
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
                initialDistance = Vector3.Distance(clickedObject.transform.position - new Vector3(0, 0, 1f),
                    mainCamera.transform.position);
            }

            else if (mouseWheelUp.triggered)
            {
                initialDistance = Vector3.Distance(clickedObject.transform.position + new Vector3(0, 0, 1f),
                    mainCamera.transform.position);
            }
            else if (rotateRight.ReadValue<float>() != 0)
            {     
                    CursorControl.SetPosition(lastCursorPos);
                    float rotSpeed = 20f;
                    
                    float rotx = Input.GetAxis ("Mouse X") * rotSpeed * Mathf.Deg2Rad;
                    float roty = Input.GetAxis ("Mouse Y") * rotSpeed * Mathf.Deg2Rad;
                    clickedObject.transform.RotateAround(Vector3.up, -rotx);
                    clickedObject.transform.RotateAround(Vector3.right, roty);
            }
        }
    }

    private void LastCursorPosition()
    {
        if (rotateRight.triggered)
        {
            lastCursorPos = CursorControl.GetPosition();
        }
    }
    
    

}
