using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class HighscoreManager : MonoBehaviour {
    private List<HighscoreEntry> highscores = new List<HighscoreEntry>();
    private string highscoreFilePath;

    private void Awake() {
        // Combine the application's persistent data path with the filename "highscores.json"
        highscoreFilePath = Path.Combine(Application.persistentDataPath, "highscores.json");

        // Load existing highscores from the file on awake
        LoadHighscores();
    }

    // Save the current highscores list to the JSON file
    public void SaveHighscores() {
        // Convert the list to JSON format using Unity's JsonUtility
        string json = JsonUtility.ToJson(highscores);

        // Write the JSON data to the file
        File.WriteAllText(highscoreFilePath, json);
    }

    // Load highscores from the JSON file
    public void LoadHighscores() {
        if (File.Exists(highscoreFilePath)) {
            // Read the JSON data from the file
            string json = File.ReadAllText(highscoreFilePath);

            // Convert the JSON data back to the list using Unity's JsonUtility
            highscores = JsonUtility.FromJson<List<HighscoreEntry>>(json);
        }
    }

    // Add a new highscore entry and sort the list by score in descending order
    public void AddHighscore(string playerName, float score, int level) {
        HighscoreEntry entry = new HighscoreEntry {
            playerName = playerName,
            score = score,
            level = level
        };
        highscores.Add(entry);
        highscores.Sort((a, b) => b.score.CompareTo(a.score)); // Sort in descending order
        SaveHighscores(); // Save the updated highscores list
    }
    
    // Serializable class for highscore entries
    [System.Serializable]
    public class HighscoreEntry {
        public string playerName;
        public float score;
        public int level;
    }
}