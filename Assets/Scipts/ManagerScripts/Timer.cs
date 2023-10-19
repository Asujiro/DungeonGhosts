using System;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshProUGUI timerText; // Reference to the timer text UI element
    
    [Header("Time")]
    private float currentTime; // Current time on the timer
    
    [SerializeField] private bool timerActive; // Flag to track if the timer is active
    [SerializeField] private int lvl; // Level identifier (not used in this script)
    [SerializeField] private HighscoreDownloader highscore; // Reference to the HighscoreDownloader script
    
    void Update()
    {
        Counter();
    }

    private void Counter()
    {
        if (timerActive)
        {
            currentTime += Time.deltaTime; // Increment the current time
        }
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        // Format the timer display
        timerText.text = time.Minutes.ToString() + ":" + time.Seconds.ToString() + "." + time.Milliseconds.ToString();
    }

    public void StartTimer()
    {
        timerActive = true; // Activate the timer
    }

    public void StopTimer()
    {
        timerActive = false; // Deactivate the timer
    }

    public void ResetTimer()
    {
        currentTime = 0; // Reset the timer to zero
    }

    public float GetTime()
    {
        return currentTime; // Return the current time value
    }
}