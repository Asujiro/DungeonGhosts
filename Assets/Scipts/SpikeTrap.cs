using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
    
    private Animator anim;
    private int currentActions = 0;
    [SerializeField] private int spikeID;
    

    private void OnEnable()
    {
        EvenManager.OnSpikeButtonPressed += ActivateTrap;
    }

    private void OnDisable()
    {
        EvenManager.OnSpikeButtonPressed -= ActivateTrap;
    }

   
    private void Start()
    {
        anim = GetComponent<Animator>();
    }
    
    private void ActivateTrap(int id)
    {
        currentActions++;

        if (spikeID == id)
        {
            anim.SetBool("SpikeOut", true);
            Invoke(nameof(ResetTrap), 5f);
        }
    }
    
    private void ResetTrap()
    {
        anim.SetBool("SpikeOut", false);
    }
}
