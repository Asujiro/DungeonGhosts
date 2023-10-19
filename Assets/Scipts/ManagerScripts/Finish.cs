using UnityEngine;

public class Finish : MonoBehaviour
{
    private GameObject player;  // Reference to the player GameObject
    [SerializeField] private Timer timer;  // A Timer component reference
    [SerializeField] private GameObject endscreen;  // A reference to the end screen GameObject

    // Triggered when an object enters the collider attached to this GameObject
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))  // Check if the object is tagged as "Player"
        {
            timer.StopTimer();  // Stop the timer
            Cursor.visible = true;  // Make the cursor visible
            Cursor.lockState = CursorLockMode.None;  // Release the cursor lock
            player.GetComponentInChildren<PlayerMovement>().enabled = false;  // Disable player movement
            player.GetComponentInChildren<FirstPersonCamera>().enabled = false;  // Disable the first-person camera
            endscreen.SetActive(true);  // Activate the end screen
            EventManager.LevelEndedTrigger();  // Trigger the level ended event
        }
    }

    // Set the player GameObject reference
    public void SetPlayer(GameObject p)
    {
        player = p;
    }
}