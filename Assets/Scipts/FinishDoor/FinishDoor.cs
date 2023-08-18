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
    }

    private void OnDisable()
    {
        EvenManager.OnButtonPressed -= OpenDoor;
    }

    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    
    private void OpenDoor()
    {
        currentActions++;

        if (currentActions >= requiredActions)
        {
            anim.SetBool("ButtonPressed", true);
        }
    }
    
    
}
