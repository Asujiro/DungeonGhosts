using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrapButton : MonoBehaviour
{
    private Animator anim;  // Reference to the Animator component
    [SerializeField] private int spikeID;  // Identifier for the spike button

    // Called when the script starts
    private void Start()
    {
        // Get the Animator component attached to this GameObject
        anim = GetComponent<Animator>();
    }

    // Called when a Collider enters the trigger zone
    private void OnTriggerEnter(Collider other)
    {
        // Check if the entering Collider belongs to the "Player" tag
        if (other.gameObject.CompareTag("Player"))
        {
            // Set the "ButtonTriggered" parameter in the Animator to true
            anim.SetBool("ButtonTriggered", true);

            // Trigger the SpikeButtonPressed event with the provided spike ID
            EventManager.TriggerSpikeButton(spikeID);

            // Schedule the ResetButton method to be called after 5 seconds
            Invoke(nameof(ResetButton), 5f);
        }
    }

    // Method to reset the button animation
    private void ResetButton()
    {
        // Set the "ButtonTriggered" parameter in the Animator to false
        anim.SetBool("ButtonTriggered", false);
    }
}