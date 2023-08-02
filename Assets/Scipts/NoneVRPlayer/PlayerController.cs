using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    
    private CharacterController controller;
    private Vector3 playerVelocity;
    private bool groundedPlayer;
    
    [Header("Movement")]
    [SerializeField] private float playerSpeed = 2.0f;
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    [SerializeField] private float wallRunSpeed;
    [SerializeField] private Transform cameraTransform;
    
    [Header("Jumping")]
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private float rotationSpeed = 4f;
    [SerializeField]private float playerHeight;
    [SerializeField] private float jumpCooldown;
    [SerializeField] private float airMultiplier;
    [SerializeField]private LayerMask whatIsGround;
    
    private bool grounded;
    private bool readyToJump = true;
    private bool pressed;

    [Header("Inputs")]
    [SerializeField] private InputActionReference movementControl;
    [SerializeField] private InputActionReference sprintControl;
    [SerializeField] private InputActionReference jumpControl;
    
    private MovementState state;
    private enum MovementState
    {
        walking,
        sprinting,
        air,
    }
    private void OnEnable()
    {
        movementControl.action.Enable();
        jumpControl.action.Enable();
        sprintControl.action.Enable();
    }

    private void OnDisable()
    {
        movementControl.action.Disable();
        jumpControl.action.Disable();
        sprintControl.action.Enable();
    }


    private void Start()
    {   
        controller = gameObject.GetComponent<CharacterController>();
        jumpControl.action.performed += _ => pressed = true;
        jumpControl.action.canceled += _ => pressed = false;
    }
    void Update()
    {
        grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);
        Jump();
        Movement();
        StateHandler();
        
    }

    private void Movement()
    {
        Vector2 movement = movementControl.action.ReadValue<Vector2>();
        Vector3 move = new Vector3(movement.x, 0, movement.y);
        move = cameraTransform.forward * move.z + cameraTransform.right * move.x;
        move.y = 0f;
        if (grounded)
        {
            controller.Move(move * Time.deltaTime * playerSpeed); 
        }
        else if(!grounded)
        { 
            controller.Move(move * Time.deltaTime * playerSpeed * airMultiplier);
        }
        
        if (movement != Vector2.zero)
        {
            float targetAngle = Mathf.Atan2(movement.x, movement.y) * Mathf.Rad2Deg + cameraTransform.eulerAngles.y;
            Quaternion rotation = Quaternion.Euler(0f, targetAngle, 0f);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation, Time.deltaTime * rotationSpeed);
        }
    }

    private void Jump()
    {
        
        if (grounded && playerVelocity.y < 0)
        {
            playerVelocity = new Vector3(playerVelocity.x, 0f, playerVelocity.z);
        }
        
        // Changes the height position of the player..
        if (pressed && grounded && readyToJump)
        {
            Debug.Log("jump");
            readyToJump = false;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            Invoke(nameof(ResetJump), jumpCooldown);
        }

        playerVelocity.y += gravityValue * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }

    private void StateHandler()
    {
        // Mode - Sprinting
        if (grounded && sprintControl.action.inProgress)
        {
            state = MovementState.sprinting;
            playerSpeed = sprintSpeed;
        }
        else if(grounded)
        {
            state = MovementState.walking;
            playerSpeed = walkSpeed;
        }
        else
        {
            
        }
    }

}

