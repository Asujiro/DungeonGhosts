using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishDoor : MonoBehaviour
{
    private Animator anim;
    private int currentActions = 0;
    
    [SerializeField] private int requiredActions = 2;
    

    private void OnEnable()
    {
        EvenManager.OnButtonPressed += OpenDoor;
        EvenManager.OnResetAllDoorTrigger += Reset;
    }

    private void OnDisable()
    {
        EvenManager.OnResetAllDoorTrigger -= Reset;
        EvenManager.OnButtonPressed -= OpenDoor;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    
    private void OpenDoor()
    {
        if (anim.GetBool("Reset"))
        {
            anim.SetBool("Reset", false);
        }
        currentActions++;
        if (currentActions >= requiredActions)
        {
            anim.SetBool("ButtonPressed", true);
        }
    }
    
    private void Reset()
    {
        currentActions = 0;
        anim.SetBool("ButtonPressed", false);
        anim.SetBool("Reset", true);
        anim.SetBool("Reset", false);
    }
    
    
}
