using System;
using TMPro;
using UnityEditor.TestTools.CodeCoverage;
using UnityEngine;

public class Timer : MonoBehaviour
{
    // Start is called before the first frame update

    [Header("References")]
    [SerializeField] private TextMeshProUGUI timerText;
    
    [Header("Time")]
    private float currentTime;

    [SerializeField] private bool timerActive;
    [SerializeField] private int lvl;
    void Update()
    {
        Counter();
    }

    private void Counter()
    {
        if (timerActive)
        {
            currentTime = currentTime += Time.deltaTime;
        }
        TimeSpan time = TimeSpan.FromSeconds(currentTime);
        timerText.text = time.Minutes.ToString() + ":" + time.Seconds.ToString() + ":" + time.Milliseconds.ToString();
    }

    public void StartTimer()
    {
        timerActive = true;
    }

    public void StopTimer()
    {
        timerActive = false;
    }

    public void ResetTimer()
    {
        currentTime = 0;
    }

    public float GetTime()
    {
        return currentTime;
    }

    public int GetLevel()
    {
        return lvl;
    }

}
