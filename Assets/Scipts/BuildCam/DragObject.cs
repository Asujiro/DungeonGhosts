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
    [SerializeField] private float scrollSpeed;

    private void Awake()
    {
        mainCamera = Camera.main;
    }

    private void OnEnable()
    {
        // Enable input actions and attach event handlers
        rotateControl.Enable();
        mouseWheelUp.Enable();
        mouseWheelDown.Enable();
        mouseClick.Enable();
        mouseControl.Enable();
        rotateControl.Enable();
        rotateControlLeft.Enable();

        // Attach the MousePressed method to the mouseClick event
        mouseClick.performed += MousePressed;
    }

    private void OnDisable()
    {
        // Disable input actions and detach event handlers
        rotateControl.Disable();
        mouseControl.Disable();
        mouseWheelUp.Disable();
        mouseWheelDown.Disable();
        mouseClick.Disable();
        rotateControl.Disable();
        rotateControlLeft.Disable();

        // Detach the MousePressed method from the mouseClick event
        mouseClick.performed -= MousePressed;
    }

    private void Update()
    {
        // Update the last cursor position for rotation
        LastCursorPosition();
    }

    private void MousePressed(InputAction.CallbackContext obj)
    {
        // Handle mouse click for dragging objects
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
        // Handle dragging the object
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

            // Handle mouse wheel scrolling to adjust distance
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
            // Handle rotation controls
            else if (rotateControl.triggered)
            {     
                CursorControl.SetPosition(lastCursorPos);

                // Rotate the object around its Y axis
                clickedObject.transform.Rotate(0.0f, 45f, 0.0f, Space.World);
            }
            else if (rotateControlLeft.triggered)
            {
                // Rotate the object around its Y axis in the opposite direction
                clickedObject.transform.Rotate(0.0f, -45f, 0.0f, Space.World);
            }
        }

        iDragComponent?.OnDragEnd();
    }

    private void LastCursorPosition()
    {
        // Update the last cursor position for rotation
        if (rotateControl.triggered)
        {
            lastCursorPos = CursorControl.GetPosition();
        }
    }
}
