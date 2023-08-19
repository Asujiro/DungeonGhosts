using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(GrapplePointPrediction))]
public class Swinging : MonoBehaviour
{
    // Input references
    [Header("Input")]
    [SerializeField] private InputActionReference swingKey;
    [SerializeField] private InputActionReference airMoveControl;
    [SerializeField] private InputActionReference shortenCableControl;

    // Swinging variables
    private Vector3 swingPoint;
    private SpringJoint joint;
    private Vector3 currentGrapplePosition;

    // Swing air movement settings
    [Header("SwingAirMovement")]
    [SerializeField] private float horizontalThrustForce;
    [SerializeField] private float forwardThrustForce;
    [SerializeField] private float extendCableSpeed;

    // References
    [Header("References")]
    [SerializeField] private Transform gunTip, player;
    [SerializeField] private Transform orientation;
    private Rigidbody rb;
    private PlayerMovement pM;
    private LineRenderer lr;
    private bool swinging;
    private GrapplePointPrediction gPP;
    private bool isThrowing = false;

    // Enable input actions when the component is enabled
    private void OnEnable()
    {
        swingKey.action.Enable();
        shortenCableControl.action.Enable();
        airMoveControl.action.Enable();
    }

    // Disable input actions when the component is disabled
    private void OnDisable()
    {
        swingKey.action.Disable();
        airMoveControl.action.Disable();
        shortenCableControl.action.Disable();
    }

    // Initialize references
    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        pM = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        gPP = GetComponent<GrapplePointPrediction>();
    }

    // Update is called once per frame
    void Update()
    {
        swinging = swingKey.action.IsPressed();
        
        // Start swinging when the swing key is pressed and not throwing
        if (swingKey.action.triggered && !isThrowing)
        {
            StartSwing();
        }
        // Stop swinging when the swing key is released
        else if (!swinging)
        {
            StopSwing();
        }

        // Apply swing air movement when swinging and not throwing
        if (swinging && !isThrowing)
        {
            SwingAirMovement(); 
        }
    }

    // LateUpdate is called once per frame after Update
    private void LateUpdate()
    {
        DrawRope();
    }

    // Start the swing
    private void StartSwing()
    {
        // Return if there's no prediction hit point
        if (gPP.GetPredictionHitPoint().point == Vector3.zero)
        {
            return;
        }
        
        pM.SetSwinging(true);
        
        // Set swing point and create SpringJoint
        swingPoint = gPP.GetPredictionHitPoint().point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);

        // Set joint properties
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;
        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        lr.positionCount = 2;
        currentGrapplePosition = gunTip.position;
    }

    // Stop the swing
    public void StopSwing()
    {
        pM.SetSwinging(false);
        lr.positionCount = 0;
        Destroy(joint);
    }

    // Draw the rope between the gun tip and swing point
    private void DrawRope()
    {
        // If not grappling, don't draw rope
        if (!joint){ return;}

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);
        
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }

    // Apply air movement while swinging
    private void SwingAirMovement()
    {
        var movement = airMoveControl.action.ReadValue<Vector2>();
        
        // Left and right movement
        switch (movement.x)
        {
            case > 0:
                rb.AddForce(orientation.right * (horizontalThrustForce * Time.deltaTime));
                break;
            case < 0:
                rb.AddForce(-orientation.right * (horizontalThrustForce * Time.deltaTime));
                break;
        }
        
        // Forward movement
        if (movement.y > 0)
        {
            rb.AddForce(orientation.forward * (forwardThrustForce * Time.deltaTime));
        }
        
        // Shorten cable
        if (shortenCableControl.action.inProgress)
        {
            try
            {
                Vector3 directionPoint = swingPoint - transform.position;
                rb.AddForce(directionPoint.normalized * (forwardThrustForce * Time.deltaTime));

                float distanceFromPoint = Vector3.Distance(transform.position, swingPoint);

                joint.maxDistance = distanceFromPoint * 0.8f;
                joint.minDistance = distanceFromPoint * 0.25f;
            }
            catch (Exception e)
            {
                Debug.Log(e);   
            }
        }
        
        // Extend cable
        if (movement.y < 0)
        {
            try
            {
                float extendedDistanceFromPoint = Vector3.Distance(transform.position, swingPoint) + extendCableSpeed;

                joint.maxDistance = extendedDistanceFromPoint * 0.8f;
                joint.minDistance = extendedDistanceFromPoint * 0.25f;
            }
            catch (Exception e)
            {
                Debug.Log(e);
            }
        }
    }

    // Set the throwing status
    public void SetIsThrowing(bool status)
    {
        isThrowing = status;
    }
}
