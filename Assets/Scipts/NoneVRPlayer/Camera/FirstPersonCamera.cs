using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class FirstPersonCamera : MonoBehaviour
{
    [SerializeField] private float sensX;
    [SerializeField] private float sensY;

    [SerializeField] private Transform orientation;
    [SerializeField] private Transform camHolder;

    [SerializeField] private InputActionReference mouseControl;

    private float xRotation;
    private float yRotation;

    private void OnEnable()
    {
        // Enable mouse control input action
        mouseControl.action.Enable();
    }

    private void OnDisable()
    {
        // Disable mouse control input action
        mouseControl.action.Disable();
    }

    private void Start()
    {
        // Lock cursor and hide it
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        // Read mouse movement input
        Vector2 mouseMovement = mouseControl.action.ReadValue<Vector2>();
        float mouseX = mouseMovement.x * Time.fixedDeltaTime * sensX;
        float mouseY = mouseMovement.y * Time.fixedDeltaTime * sensY;

        // Update rotations based on mouse movement
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate camera holder and orientation
        camHolder.rotation = Quaternion.Euler(xRotation, yRotation, 0);
        orientation.rotation = Quaternion.Euler(0, yRotation, 0);
    }

    public void DoFov(float endValue)
    {
        // Change camera field of view using DOTween
        GetComponent<Camera>().DOFieldOfView(endValue, 0.25f);
    }

    public void DoTilt(float zTilt)
    {
        // Tilt the camera using DOTween
        transform.DOLocalRotate(new Vector3(0, 0, zTilt), 0.25f);
    }
}