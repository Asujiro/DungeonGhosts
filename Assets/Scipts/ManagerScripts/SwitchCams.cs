using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwitchCams : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject buildCam;
    // Start is called before the first frame update
    [SerializeField] private Timer timer;
    [SerializeField] private GameObject dragManager;
    [SerializeField] private InputAction input;
    [SerializeField] private Finish finishLine;
    private GameObject spawnedPlayer;
    
    private void OnEnable()
    {
        input.performed += SwitchToPlayMode;
        input.Enable();
    }

    private void OnDisable()
    {
        input.performed -= SwitchToPlayMode;
        input.Disable();
    }

    private void SwitchToPlayMode(InputAction.CallbackContext callbackContext)
    {
        if (buildCam.activeSelf)
        {
            timer.StartTimer();
            buildCam.SetActive(false);
            spawnedPlayer = Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity);
            dragManager.SetActive(false);
            finishLine.SetPlayer(spawnedPlayer);
            timer.StartTimer();
            
        }
        else
        {
            Destroy(spawnedPlayer);
            buildCam.SetActive(true);
            dragManager.SetActive(true);
            timer.StopTimer();
            timer.ResetTimer();
        }
    }


}
