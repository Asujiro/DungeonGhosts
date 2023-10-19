using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    private Animator anim;

    // Subscribe to the event when this script is enabled
    private void OnEnable()
    {
        EventManager.OnResetAllDoorTrigger += Reset;
    }

    // Unsubscribe from the event when this script is disabled
    private void OnDisable()
    {
        EventManager.OnResetAllDoorTrigger -= Reset;
    }

    private bool wasPressed = false;

    private void Start()
    {
        // Get the Animator component attached to this GameObject
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider's GameObject has the "Player" tag and the button hasn't been pressed
        if (other.gameObject.CompareTag("Player") && !wasPressed)
        {
            // Mark the button as pressed
            wasPressed = true;

            // Trigger the "PressButton" animation state
            anim.SetBool("PressButton", true);

            // Invoke the event to indicate that the button has been triggered
            EventManager.TriggerButton();
        }
    }

    private void Reset()
    {
        // Reset the button's state when the event is invoked
        wasPressed = false;
        anim.SetBool("PressButton", false);
    }
}