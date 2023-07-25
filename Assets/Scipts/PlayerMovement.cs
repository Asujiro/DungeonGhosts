using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("Movement")] 
    [SerializeField] private float moveSpeed;

    [SerializeField] private Transform orientation;
    
    private Vector3 moveDirection;

    private Rigidbody rb;
    
    [SerializeField] private InputAction playerControls;

    private void OnEnable()
    {
        playerControls.Enable();
    }

    private void OnDisable()
    {
        playerControls.Disable();
    }
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }
    private void FixedUpdate()
    {
        MovePlayer();
    }

    
    private void MovePlayer()
    {
        // calculate movement direction
        moveDirection = playerControls.ReadValue<Vector3>();
        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

}
