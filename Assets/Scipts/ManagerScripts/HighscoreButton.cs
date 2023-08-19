using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighscoreButton : MonoBehaviour
{
    private TextMeshProUGUI buttonText;  // Reference to the TextMeshProUGUI component in the button
    [SerializeField] private HighscoreDownloader hDownloader;  // Reference to the HighscoreDownloader component

    
    private void Start()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();  // Get the TextMeshProUGUI component from the button
    }

    // Extract the level number from the button's text
    private int ExtractLevelNumber(string input)
    {
        int levelNumber = 0;
        string[] parts = input.Split(' ');  // Split the input string into parts using space as the separator

        if (parts.Length >= 2 && parts[0].ToLower() == "level")
        {
            if (int.TryParse(parts[1], out levelNumber))
            {
                return levelNumber;
            }
        }

        Debug.LogWarning("Invalid input format: " + input);
        return levelNumber;
    }

    // Called when the button is clicked
    public void OnButtonClick()
    {
        hDownloader.SetLevel(ExtractLevelNumber(buttonText.text));  // Set the level in the HighscoreDownloader
        hDownloader.RefreshHighscores();  // Trigger the highscore refresh for the selected level
    }
}