using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    // Define delegate and event for when a button is pressed
    public delegate void ButtonPressAction();
    public static event ButtonPressAction OnButtonPressed;

    // Define delegate and event for when a spike button is pressed with an ID parameter
    public delegate void SpikeButtonPressAction(int id);
    public static event SpikeButtonPressAction OnSpikeButtonPressed;

    // Define delegate and event for resetting all doors
    public delegate void ResetAllDoorTrigger();
    public static event ResetAllDoorTrigger OnResetAllDoorTrigger;

    // Define delegate and event for when a level ends
    public delegate void LevelEndedAction();
    public static event LevelEndedAction OnLevelEnd;

    // Define delegate and event for switching tools with an index parameter
    public delegate void SwitchToolAction(int index);
    public static event SwitchToolAction OnSwitchTool;

    // Trigger the button pressed event
    public static void TriggerButton()
    {
        if (OnButtonPressed != null)
        {
            OnButtonPressed?.Invoke();
        }
    }

    // Trigger the spike button pressed event with an ID parameter
    public static void TriggerSpikeButton(int id)
    {
        if (OnSpikeButtonPressed != null)
        {
            OnSpikeButtonPressed?.Invoke(id);
        }
    }

    // Trigger the reset all door event
    public static void ResetAllDoor()
    {
        if (OnResetAllDoorTrigger != null)
        {
            OnResetAllDoorTrigger?.Invoke();
        }
    }

    // Trigger the level end event
    public static void LevelEndedTrigger()
    {
        if (OnLevelEnd != null)
        {
            OnLevelEnd?.Invoke();
        }
    }

    // Trigger the switch tool event with an index parameter
    public static void SwitchToolTrigger(int index)
    {
        if (OnSwitchTool != null)
        {
            OnSwitchTool?.Invoke(index);
        }
    }
}
