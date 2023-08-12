using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(GrapplePointPediction))]
public class Swinging : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference swingKey;
    [SerializeField] private InputActionReference airMoveControl;
    [SerializeField] private InputActionReference shortenCableControl;
    
    [Header("Swinging")]
    private Vector3 swingPoint;
    private SpringJoint joint;
    private Vector3 currentGrapplePosition;

    [Header("SwingAirMovement")]
    [SerializeField] private float horizontalTrustForce;
    [SerializeField] private float forwardThrustForce;
    [SerializeField] private float extendCableSpeed;

    [Header("References")]
    [SerializeField] private Transform gunTip, player;
    [SerializeField] private Transform orientation;
    private Rigidbody rb;
    private PlayerMovement pM;
    private LineRenderer lr;
    private bool swinging;
    private GrapplePointPediction gPP;
    private void OnEnable()
    {
        swingKey.action.Enable();
        shortenCableControl.action.Enable();
        airMoveControl.action.Enable();
    }

    private void OnDisable()
    {
        swingKey.action.Disable();
        airMoveControl.action.Disable();
        shortenCableControl.action.Disable();
    }

    private void Start()
    {
        lr = GetComponent<LineRenderer>();
        pM = GetComponent<PlayerMovement>();
        rb = GetComponent<Rigidbody>();
        gPP = GetComponent<GrapplePointPediction>();
    }

    // Update is called once per frame
    void Update()
    {
        swinging = swingKey.action.IsPressed();
        if (swingKey.action.triggered)
        {
            StartSwing();
        }
        else if (!swinging)
        {
            StopSwing();
        }

        if (swinging)
        {
            SwingAirMovement(); 
        }
        
        
    }

    private void LateUpdate()
    {
        DrawRope();
    }


    private void StartSwing()
    {
        if (gPP.GetPredictionHitPoint().point == Vector3.zero)
        {
            return;
        }
        
        pM.SetSwinging(true);
        
        
        swingPoint = gPP.GetPredictionHitPoint().point;
        joint = player.gameObject.AddComponent<SpringJoint>();
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = swingPoint;

        float distanceFromPoint = Vector3.Distance(player.position, swingPoint);
            
        // the distance the grapple will try to keep from the point
        joint.maxDistance = distanceFromPoint * 0.8f;
        joint.minDistance = distanceFromPoint * 0.25f;
            
        // changeable values
        joint.spring = 4.5f;
        joint.damper = 7f;
        joint.massScale = 4.5f;

        lr.positionCount = 2;
        currentGrapplePosition = gunTip.position;
        
    }

    public void StopSwing()
    {
        pM.SetSwinging(false);
        lr.positionCount = 0;
        Destroy(joint);
    }

    private void DrawRope()
    {
        // if not grappling, don't draw rope
        if (!joint){ return;}

        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, swingPoint, Time.deltaTime * 8f);
        
        lr.SetPosition(0, gunTip.position);
        lr.SetPosition(1, swingPoint);
    }

    private void SwingAirMovement()
    {
        var movement = airMoveControl.action.ReadValue<Vector2>();
        
        //left and right movement
        switch (movement.x)
        {
            case > 0:
                rb.AddForce(orientation.right * (horizontalTrustForce * Time.deltaTime));
                break;
            case < 0:
                rb.AddForce(-orientation.right * (horizontalTrustForce * Time.deltaTime));
                break;
        }
        
        // forward movement
        if (movement.y > 0)
        {
            rb.AddForce(orientation.forward * (forwardThrustForce * Time.deltaTime));
        }
        
        // shorten cable
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
                
            }
            
        }
        
        // extend cable
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
                
            }
            
        }
        
    }
    
}
