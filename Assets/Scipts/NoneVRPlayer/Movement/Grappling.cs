using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerMovement))]
[RequireComponent(typeof(Swinging))]
[RequireComponent(typeof(GrapplePointPrediction))]
public class Grappling : MonoBehaviour
{
    [Header("Input")] 
    [SerializeField] private InputActionReference grapplingControl;
    
    [Header("References")] 
    [SerializeField] private Transform cam;
    [SerializeField] private Transform gunTip;
    [SerializeField] private LineRenderer lr;
    
    private PlayerMovement pM;
    private GrapplePointPrediction gPP;
    private Rigidbody rb;

    [Header("Grappling")]
    [SerializeField] private float grappleDelayTime;
    [SerializeField] private float overshootYAxis;
    private Vector3 grapplePoint;
    private bool grappling;
    private float maxGrapplingDistance;
    private bool isThrowing;
    
    private bool enableMovementOnNextTouch;
    private Vector3 velocityToSet;
    
    [Header("FOV")]
    [SerializeField] private FirstPersonCamera fCam;
    [SerializeField] private float grapplingFOV;
    
    [Header("Cooldown")] 
    [SerializeField] private float grapplingCooldown;
    private float grapplingCdTimer;
    
    private void OnEnable()
    {
        grapplingControl.action.Enable();
    }

    private void OnDisable()
    {
        grapplingControl.action.Disable();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        pM = GetComponent<PlayerMovement>();
        gPP = GetComponent<GrapplePointPrediction>();
        maxGrapplingDistance = gPP.GetMaxSwingGrapplingDistance();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    { 
        if (grapplingControl.action.triggered && !isThrowing)
        {
            StartGrapple();
        }

        if (grapplingCdTimer > 0)
        {
            grapplingCdTimer -= Time.deltaTime;
        }
        
    }

    private void LateUpdate()
    {
        if (grappling)
        {
            lr.SetPosition(0, gunTip.position);
        }
    }

    private void StartGrapple()
    {
        if (grapplingCdTimer > 0)
        {
            return;
        }

        if (gPP.GetPredictionHitPoint().point != Vector3.zero)
        {
            grappling = true;
            
            pM.SetFreeze(true);
            
            grapplePoint = gPP.GetPredictionHitPoint().point;
            Invoke(nameof(ExecuteGrapple), grappleDelayTime);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrapplingDistance;
            Invoke(nameof(StopGrapple), grappleDelayTime);
        }
        
        lr.enabled = true;
        lr.SetPosition(1, grapplePoint);
    }

    private void ExecuteGrapple()
    {
        pM.SetFreeze(false);

        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0)
        {
            highestPointOnArc = overshootYAxis;
        }
        JumpToPosition(grapplePoint, highestPointOnArc);
        Invoke(nameof(StopGrapple), 1f);
    }
    
    public void StopGrapple()
    {
        pM.SetFreeze(false);
        grappling = false;
        grapplingCdTimer = grapplingCooldown;
        lr.enabled = false;
    }
    
        
    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        pM.SetActiveGrapple(true);
        
        velocityToSet = CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight);
        Invoke(nameof(SetVelocity), 0.1f);
        
        // ensure that movement gets enabled
        Invoke(nameof(ResetRestriction), 3f);
    }

    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;
        rb.velocity = velocityToSet;
        fCam.DoFov(grapplingFOV);
    }

    private void ResetRestriction()
    {
        pM.SetActiveGrapple(false);
        fCam.DoFov(80f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            ResetRestriction();
            GetComponent<Grappling>().StopGrapple();
        }
    }

    private Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);
        
        Vector3 velocityY = Vector3.up * (float)Math.Sqrt(-2 * gravity * trajectoryHeight);
        Vector3 velocityXZ = displacementXZ / (float)(Math.Sqrt(-2 * trajectoryHeight / gravity) + Math.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        return velocityXZ + velocityY;
    }
    
    public void SetIsThrowing(bool status)
    {
        isThrowing = status;
    }
}
