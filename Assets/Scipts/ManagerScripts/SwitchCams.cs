using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class SwitchCams : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject buildCam;
    // Start is called before the first frame update
    [SerializeField] private Timer timer;
    [SerializeField] private GameObject dragManager;
    [FormerlySerializedAs("input")] [SerializeField] private InputAction switchCam;
    [SerializeField] private InputAction escMenuButton;
    [SerializeField] private Finish finishLine;
    [SerializeField] private GameObject escMenu;
    private bool menuIsOpen;
    
    private GameObject spawnedPlayer;
    
    
    private void OnEnable()
    {
        escMenuButton.performed += OpenCloseMenu;
        switchCam.performed += SwitchToPlayMode;
        switchCam.Enable();
        escMenuButton.Enable();
    }

    

    private void OnDisable()
    {
        switchCam.performed -= SwitchToPlayMode;
        switchCam.Disable();
        escMenuButton.Disable();
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
            EvenManager.ResetAllDoor();
        }
    }
    
    private void OpenCloseMenu(InputAction.CallbackContext obj)
    {
        if (!escMenu.activeSelf)
        {
            
            escMenu.SetActive(true);
            if (!buildCam.activeSelf)
            {
                spawnedPlayer.GetComponentInChildren<PlayerMovement>().enabled = false;
                spawnedPlayer.GetComponentInChildren<FirstPersonCamera>().enabled = false;
                timer.StopTimer();
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            
        }
        else
        {
            escMenu.SetActive(false);
            if (!buildCam.activeSelf)
            {
                spawnedPlayer.GetComponentInChildren<FirstPersonCamera>().enabled = true;
                spawnedPlayer.GetComponentInChildren<PlayerMovement>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                timer.StartTimer();
            }
            
        }
    }


}
