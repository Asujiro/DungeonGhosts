using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HighscoreButton : MonoBehaviour
{
    private TextMeshProUGUI buttonText;
    [SerializeField] private HighscoreDownloader hDownloader;
    
    private void Start()
    {
        buttonText = GetComponentInChildren<TextMeshProUGUI>();
    }

    private int ExtractLevelNumber(string input)
    {
        int levelNumber = 0;
        string[] parts = input.Split(' ');

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
    
    public void OnButtonClick()
    {
        hDownloader.SetLevel(ExtractLevelNumber(buttonText.text));
        hDownloader.RefreshHighscores();
    }
}

