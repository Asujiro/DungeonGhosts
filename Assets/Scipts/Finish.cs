using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;

public class Finish : MonoBehaviour
{

    [SerializeField] private Timer timer;
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            timer.StopTimer();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

        }
    }
}
