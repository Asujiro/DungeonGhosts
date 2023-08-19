using UnityEngine;

public class Finish : MonoBehaviour
{
    private GameObject player;
    [SerializeField] private Timer timer;
    [SerializeField] private GameObject endscreen;
   
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            timer.StopTimer();
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            player.GetComponentInChildren<PlayerMovement>().enabled = false;
            player.GetComponentInChildren<FirstPersonCamera>().enabled = false;
            endscreen.SetActive(true);
            
        }
    }

    public void SetPlayer(GameObject p)
    {
        player = p;
    }
}
