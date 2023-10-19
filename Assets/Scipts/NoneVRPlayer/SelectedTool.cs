
using UnityEngine;

public class SelectedTool : MonoBehaviour
{
    [SerializeField] private GameObject iconSwing;
    [SerializeField] private GameObject iconThrow;

    private void OnEnable()
    {
        // Subscribe to the event for switching tools
        EventManager.OnSwitchTool += ChangeIcon;
    }
    
    private void OnDisable()
    {
        // Unsubscribe from the event when disabled
        EventManager.OnSwitchTool -= ChangeIcon;
    }

    private void ChangeIcon(int index)
    {
        // Change the active tool icon based on the provided index
        switch (index)
        {
            case 1:
                iconThrow.SetActive(false);
                iconSwing.SetActive(true);
                break;
            case 2:
                iconThrow.SetActive(true);
                iconSwing.SetActive(false);
                break;
            case 3:
                iconThrow.SetActive(false);
                iconSwing.SetActive(false);
                break;
        }
    }
}