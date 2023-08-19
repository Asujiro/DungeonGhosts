using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VRTemplate;
using UnityEngine;
using UnityEngine.InputSystem;

public class Throwing : MonoBehaviour
{
   // Reference to the player's camera
   [SerializeField] private Transform playerCam;

   // Prefab of the object to be thrown
   [SerializeField] private GameObject throwingObject;

   // Reference to the tip of the gun where objects are spawned
   [SerializeField] private Transform gunTip;

   // Force applied when throwing an object
   [SerializeField] private float throwForce;

   // List to keep track of spawned objects
   private List<GameObject> spawnedObjects = new List<GameObject>();

   // Input action for throwing
   [SerializeField] private InputAction throwKey;

   // Flag to indicate if the player is currently swinging
   private bool isSwinging = true;

   // Called when the script is enabled
   private void OnEnable()
   {
      // Enable the throwKey input action
      throwKey.Enable();

      // Attach the Throw method to the performed event of throwKey
      throwKey.performed += Throw;
   }

   // Called when the script is disabled
   private void OnDisable()
   {
      // Disable the throwKey input action
      throwKey.Disable();

      // Detach the Throw method from the performed event of throwKey
      throwKey.performed -= Throw;
   }

   // Method to handle the throw action
   private void Throw(InputAction.CallbackContext callbackContext)
   {
      // Check if the player is currently swinging
      if (!isSwinging)
      {
         GameObject spawnedObject = null;

         // Check if the number of spawned objects is less than 5
         if (spawnedObjects.Count < 5)
         {
            // Instantiate the throwingObject at the gunTip position with the playerCam rotation
            spawnedObject = Instantiate(throwingObject, gunTip.position, playerCam.rotation);

            // Add the spawned object to the list
            spawnedObjects.Add(spawnedObject);

            // Invoke the DestroyObject method after 10 seconds
            Invoke(nameof(DestroyObject), 10f);

            // Log the current count of spawned objects
            Debug.Log(spawnedObjects.Count);
         }
         else
         {
            // Destroy the oldest spawned object
            Destroy(spawnedObjects[0]);
            spawnedObjects.RemoveAt(0);

            // Instantiate the throwingObject at the gunTip position with the playerCam rotation
            spawnedObject = Instantiate(throwingObject, gunTip.position, playerCam.rotation);

            // Add the spawned object to the list
            spawnedObjects.Add(spawnedObject);
         }

         // Check if a spawned object was successfully instantiated
         if (spawnedObject != null)
         {
            // Get the Rigidbody component of the spawned object
            Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();

            // Calculate the direction to throw the object
            Vector3 direction = playerCam.forward;
            RaycastHit hit;

            // Check for collisions using a raycast from the player's camera
            if (Physics.Raycast(playerCam.position, playerCam.forward, out hit, 400f))
            {
               // Adjust the direction towards the point of collision
               direction = (hit.point - gunTip.position).normalized;
            }

            // Calculate the total force to be applied to the object
            Vector3 addForce = direction * throwForce + transform.up;

            // Apply the calculated force to the object
            rb.AddForce(addForce, ForceMode.Impulse);
         }
      }
   }

   // Method to destroy the oldest spawned object
   private void DestroyObject()
   {
      // Check if there are spawned objects in the list
      if (spawnedObjects == null) return;

      // Destroy the oldest spawned object and remove it from the list
      Destroy(spawnedObjects[0]);
      spawnedObjects.RemoveAt(0);
   }

   // Method to set the swinging status
   public void SetIsSwinging(bool status)
   {
      isSwinging = status;
   }
}
