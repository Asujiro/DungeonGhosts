using System;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;
using UnityEngine.Networking;

public class HighscoreDownloader : MonoBehaviour
{
    public TextMeshProUGUI highscoreText; // Reference to the TextMeshProUGUI component to display highscores
    private string serverURL = "https://asujiro.com/highscores.json "; // URL to download highscores from
    [SerializeField] private int lvl; // Current level for which to download and display highscores

    // Serializable class for highscore entries
    [System.Serializable]
    public class HighscoreEntry
    {
        public string playerName;
        public float score;
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Start the coroutine to download and display highscores
        StartCoroutine(DownloadHighscores());
    }

    // Coroutine to download highscores from the server
    private IEnumerator DownloadHighscores()
    {
        yield return new WaitForSeconds(2f); // Wait for 2 seconds before downloading

        using (UnityWebRequest www = UnityWebRequest.Get(serverURL))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error downloading highscores: " + www.error);
            }
            else
            {
                string json = www.downloadHandler.text;
                ProcessAndDisplayHighscores(json); // Process and display downloaded highscores
            }
        }
    }

    // Process and display highscores using the provided JSON data
    private void ProcessAndDisplayHighscores(string json)
    {
        string displayText = "";

        // Parse the JSON data
        var jsonObject = JSON.Parse(json);
        if (jsonObject != null && jsonObject["highscores"] != null)
        {
            string levelName = "level" + lvl;
            JSONNode levelHighscores = jsonObject["highscores"][levelName];

            if (levelHighscores != null)
            {
                displayText += $"Top Highscores for {levelName}:\n";

                List<HighscoreEntry> sortedHighscores = new List<HighscoreEntry>();

                // Iterate through the highscore entries in the JSON data
                foreach (JSONNode entryNode in levelHighscores)
                {
                    HighscoreEntry entry = new HighscoreEntry();
                    entry.playerName = entryNode["playerName"];

                    string scoreString = entryNode["score"];
                    float score;
                    if (float.TryParse(scoreString, out score))
                    {
                        entry.score = score;
                    }
                    else
                    {
                        Debug.LogError($"Invalid score format for player: {entry.playerName}");
                    }

                    sortedHighscores.Add(entry);
                }

                sortedHighscores = sortedHighscores.OrderBy(entry => entry.score).ToList();

                int maxDisplayCount = Mathf.Min(10, sortedHighscores.Count);
                for (int i = 0; i < maxDisplayCount; i++)
                {
                    HighscoreEntry entry = sortedHighscores[i];
                    string formattedTime = FormatTime(entry.score);
                    displayText += $"{i + 1}. {entry.playerName}: {formattedTime}\n";
                }
            }
            else
            {
                displayText = $"No highscores available for {levelName}";
            }
        }
        else
        {
            Debug.LogWarning("Highscore data is null or invalid.");
        }

        highscoreText.text = displayText; // Display the formatted highscore text
    }

    // Format time in seconds into minutes:seconds.milliseconds format
    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        int milliseconds = Mathf.FloorToInt((timeInSeconds - Mathf.Floor(timeInSeconds)) * 1000);

        string formattedTime = string.Format("{0:D2}:{1:D2}.{2:D3}", minutes, seconds, milliseconds);
        return formattedTime;
    }

    // Method to refresh highscores manually
    public void RefreshHighscores()
    {
        StartCoroutine(DownloadHighscores());
    }

    // Method to set the level for highscore download
    public void SetLevel(int level)
    {
        lvl = level;
    }

    // Method to get the current level
    public int GetLvl()
    {
        return lvl;
    }
}
