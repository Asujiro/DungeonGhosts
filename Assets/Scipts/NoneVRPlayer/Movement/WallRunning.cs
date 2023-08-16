using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;
using Object = UnityEngine.Object;

[RequireComponent(typeof(PlayerMovement))]
public class WallRunning : MonoBehaviour
{
    [Header("Wall-Running")]
    [SerializeField] private LayerMask whatIsWall;
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float wallRunForce;
    [SerializeField] private float maxWallRunTime; 
    [SerializeField] private float wallClimbSpeed;
    private float wallRunTimer;
    private bool upwardsRunning;
    private bool downwardsRunning;

    [Header("Wall-Jumping")] 
    [SerializeField] private float jumpForceUp;
    [SerializeField] private float jumpForceSide;

    [Header("Exiting-Wall")]
    [SerializeField] private float exitingWallTime;
    private float exitingWallTimer;
    private bool exitingWall;

    [Header("Gravity")] 
    [SerializeField] private bool useGravity;
    [SerializeField] private float gravityCounterForce;
    
    [Header("Input")]
    [SerializeField] private InputActionReference movementControl;
    [SerializeField] private InputActionReference mouseControl;
    [SerializeField] private InputActionReference jumpControl;
    private float horizontalInput;
    private float verticalInput;
   
    
    [Header("Detection")]
    [SerializeField] private float wallCheckDistance;
    [SerializeField] private float minJumpHeight;
    private RaycastHit leftWallHit;
    private RaycastHit rightWallHit;
    private bool wallLeft;
    private bool wallRight;
    
    [Header("References")]
    [SerializeField] private Transform orientation;
    [SerializeField] private FirstPersonCamera fCam;
    private Rigidbody rb;
    private PlayerMovement pM;

    private void OnEnable()
    {
        movementControl.action.Enable();
        mouseControl.action.Enable();
        jumpControl.action.Enable();
    }

    private void OnDisable()
    {
        jumpControl.action.Disable();
        mouseControl.action.Disable();
        movementControl.action.Disable();
    }
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        pM = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckForWall();
        StateMachine();
    }

    private void FixedUpdate()
    {
        if (pM.GetWallRunning())
        {
            WallRunningMovement();
        }
    }

    private void CheckForWall()
    {
        wallRight = Physics.Raycast(transform.position, orientation.right, out rightWallHit, wallCheckDistance, whatIsWall);
        wallLeft = Physics.Raycast(transform.position, -orientation.right, out leftWallHit, wallCheckDistance, whatIsWall);
    }

    private bool AboveGround()
    {
        return !Physics.Raycast(transform.position, Vector3.down, minJumpHeight, whatIsGround);
    }

    private void StateMachine()
    {
        //Getting Inputs
        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        Vector2 mouseMovement = mouseControl.action.ReadValue<Vector2>();
        horizontalInput = movement.x;
        verticalInput = movement.y;
        float mouseY = mouseMovement.y * Time.deltaTime;
        
        // handles Climbing when the mouse is moved up or down
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
        
        //State 1 - Wallrunning
        if ((wallLeft || wallRight) && verticalInput > 0 && AboveGround() && !exitingWall)
        {
            //start Wall Run
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

    

    private void WallRunningMovement()
    {
        
        
        rb.useGravity = useGravity;
        
        
        //when the wall is on the right use rightHit when on the left use leftHit
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;
        
        // ensure that you can move forward on the wall even when its rotated 
        Vector3 wallForward = Vector3.Cross(wallNormal, transform.up);

        if ((orientation.forward - wallForward).magnitude > (orientation.forward - -wallForward).magnitude)
        {
            wallForward = -wallForward;
        }
        
        // forward force
        rb.AddForce(wallForward * wallRunForce, ForceMode.Force);

        if (upwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, wallClimbSpeed, rb.velocity.z);
        }
        else if (downwardsRunning)
        {
            rb.velocity = new Vector3(rb.velocity.x, -wallClimbSpeed, rb.velocity.z);
        }
        
        
        // push to wall force
        if (!(wallLeft && horizontalInput > 0) && !(wallRight && horizontalInput < 0))
        {
            rb.AddForce(-wallNormal * 100, ForceMode.Force); 
        }
        
        // weaken gravity
        if (useGravity)
        {
            rb.AddForce(transform.up * gravityCounterForce, ForceMode.Force);
        }
        
    }

    private void StartWallRun()
    {
        
        pM.SetWallRunning(true);

        wallRunTimer = maxWallRunTime;
        
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        
        // apply camera effects
        fCam.DoFov(90f);
        if (wallLeft) fCam.DoTilt(-5f);
        if (wallRight) fCam.DoTilt(5f);
    }
    private void StopWallRun()
    {
        pM.SetWallRunning(false);
        fCam.DoFov(80f);
        fCam.DoTilt(0f);
    }

    private void WallJump()
    {
        // enter exiting wall state
        exitingWall = true;
        exitingWallTimer = exitingWallTime;
        
        Vector3 wallNormal = wallRight ? rightWallHit.normal : leftWallHit.normal;

        Vector3 forceToApply = transform.up * jumpForceUp + wallNormal * jumpForceSide;
        
        // reset y velocity
        rb.velocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
        // Add force
        rb.AddForce(forceToApply, ForceMode.Impulse);
    }

}
