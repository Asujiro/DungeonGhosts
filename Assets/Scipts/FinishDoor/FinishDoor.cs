using UnityEngine;

public class FinishDoor : MonoBehaviour
{
    private Animator anim;
    private int currentActions = 0;
    
    [SerializeField] private int requiredActions = 2;
    // Subscribe to the events when this script is enabled
    private void OnEnable()
    {
        EventManager.OnButtonPressed += OpenDoor;
        EventManager.OnResetAllDoorTrigger += Reset;
    }

    // Unsubscribe from the events when this script is disabled
    private void OnDisable()
    {
        EventManager.OnButtonPressed -= OpenDoor;
        EventManager.OnResetAllDoorTrigger -= Reset;
    }
    
    private void Start()
    {
        // Get the Animator component attached to this GameObject
        anim = GetComponent<Animator>();
    }

    private void OpenDoor()
    {
        // Increment the count of actions triggered by buttons
        currentActions++;

        // Check if the required number of actions have been reached
        if (currentActions >= requiredActions)
        {
            // Trigger the "ButtonPressed" animation state to open the door
            anim.SetBool("ButtonPressed", true);
        }
    }

    private void Reset()
    {
        // Reset the door's state when the event is invoked
        currentActions = 0;
        anim.SetBool("ButtonPressed", false);
    }
}