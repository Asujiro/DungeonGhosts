using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvenManager : MonoBehaviour
{
    public delegate void ButtonPressAction();
    public static event ButtonPressAction OnButtonPressed;
    
    public delegate void SpikeButtonPressAction(int id);
    public static event SpikeButtonPressAction OnSpikeButtonPressed;
    
    public static void TriggerButton()
    {
        if (OnButtonPressed != null)
        {
            OnButtonPressed?.Invoke();
        }
    }

    public static void TriggerSpikeButton(int id)
    {
        if (OnSpikeButtonPressed != null)
        {
            OnSpikeButtonPressed?.Invoke(id);
        }
    }
    
    
}
