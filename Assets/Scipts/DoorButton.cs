using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    private Animator anim;
    
    private void OnEnable()
    {
        EvenManager.OnResetAllDoorTrigger += Reset;
    }

    private void OnDisable()
    {
        EvenManager.OnResetAllDoorTrigger -= Reset;
    }
    
    private bool wasPressed = false; 
    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && !wasPressed)
        {
            wasPressed = true;
            anim.SetBool("PressButton", true);
            EvenManager.TriggerButton();
        }
    }
    
    private void Reset()
    {
        wasPressed = false;
        anim.SetBool("PressButton", false);
        anim.SetBool("Reset", true);
        anim.SetBool("Reset", false);
    }
}
