using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCamera : MonoBehaviour
{
    [SerializeField] private Transform cameraPosition; // Reference to the target camera position

    // Update is called once per frame
    void Update()
    {
        // Move the camera to the cameraPosition
        transform.position = cameraPosition.position;
    }
}