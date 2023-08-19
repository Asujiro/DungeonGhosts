using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowingTarget : MonoBehaviour
{
    private bool wasPressed = false;

    private void OnEnable()
    {
        EvenManager.OnResetAllDoorTrigger += Reset;
    }

    private void OnDisable()
    {
        EvenManager.OnResetAllDoorTrigger -= Reset;
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Stone") && !wasPressed)
        {
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            gameObject.transform.GetChild(1).gameObject.SetActive(true);
            wasPressed = true;
            EvenManager.TriggerButton();
        }
    }
    
    private void Reset()
    {
        wasPressed = false;
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }
    
}

