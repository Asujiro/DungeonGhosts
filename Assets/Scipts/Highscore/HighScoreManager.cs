using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class HighscoreManager : MonoBehaviour {
    public List<HighscoreEntry> highscores = new List<HighscoreEntry>();
    private string highscoreFilePath;

    private void Awake() {
        highscoreFilePath = Path.Combine(Application.persistentDataPath, "highscores.json");
        LoadHighscores();
    }

    public void SaveHighscores() {
        string json = JsonUtility.ToJson(highscores);
        File.WriteAllText(highscoreFilePath, json);
    }

    public void LoadHighscores() {
        if (File.Exists(highscoreFilePath)) {
            string json = File.ReadAllText(highscoreFilePath);
            highscores = JsonUtility.FromJson<List<HighscoreEntry>>(json);
        }
    }

    public void AddHighscore(string playerName, float score, int level) {
        HighscoreEntry entry = new HighscoreEntry {
            playerName = playerName,
            score = score,
            level = level
        };
        highscores.Add(entry);
        highscores.Sort((a, b) => b.score.CompareTo(a.score));
        SaveHighscores();
    }
    
    [System.Serializable]
    public class HighscoreEntry {
        public string playerName;
        public float score;
        public int level;
    }
}

