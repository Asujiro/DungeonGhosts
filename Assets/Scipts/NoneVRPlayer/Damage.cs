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
        deathSound = GetComponentInChildren<AudioSource>();
        lastCheckPoint = GameObject.FindWithTag("SpawnPoint");
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("DeathBox") || other.gameObject.CompareTag("DamageSource"))
        {
            gameObject.transform.position = lastCheckPoint.transform.position;
            gameObject.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
            deathSound.Play();
            
        }
        else if (other.gameObject.CompareTag("Checkpoint"))
        {
            lastCheckPoint = other.gameObject;
        }
    }
}
