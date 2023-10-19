using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Implements the IDrag interface to enable drag-and-drop behavior for this object
public class FlyingObjects : MonoBehaviour, IDrag
{
    // Called when the object starts being dragged
    public void OnDragStart()
    {
        // Enable the Rigidbody's kinematic property to disable physical influences
        gameObject.GetComponent<Rigidbody>().isKinematic = false;
    }

    // Called when the object is no longer being dragged
    public void OnDragEnd()
    {
        // Disable the Rigidbody's kinematic property to enable physical influences
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
    }
}