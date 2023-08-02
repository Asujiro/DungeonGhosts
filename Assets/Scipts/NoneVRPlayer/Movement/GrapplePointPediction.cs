using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerMovement))]
public class GrapplePointPediction : MonoBehaviour
{

    [Header("Hit-Prediction")] 
    private RaycastHit predictionHit;
    [SerializeField] private float predictionRadius;
    [SerializeField] private Transform predictionPoint;
    [SerializeField] private Transform cam;
    [SerializeField] private LayerMask whatIsGrappleable;


    [Header("References")] private PlayerMovement pM;

    [FormerlySerializedAs("maxSwingDistance")]
    [Header("Values")] 
    [SerializeField] private float maxSwingGrapplingDistance;

    private void Start()
    {
        pM = GetComponent<PlayerMovement>();
    }

    private void Update()
    {
        CheckForSwingPoints();
    }

    private void CheckForSwingPoints()
    {
        if (pM.GetSwinging() || pM.GetActiveGrapple())
        {
            return;  
        }
        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, predictionRadius, cam.forward, out sphereCastHit, maxSwingGrapplingDistance,
                whatIsGrappleable);

        RaycastHit raycastHit;
        Physics.Raycast(cam.position, cam.forward, out raycastHit, maxSwingGrapplingDistance, whatIsGrappleable);

        Vector3 realHitPoint;

        // Option 1 - Direct Hit: The player is looking directly at the object
        if (raycastHit.point != Vector3.zero)
        {
            realHitPoint = raycastHit.point;
        }

        // Option 2 - Indirect (predicted) Hit: The Player is looking near an object
        else if (sphereCastHit.point != Vector3.zero)
        {
                realHitPoint = sphereCastHit.point;
        }

        //Option 3 - Miss: nothing is in range to hit
        else
        {
            realHitPoint = Vector3.zero;
        }

        //realHitPoint found
        if (realHitPoint != Vector3.zero)
        {
                predictionPoint.gameObject.SetActive(true);
                predictionPoint.position = realHitPoint;
        }
        else
        {
                predictionPoint.gameObject.SetActive(false);
        }

        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    public RaycastHit GetPredictionHitPoint()
    {
        return predictionHit;
    }

    public float GetMaxSwingGrapplingDistance()
    {
        return maxSwingGrapplingDistance;
    }
}
