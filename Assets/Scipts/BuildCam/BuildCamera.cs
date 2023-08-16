using UnityEngine;
using UnityEngine.InputSystem;

public class BuildCamera : MonoBehaviour
{
    
    [SerializeField] private float _mouseSense = 1.8f;
    [SerializeField] private float _movementSpeed = 10f;
    [SerializeField] private float _boostedSpeed = 50f;
    [SerializeField] private KeyCode _boostSpeed = KeyCode.LeftShift;
    [SerializeField] private bool _enableSpeedAcceleration = true;
    [SerializeField] private float _speedAccelerationFactor = 1.5f;
    [SerializeField] private KeyCode _initPositonButton = KeyCode.R;
    
    private CursorLockMode _wantedMode;
    private float _currentIncrease = 1;
    private float _currentIncreaseMem = 0;
    private Vector3 _initPosition;
    private Vector3 _initRotation;
    private Vector3 moveDirection;
    
    [Header("Input")] 
    [SerializeField] private InputActionReference movementControl;
    [SerializeField] private InputActionReference camRotationControl;
    [SerializeField] private InputActionReference mouse;
    private Vector2 movement;
    private float verticalInput;
    private float horizontalInput;
    
    
    
    private bool lockMouse;



    private void Start()
    {
        _initPosition = transform.position;
        _initRotation = transform.eulerAngles;
    }

    private void OnEnable()
    {   
        camRotationControl.action.Enable();
        movementControl.action.Enable();
        mouse.action.Enable();
        
    }
    
    private void OnDisable()
    {
        camRotationControl.action.Enable();
        movementControl.action.Disable();
        mouse.action.Enable();
    }

    // Apply requested cursor state
    private void SetCursorState()
    {
        switch (lockMouse)
        {
            case false:
                Cursor.lockState = _wantedMode = CursorLockMode.None;
                break;
            case true:
                _wantedMode = CursorLockMode.Locked;
                break;
        }
        // Apply cursor state
        Cursor.lockState = _wantedMode;
        // Hide cursor when locking
        Cursor.visible = (CursorLockMode.Locked != _wantedMode);
    }

    private void CalculateCurrentIncrease(bool moving)
    {
        _currentIncrease = Time.deltaTime;

        if (!_enableSpeedAcceleration || _enableSpeedAcceleration && !moving)
        {
            _currentIncreaseMem = 0;
            return;
        }

        _currentIncreaseMem += Time.deltaTime * (_speedAccelerationFactor - 1);
        _currentIncrease = Time.deltaTime + Mathf.Pow(_currentIncreaseMem, 3) * Time.deltaTime;
    }

    private void Update()
    {
        SetCursorState();
        
        // Movement
            CamMovement();
        

        // Rotation
            CamRotation();

        // Return to init position
        if (Input.GetKeyDown(_initPositonButton))
        {
            transform.position = _initPosition;
            transform.eulerAngles = _initRotation;
        }
    }

    private void CamMovement()
    {
        float currentSpeed = _movementSpeed;

        if (Input.GetKey(_boostSpeed))
            currentSpeed = _boostedSpeed;
            
        movement = movementControl.action.ReadValue<Vector2>();
        horizontalInput = movement.x;
        verticalInput = movement.y;
            
        moveDirection = transform.forward * verticalInput + transform.right * horizontalInput;
            
           
        // Calc acceleration
        CalculateCurrentIncrease(moveDirection != Vector3.zero);

        if (moveDirection != Vector3.zero)
        {
            lockMouse = true;
        }
        else
        {
            lockMouse = false;
        }
            
            
        transform.position += moveDirection * currentSpeed * _currentIncrease;
    }

    private void CamRotation()
    {
        if (lockMouse || camRotationControl.action.inProgress)
        {
            // Pitch
            transform.rotation *= Quaternion.AngleAxis(
                -mouse.action.ReadValue<Vector2>().y * _mouseSense * Time.deltaTime,
                Vector3.right
            );

            // Paw
            transform.rotation = Quaternion.Euler(
                transform.eulerAngles.x,
                transform.eulerAngles.y + mouse.action.ReadValue<Vector2>().x * _mouseSense * Time.deltaTime,
                transform.eulerAngles.z
            );
        }
    }
}
