using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class SwitchCams : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab; // Prefab of the player character
    [SerializeField] private GameObject spawnPoint; // Spawn point for the player character
    [SerializeField] private GameObject buildCam; // Camera used for building mode
    [SerializeField] private Timer timer; // Timer component to track play time
    [SerializeField] private GameObject dragManager; // Object to manage drag interactions
    [FormerlySerializedAs("input")] [SerializeField] private InputAction switchCam; // Input action to switch cameras
    [SerializeField] private InputAction escMenuButton; // Input action to open/close the menu
    [SerializeField] private Finish finishLine; // Reference to the Finish script for level completion
    [SerializeField] private GameObject escMenu; // UI element for the in-game menu
    private bool menuIsOpen; // Flag to track if the menu is open
    private bool levelOver = false; // Flag to track if the level has ended
    private GameObject spawnedPlayer; // Reference to the spawned player character
    
    
    private void OnEnable()
    {
        escMenuButton.performed += OpenCloseMenu; // Subscribe to the menu input action
        switchCam.performed += SwitchToPlayMode; // Subscribe to the camera switch input action
        switchCam.Enable();
        escMenuButton.Enable();
        EventManager.OnLevelEnd += DeactivateEscScreen; // Subscribe to the level end event
    }

    

    private void OnDisable()
    {
        EventManager.OnLevelEnd -= DeactivateEscScreen; // Unsubscribe from the level end event
        escMenuButton.performed -= OpenCloseMenu;
        switchCam.performed -= SwitchToPlayMode;
        switchCam.Disable();
        escMenuButton.Disable();
    }

    private void SwitchToPlayMode(InputAction.CallbackContext callbackContext)
    {
        if (buildCam.activeSelf)
        {
            timer.StartTimer();
            buildCam.SetActive(false); // Disable build mode camera
            spawnedPlayer = Instantiate(playerPrefab, spawnPoint.transform.position, Quaternion.identity); // Spawn player character
            dragManager.SetActive(false); // Deactivate drag interactions
            finishLine.SetPlayer(spawnedPlayer); // Set the player character for the finish line
            timer.StartTimer();
            EventManager.SwitchToolTrigger(1); // Trigger switch of player's tool
        }
        else
        {
            Destroy(spawnedPlayer); // Destroy the player character
            buildCam.SetActive(true); // Enable build mode camera
            dragManager.SetActive(true); // Activate drag interactions
            timer.StopTimer(); // Stop the timer
            timer.ResetTimer(); // Reset the timer
            EventManager.ResetAllDoor(); // Reset all doors
            EventManager.SwitchToolTrigger(3); // Trigger switch of player's tool
        }
    }

    private void DeactivateEscScreen()
    {
        levelOver = true; // Set the level over flag
    }
    
    private void OpenCloseMenu(InputAction.CallbackContext obj)
    {
        if (!escMenu.activeSelf && !levelOver) // Check if the menu is not open and the level is not over
        {
            escMenu.SetActive(true); // Open the menu
            if (!buildCam.activeSelf)
            {
                spawnedPlayer.GetComponentInChildren<PlayerMovement>().enabled = false;
                spawnedPlayer.GetComponentInChildren<FirstPersonCamera>().enabled = false;
                if (!levelOver)
                {
                    timer.StopTimer();
                }
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            
        }
        else if(escMenu.activeSelf)
        {
            escMenu.SetActive(false); // Close the menu
            if (!buildCam.activeSelf)
            {
                spawnedPlayer.GetComponentInChildren<FirstPersonCamera>().enabled = true;
                spawnedPlayer.GetComponentInChildren<PlayerMovement>().enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                if (!levelOver)
                {
                    timer.StartTimer();
                }
                
            }
            
        }
    }
}
