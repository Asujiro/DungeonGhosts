using System;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] private InputActionReference movementControl;
    [SerializeField] private InputActionReference sprintControl;
    [SerializeField] private InputActionReference jumpControl;
    [SerializeField] private InputActionReference crouchControl;
    private Vector2 movement;
    private float horizontalInput;
    private float verticalInput;
    
    
    [Header("Movement")] 
    private float moveSpeed;
    private bool crouching;
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

    //[SerializeField] private float rotationSpeed = 4f;

    [Header("Crouching")] 
    [SerializeField] private float crouchSpeed;
    [SerializeField] private float crouchYScale;
    private float startYScale;
    
    
    [Header("Jumping")] 
    [SerializeField] private float jumpForce;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    private bool readyToJump;
    private bool exitingSlope;
    
    [Header("Ground Check")] 
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private LayerMask whatIsIce;
    [SerializeField] private LayerMask whatIsGrappleable;
    private bool grounded;
    private bool groundedOnIce;
    
    [Header("Slope Handling")] 
    [SerializeField] private float maxSlopeAngle;
    private RaycastHit slopeHit;
    
   

    
    private Vector3 moveDirection;

    private Rigidbody rb;

    private MovementState state;

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
    
    // [SerializeField] private Transform cameraTransform;
    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
        sprintControl.action.Enable();
        crouchControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
        sprintControl.action.Disable();
        crouchControl.action.Disable();
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        readyToJump = true;
        startYScale = transform.localScale.y;
        swinging = false;
    }

    private void Update()
    {
        // ground check
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        groundedOnIce = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsIce);
        MyInput();
        SpeedControl();
        StateHandler();
        Debug.Log(groundedOnIce);
        //Debug.Log(moveSpeed);
        //Debug.Log(state);
        
        //handle drag
        DragHandler();

    }

    private void FixedUpdate()
    {
        MovePlayer();
    }
    
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
        else if(wallRunning)
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
    private void MovePlayer()
    {
        if (swinging || activeGrapple) return;

        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;
        // moveDirection = cameraTransform.forward * movement.y + cameraTransform.right * movement.x;

        if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * (moveSpeed * 20f), ForceMode.Force);
            if (rb.velocity.y > 0)
            {
                rb.AddForce(Vector3.down * 80f, ForceMode.Force);
            }
        }
        
        // on ground
        if (grounded)
        {
            rb.AddForce(moveDirection.normalized * (moveSpeed * 10f), ForceMode.Force);
            rb.AddForce(Vector3.down * 2f, ForceMode.Force);
        }       
        // in air
        else if(!grounded)
        {
            rb.AddForce(moveDirection.normalized * (moveSpeed * 10f * airMultiplier), ForceMode.Force);
        }
        
        // trun off gravity while on slope
        if (!wallRunning)
        {
            rb.useGravity = !OnSlope();
        }
        /*
        if (movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
        */
    }
    
    private void SpeedControl()
    {
        if (activeGrapple)
        {
            return;
        }
        //limiting speed on slope
        if (OnSlope() && !exitingSlope)
        {
            if (rb.velocity.magnitude > moveSpeed)
            {
                rb.velocity = rb.velocity.normalized * moveSpeed;
            }
        }
        //limiting speed on ground or in air
        else
        {
            Vector3 flatVel = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            //limit velocity if needed
            if (flatVel.magnitude > moveSpeed)
            {
                Vector3 limizedVel = flatVel.normalized * moveSpeed;
                rb.velocity = new Vector3(limizedVel.x, rb.velocity.y, limizedVel.z);
            } 
        }
    }

    private void DragHandler()
    {
        if (grounded && !activeGrapple)
            rb.drag = drag;
        else
        {
            rb.drag = 0;
        }
    }

    private void Jump()
    {
        exitingSlope = true;
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, jumpForce, rb.velocity.z);
        
    }

    private void ResetJump()
    {
        exitingSlope = false;
        readyToJump = true;
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight * 0.5f + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }

        return false;
    }
    
    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }
    
    
    

    public void SetWallRunning(bool status)
    {
        wallRunning = status;
    }

    public bool GetWallRunning()
    {
        return wallRunning;
    }

    public void SetSwinging(bool swing)
    {
        swinging = swing;
    }

    public bool GetSwinging()
    {
        return swinging;
    }

    public void SetFreeze(bool status)
    {
        freeze = status;
    }

    public void SetActiveGrapple(bool status)
    {
        activeGrapple = status;
    }

    public bool GetActiveGrapple()
    {
        return activeGrapple;
    }
}