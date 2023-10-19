using UnityEngine;

public class ThrowingTarget : MonoBehaviour
{
    private bool wasPressed = false;

    // Subscribe to the event when this script is enabled
    private void OnEnable()
    {
        EventManager.OnResetAllDoorTrigger += Reset;
    }

    // Unsubscribe from the event when this script is disabled
    private void OnDisable()
    {
        EventManager.OnResetAllDoorTrigger -= Reset;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the collider's GameObject has the "Stone" tag and the target hasn't been pressed
        if (other.gameObject.CompareTag("Stone") && !wasPressed)
        {
            // Deactivate the first child object (assuming there are two child objects)
            gameObject.transform.GetChild(0).gameObject.SetActive(false);
            // Activate the second child object
            gameObject.transform.GetChild(1).gameObject.SetActive(true);

            // Mark the target as pressed
            wasPressed = true;

            // Invoke the event to indicate that a button has been triggered
            EventManager.TriggerButton();
        }
    }

    private void Reset()
    {
        // Reset the target's state when the event is invoked
        wasPressed = false;
        // Activate the first child object
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        // Deactivate the second child object
        gameObject.transform.GetChild(1).gameObject.SetActive(false);
    }
}