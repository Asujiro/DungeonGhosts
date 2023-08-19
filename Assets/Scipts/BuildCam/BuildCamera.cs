using UnityEngine;
using UnityEngine.InputSystem;

public class BuildCamera : MonoBehaviour
{
    [SerializeField] private float mouseSense = 1.8f;
    [SerializeField] private float movementSpeed = 10f;
    [SerializeField] private float boostedSpeed = 50f;
    [SerializeField] private bool enableSpeedAcceleration = true;
    [SerializeField] private float speedAccelerationFactor = 1.5f;

    private float currentIncrease = 1;
    private float currentIncreaseMem = 0;
    private Vector3 initPosition;
    private Vector3 initRotation;
    private Vector3 moveDirection;

    [Header("Input")]
    [SerializeField] private InputAction movementControl;
    [SerializeField] private InputAction camRotationControl;
    [SerializeField] private InputAction mouse;
    [SerializeField] private InputAction resetCamButton;
    [SerializeField] private InputAction boostSpeed;
    private Vector2 movement;
    private float verticalInput;
    private float horizontalInput;

    private bool lockMouse;

    private void Start()
    {
        // Initialize initial position and rotation
        initPosition = transform.position;
        initRotation = transform.eulerAngles;
    }

    private void OnEnable()
    {
        // Enable input actions and attach event handler for camera reset
        boostSpeed.Enable();
        resetCamButton.Enable();
        camRotationControl.Enable();
        movementControl.Enable();
        mouse.Enable();
        resetCamButton.performed += ResetCam;
    }

    private void OnDisable()
    {
        // Disable input actions and detach event handler
        boostSpeed.Disable();
        resetCamButton.Disable();
        camRotationControl.Enable();
        movementControl.Disable();
        mouse.Enable();
        resetCamButton.performed -= ResetCam;
    }

    // Apply requested cursor state
    private void SetCursorState()
    {
        switch (lockMouse)
        {
            case false:
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
                break;
            case true:
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
        }
    }

    // Calculate current increase for movement speed
    private void CalculateCurrentIncrease(bool moving)
    {
        currentIncrease = Time.deltaTime;

        if (!enableSpeedAcceleration || enableSpeedAcceleration && !moving)
        {
            currentIncreaseMem = 0;
            return;
        }

        currentIncreaseMem += Time.deltaTime * (speedAccelerationFactor - 1);
        currentIncrease = Time.deltaTime + Mathf.Pow(currentIncreaseMem, 3) * Time.deltaTime;
    }

    private void Update()
    {
        SetCursorState();
        CamMovement();
        CamRotation();
    }

    private void CamMovement()
    {
        float currentSpeed = movementSpeed;

        if (boostSpeed.inProgress)
            currentSpeed = boostedSpeed;

        // Read movement input
        movement = movementControl.ReadValue<Vector2>();
        horizontalInput = movement.x;
        verticalInput = movement.y;

        // Calculate movement direction
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;

        // Calculate acceleration
        CalculateCurrentIncrease(moveDirection != Vector3.zero);

        // Determine if mouse should be locked based on movement
        lockMouse = moveDirection != Vector3.zero;

        // Apply movement
        transform.position += moveDirection * currentSpeed * currentIncrease;
    }

    private void CamRotation()
    {
        if (lockMouse || camRotationControl.inProgress)
        {
            // Pitch (up and down)
            transform.rotation *= Quaternion.AngleAxis(
                -mouse.ReadValue<Vector2>().y * mouseSense * Time.deltaTime,
                Vector3.right
            );

            // Yaw (left and right)
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                transform.eulerAngles.y + mouse.ReadValue<Vector2>().x * mouseSense * Time.deltaTime,
                transform.eulerAngles.z
            );
        }
    }

    private void ResetCam(InputAction.CallbackContext callbackContext)
    {
        // Reset camera position and rotation to initial values
        transform.position = initPosition;
        transform.eulerAngles = initRotation;
    }
}
