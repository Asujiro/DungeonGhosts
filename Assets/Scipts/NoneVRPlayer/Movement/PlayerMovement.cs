using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Inputs
    [Header("Inputs")]
    [SerializeField] private InputActionReference movementControl;
    [SerializeField] private InputActionReference sprintControl;
    [SerializeField] private InputActionReference jumpControl;
    [SerializeField] private InputActionReference crouchControl;
    [SerializeField] private InputAction switchModeSwing;
    [SerializeField] private InputAction switchModeThrow;

    // Movement variables
    private Vector2 movement;
    private float horizontalInput;
    private float verticalInput;
    private float moveSpeed;
    private bool crouching;
    private float startYScale;

    // Speed settings
    [Header("Movement")] 
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float wallRunSpeed;
    [SerializeField] private Transform orientation;
    [SerializeField] private float groundDrag;
    [SerializeField] private float swingSpeed;
    [SerializeField] private float iceGroundDrag;
    private float drag;
    private bool wallRunning;
    private bool swinging;
    private bool freeze;
    private bool activeGrapple;

    // Crouching settings
    [Header("Crouching")] 
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;

    // Jumping settings
    [Header("Jumping")] 
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    private bool readyToJump;
    private bool exitingSlope;

    // Ground check settings
    [Header("Ground Check")] 
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsIce;
    private bool grounded;
    private bool groundedOnIce;

    // Slope handling
    [Header("Slope Handling")] 
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;

    // Switch Mouse Mode settings
    private Grappling grap;
    private Swinging swing;
    private GrapplePointPrediction grapPoint;
    private Throwing throwning;

    private Vector3 moveDirection;

    private Rigidbody rb;

    private MovementState state;

    // Methods

    // Enable input actions when the component is enabled
    private void OnEnable()
    {
        // Enable input actions
        movementControl.action.Enable();
        jumpControl.action.Enable();
        sprintControl.action.Enable();
        crouchControl.action.Enable();
        switchModeSwing.Enable();
        switchModeSwing.performed += SwitchToSwingMode;
        switchModeThrow.Enable();
        switchModeThrow.performed += SwitchToThrowing;
    }
    
    // Disable input actions when the component is disabled
    private void OnDisable()
    {
        // Disable input actions
        movementControl.action.Disable();
        jumpControl.action.Disable();
        sprintControl.action.Disable();
        crouchControl.action.Disable();
        switchModeSwing.Disable();
        switchModeSwing.performed -= SwitchToSwingMode;
        switchModeThrow.Disable();
        switchModeThrow.performed -= SwitchToThrowing;
    }

    // Initialize components and variables
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;
        swinging = false;
        swing = GetComponent<Swinging>();
        grap = GetComponent<Grappling>();
        grapPoint = GetComponent<GrapplePointPrediction>();
        throwning = GetComponent<Throwing>();
    }

    // Update is called once per frame
    private void Update()
    {
        // Ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        groundedOnIce = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsIce);
        MyInput();
        SpeedControl();
        StateHandler();
        // Debug.Log(moveSpeed);
        // Debug.Log(state);
        
        // Handle drag
        DragHandler();
    }

    // FixedUpdate is called at a fixed interval
    private void FixedUpdate()
    {
        MovePlayer();
    }

    // Enumeration for movement states
    private enum MovementState
    {
        Freeze,
        Walking,
        Sprinting,
        WallRunning,
        Swinging,
        Crouching,
        Ice,
        Air
    }

    // Handle user input
    private void MyInput()
    {
        movement = movementControl.action.ReadValue<Vector2>();
        horizontalInput = movement.x;
        verticalInput = movement.y;

        if (jumpControl.action.inProgress && readyToJump && (grounded || groundedOnIce))
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        if (crouchControl.action.inProgress)
        {
            transform.localScale = new Vector3(transform.localScale.x, crouchYScale, transform.localScale.z);
            if (!crouching)
            {
                rb.AddForce(Vector3.down * 0.5f, ForceMode.Impulse);
                crouching = true;
            }
        }

        if (!crouchControl.action.inProgress)
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            crouching = false;
        }
    }

    // Handle movement state based on conditions
    private void StateHandler()
    {
        // Mode - Freeze
        if (freeze)
        {
            state = MovementState.Freeze;
            moveSpeed = 0;
            rb.velocity = Vector3.zero;
        }
        
        // Mode - Sprinting 
        else if (sprintControl.action.inProgress && grounded || groundedOnIce)
        {
            state = MovementState.Sprinting;
            moveSpeed = sprintSpeed;
        }
        
        // Mode - Swinging
        else if (swinging)
        {
            state = MovementState.Swinging;
            moveSpeed = swingSpeed;
        }
        
        // Mode - Wall-Running
        else if (wallRunning)
        {
            state = MovementState.WallRunning;
            moveSpeed = wallRunSpeed;
        }
        
        // Mode - Crouching
        else if (crouchControl.action.inProgress)
        {
            state = MovementState.Crouching;
            moveSpeed = crouchSpeed;
        }
        
        // Mode - Walking
        else if (grounded)
        {
            state = MovementState.Walking;
            moveSpeed = walkSpeed;
            drag = groundDrag;
        }
        
        else if (groundedOnIce)
        {
            state = MovementState.Ice;
            moveSpeed = walkSpeed;
            drag = iceGroundDrag;
        }
        
        // Mode - Air
        else
        {
            state = MovementState.Air;
        }
    }

    // Move the player based on the movement state
    private void MovePlayer()
    {
        if (swinging || activeGrapple) return;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * (moveSpeed * 20f), ForceMode.Force);
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }

        // On ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * (moveSpeed * 10f), ForceMode.Force);
            rb.AddForce(Vector3.down * 2f, ForceMode.Force);
        }       
        // In air
        else if (!grounded)
        {
            rb.AddForce(moveDirection.normalized * (moveSpeed * 10f * airMultiplier), ForceMode.Force);
        }

        // Turn off gravity while on slope
        if (!wallRunning)
        {
            rb.useGravity = !OnSlope();
        }
    }
    
    // Control speed based on conditions
    private void SpeedControl()
    {
        if (activeGrapple)
        {
            return;
        }

        // Limit speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        // Limit speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            
            // Limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limitedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limitedVel.x, rb.velocity.y, limitedVel.z);
            } 
        }
    }

    // Handle drag based on conditions
    private void DragHandler()
    {
        if (grounded && !activeGrapple)
        {
            rb.drag = drag;
        }
        else
        {
            rb.drag = 0;
        }
    }

    // Handle jump
    private void Jump()
    {
        exitingSlope = true;
        // Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
    }

    // Reset jump state
    private void ResetJump()
    {
        exitingSlope = false;
        readyToJump = true;
    }

    // Check if the player is on a slope
    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }

    // Get the movement direction on a slope
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    // Set wall running status
    public void SetWallRunning(bool status)
    {
        wallRunning = status;
    }

    // Get wall running status
    public bool GetWallRunning()
    {
        return wallRunning;
    }

    // Set swinging status
    public void SetSwinging(bool swing)
    {
        swinging = swing;
    }

    // Get swinging status
    public bool GetSwinging()
    {
        return swinging;
    }

    // Set freeze status
    public void SetFreeze(bool status)
    {
        freeze = status;
    }

    // Set active grapple status
    public void SetActiveGrapple(bool status)
    {
        activeGrapple = status;
    }

    // Get active grapple status
    public bool GetActiveGrapple()
    {
        return activeGrapple;
    }

    // Switch to swing mode input handler
    private void SwitchToSwingMode(InputAction.CallbackContext obj)
    {
        throwning.SetIsSwinging(true);
        swing.SetIsThrowing(false);
        grap.SetIsThrowing(false);
        grapPoint.enabled = true;
        Debug.Log("Swinging Mode");
        EventManager.SwitchToolTrigger(1);
    }

    // Switch to throwing mode input handler
    private void SwitchToThrowing(InputAction.CallbackContext callbackContext)
    {
        Debug.Log("Throwing Mode");
        throwning.SetIsSwinging(false);
        swing.SetIsThrowing(true);
        grap.SetIsThrowing(true);
        grapPoint.enabled = false;
        EventManager.SwitchToolTrigger(2);
    }
}
