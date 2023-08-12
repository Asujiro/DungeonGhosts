using System;
using UnityEngine;
using TMPro;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SimpleJSON;

public class HighscoreDownloader : MonoBehaviour
{
    public TextMeshProUGUI highscoreText;
    private string serverURL = "https://asujiro.com/highscores.json ";
    [SerializeField] private Timer lvl;

    [System.Serializable]
    public class HighscoreEntry
    {
        public string playerName;
        public float score;
    }

    private void Start()
    {
        StartCoroutine(DownloadHighscores());
    }

    private IEnumerator DownloadHighscores()
    {
        using (WWW www = new WWW(serverURL))
        {
            yield return www;

            if (!string.IsNullOrEmpty(www.error))
            {
                Debug.LogError("Error downloading highscores: " + www.error);
            }
            else
            {
                string json = www.text;
                ProcessAndDisplayHighscores(json);
            }
        }
    }

    private void ProcessAndDisplayHighscores(string json)
    {
        string displayText = "";

        var jsonObject = JSON.Parse(json);
        if (jsonObject != null && jsonObject["highscores"] != null)
        {
            string levelName = "level" + lvl.GetLevel();
            JSONNode levelHighscores = jsonObject["highscores"][levelName];

            if (levelHighscores != null)
            {
                displayText += $"Top Highscores for {levelName}:\n";

                List<HighscoreEntry> sortedHighscores = new List<HighscoreEntry>();

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

        highscoreText.text = displayText;
    }

    private string FormatTime(float timeInSeconds)
    {
        int minutes = Mathf.FloorToInt(timeInSeconds / 60);
        int seconds = Mathf.FloorToInt(timeInSeconds % 60);
        int milliseconds = Mathf.FloorToInt((timeInSeconds - Mathf.Floor(timeInSeconds)) * 1000);

        string formattedTime = string.Format("{0:D2}:{1:D2}.{2:D3}", minutes, seconds, milliseconds);
        return formattedTime;
    }
    
    public void RefreshHighscores()
    {
        StartCoroutine(DownloadHighscores());
    }
    
}