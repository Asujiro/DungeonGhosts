using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    private GameObject lastCheckPoint;
    private AudioSource deathSound;

    private void Awake()
    {
        // Get the AudioSource component for death sound
        deathSound = GetComponentInChildren<AudioSource>();
        // Find the initial spawn point (tagged as "SpawnPoint")
        lastCheckPoint = GameObject.FindWithTag("SpawnPoint");
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check for collisions with death triggers or damage sources
        if (other.gameObject.CompareTag("DeathBox") || other.gameObject.CompareTag("DamageSource"))
        {
            // Move the player to the last checkpoint's position
            gameObject.transform.position = lastCheckPoint.transform.position;
            // Reset the player's velocity
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            // Play the death sound
            deathSound.Play();
        }
        else if (other.gameObject.CompareTag("Checkpoint"))
        {
            // Update the last checkpoint to the collided checkpoint
            lastCheckPoint = other.gameObject;
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        // Check for collisions with damage sources
        if (other.gameObject.CompareTag("DamageSource"))
        {
            // Move the player to the last checkpoint's position
            gameObject.transform.position = lastCheckPoint.transform.position;
            // Reset the player's velocity
            gameObject.GetComponent<Rigidbody>().velocity = Vector3.zero;
            // Play the death sound
            deathSound.Play();
        }
    }
}