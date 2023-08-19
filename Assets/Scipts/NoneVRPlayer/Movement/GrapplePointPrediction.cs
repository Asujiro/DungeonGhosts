using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(PlayerMovement))]
public class GrapplePointPrediction : MonoBehaviour
{
    [Header("Hit Prediction")]
    private RaycastHit predictionHit;
    [SerializeField] private float predictionRadius;
    [SerializeField] private Transform predictionPoint;
    [SerializeField] private Transform cam;
    [SerializeField] private LayerMask whatIsGrappleable;

    [Header("References")]
    private PlayerMovement pM;

    [FormerlySerializedAs("maxSwingDistance")]
    [Header("Values")]
    [SerializeField] private float maxSwingGrapplingDistance;

    private void Start()
    {
        pM = GetComponent<PlayerMovement>();
    }

    private void FixedUpdate()
    {
        CheckForSwingPoints();
    }

    // Check for potential grapple points for swinging
    private void CheckForSwingPoints()
    {
        // If player is already swinging or using the grapple, exit this method
        if (pM.GetSwinging() || pM.GetActiveGrapple())
        {
            return;
        }

        // Perform a sphere cast to detect nearby grappleable objects
        RaycastHit sphereCastHit;
        Physics.SphereCast(cam.position, predictionRadius, cam.forward, out sphereCastHit, maxSwingGrapplingDistance, whatIsGrappleable);

        // Perform a raycast to detect direct hits on grappleable objects
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
        // Option 3 - Miss: Nothing is in range to hit
        else
        {
            realHitPoint = Vector3.zero;
        }

        // Display the predicted hit point on the predictionPoint object if valid
        if (realHitPoint != Vector3.zero)
        {
            predictionPoint.gameObject.SetActive(true);
            predictionPoint.position = realHitPoint;
        }
        else
        {
            predictionPoint.gameObject.SetActive(false);
        }

        // Assign the hit point from raycast or spherecast based on conditions
        predictionHit = raycastHit.point == Vector3.zero ? sphereCastHit : raycastHit;
    }

    // Return the hit point information for predictions
    public RaycastHit GetPredictionHitPoint()
    {
        return predictionHit;
    }

    // Return the maximum grappling distance for swing points
    public float GetMaxSwingGrapplingDistance()
    {
        return maxSwingGrapplingDistance;
    }
}
