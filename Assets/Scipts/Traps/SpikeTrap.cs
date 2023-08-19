using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    private Animator anim;  // Reference to the Animator component
    private int currentActions = 0;  // Counter for the number of trap actions
    [SerializeField] private int spikeID;  // Identifier for the spike trap

    // Called when the script is enabled
    private void OnEnable()
    {
        // Subscribe to the OnSpikeButtonPressed event
        EventManager.OnSpikeButtonPressed += ActivateTrap;
    }

    // Called when the script is disabled
    private void OnDisable()
    {
        // Unsubscribe from the OnSpikeButtonPressed event
        EventManager.OnSpikeButtonPressed -= ActivateTrap;
    }

    // Called when the script starts
    private void Start()
    {
        // Get the Animator component attached to this GameObject
        anim = GetComponent<Animator>();
    }

    // Method to activate the spike trap
    private void ActivateTrap(int id)
    {
        currentActions++;

        // Check if the provided spike ID matches the trap's spike ID
        if (spikeID == id)
        {
            // Set the "SpikeOut" parameter in the Animator to true
            anim.SetBool("SpikeOut", true);

            // Schedule the ResetTrap method to be called after 5 seconds
            Invoke(nameof(ResetTrap), 5f);
        }
    }

    // Method to reset the spike trap
    private void ResetTrap()
    {
        // Set the "SpikeOut" parameter in the Animator to false
        anim.SetBool("SpikeOut", false);
    }
}