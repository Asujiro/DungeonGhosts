using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using Unity.VRTemplate;
using UnityEngine;
using UnityEngine.InputSystem;

public class Throwing : MonoBehaviour
{
   [SerializeField] private Transform playerCam;
   [SerializeField] private GameObject throwingObject;
   [SerializeField] private Transform gunTip;
   [SerializeField] private float throwForce;
   private List<GameObject> spawnedObjects = new List<GameObject>();
   [SerializeField] private InputAction throwKey;
   private bool isSwinging = true;

   private void OnEnable()
   {
      throwKey.Enable();
      throwKey.performed += Throw;
   }

   private void OnDisable()
   {
      throwKey.Disable();
      throwKey.performed -= Throw;
   }


   private void Throw(InputAction.CallbackContext callbackContext)
   {
      if (!isSwinging)
      {
         GameObject spawnedObject = null;
         if (spawnedObjects.Count < 5)
         {
            spawnedObject = Instantiate(throwingObject, gunTip.position, playerCam.rotation);
            spawnedObjects.Add(spawnedObject);
            Invoke(nameof(DestroyObject), 10f);
            Debug.Log(spawnedObjects.Count);
         }
         else
         {
            Destroy(spawnedObjects[0]);
            spawnedObjects.RemoveAt(0);
            spawnedObject = Instantiate(throwingObject, gunTip.position, playerCam.rotation);
            spawnedObjects.Add(spawnedObject);
         }

         if (spawnedObject != null)
         {
            Rigidbody rb = spawnedObject.GetComponent<Rigidbody>();

            Vector3 direction = playerCam.forward;
            RaycastHit hit;

      
      
            if (Physics.Raycast(playerCam.position, playerCam.forward, out hit, 400f))
            {
               direction = (hit.point - gunTip.position).normalized;
            }

            Vector3 addForce = direction * throwForce + transform.up;
      
            rb.AddForce(addForce, ForceMode.Impulse);
         }
      }
   }

   private void DestroyObject()
   {
      if (spawnedObjects == null) return;
      Destroy(spawnedObjects[0]);
      spawnedObjects.RemoveAt(0);
   }

   public void SetIsSwinging(bool status)
   {
      isSwinging = status;
   }
}
