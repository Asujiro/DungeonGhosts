using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
public class WallRunning : MonoBehaviour
{
    // Wall-running settings
    [Header("Wall-Running")]
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float wallRunForce;
    [SerializeField] private float maxWallRunTime; 
    [SerializeField] private float wallClimbSpeed;
    private float wallRunTimer;
    private bool upwardsRunning;
    private bool downwardsRunning;

    // Wall-jumping settings
    [Header("Wall-Jumping")] 
    [SerializeField] private float jumpForceUp;
    [SerializeField] private float jumpForceSide;

    // Exiting wall settings
    [Header("Exiting-Wall")]
    [SerializeField] private float exitingWallTime;
    private float exitingWallTimer;
    private bool exitingWall;

    // Gravity settings
    [Header("Gravity")] 
    [SerializeField] private bool useGravity;
    [SerializeField] private float gravityCounterForce;

    // Input references
    [Header("Input")]
    [SerializeField] private InputActionReference movementControl;
    [SerializeField] private InputActionReference mouseControl;
    [SerializeField] private InputActionReference jumpControl;
    private float horizontalInput;
    private float verticalInput;

    // Wall detection settings
    [Header("Detection")]
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;

    // References
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private FirstPersonCamera fCam;
    private Rigidbody rb;
    private PlayerMovement pM;

    // Enable input actions when the component is enabled
    private void OnEnable()
    {
        movementControl.action.Enable();
        mouseControl.action.Enable();
        jumpControl.action.Enable();
    }

    // Disable input actions when the component is disabled
    private void OnDisable()
    {
        jumpControl.action.Disable();
        mouseControl.action.Disable();
        movementControl.action.Disable();
    }

    // Initialize references
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pM = GetComponent<PlayerMovement>();
    }

    // Update is called once per frame
    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    // FixedUpdate is called at a fixed interval
    private void FixedUpdate()
    {
        if (pM.GetWallRunning())
        {
            WallRunningMovement();
        }
    }

    // Check for walls
    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    // Check if the player is above the ground
    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    // Manage the wall-running state
    private void StateMachine()
    {
        // Getting Inputs
        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        Vector2 mouseMovement = mouseControl.action.ReadValue<Vector2>();
        horizontalInput = movement.x;
        verticalInput = movement.y;
        float mouseY = mouseMovement.y * Time.deltaTime;
        
        // Handle climbing when the mouse is moved up or down
        if (mouseY > 0 && pM.GetWallRunning())
        {
            upwardsRunning = true;
            downwardsRunning = false;
        }
        else if (mouseY < 0 && pM.GetWallRunning())
        {
            downwardsRunning = true;
            upwardsRunning = false;
        }
        
        // State 1 - Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            // Start Wall Run
            if (!pM.GetWallRunning())
            {
                StartWallRun();  
            }

            if (wallRunTimer > 0)
            {
                wallRunTimer -= Time.deltaTime;
            }

            if (wallRunTimer <= 0 && pM.GetWallRunning())
            {
                exitingWall = true;
                exitingWallTimer = exitingWallTime;
            }

            if (jumpControl.action.triggered)
            {
                WallJump();
            }
        }
        // State 2 - Exiting
        else if (exitingWall)
        {
            if (pM.GetWallRunning())
            {
                StopWallRun();
            }

            if (exitingWallTimer > 0)
            {
                exitingWallTimer -= Time.deltaTime;
            }

            if (exitingWallTimer <= 0)
            {
                exitingWall = false;
            }
        }
        // State 3 - None
        else
        {
            if (pM.GetWallRunning())
            {
                StopWallRun();  
            }
        }
    }

    // Apply wall running movement
    private void WallRunningMovement()
    {
        rb.useGravity = useGravity;

        // Determine the wall's normal based on the wall side
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        
        // Ensure that you can move forward on the wall even when it's rotated 
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }
        
        // Apply forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if (upwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        }
        else if (downwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        }
        
        // Apply push to wall force
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force); 
        }
        
        // Weaken gravity
        if (useGravity)
        {
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
        }
    }

    // Start wall run
    private void StartWallRun()
    {
        pM.SetWallRunning(true);

        wallRunTimer = maxWallRunTime;
        
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        
        // Apply camera effects
        fCam.DoFov(90f);
        if (wallLeft) fCam.DoTilt(-5f);
        if (wallRight) fCam.DoTilt(5f);
    }

    // Stop wall run
    private void StopWallRun()
    {
        pM.SetWallRunning(false);
        fCam.DoFov(80f);
        fCam.DoTilt(0f);
    }

    // Perform a wall jump
    private void WallJump()
    {
        // Enter exiting wall state
        exitingWall = true;
        exitingWallTimer = exitingWallTime;
        
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * jumpForceUp + wallNormal * jumpForceSide;
        
        // Reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // Add force
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }
}
