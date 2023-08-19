using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class HighscoreSender : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Timer timer;
    private HighscoreDownloader highscoreDownloader;
    private const string url = "https://asujiro.com/upload_highscore.php"; // Server URL for posting highscores

    // Send the highscore to the server
    public void SendHighscoreToServer()
    {
        highscoreDownloader = GetComponent<HighscoreDownloader>();
        StartCoroutine(PostHighscore(playerName.text, timer.GetTime(), highscoreDownloader.GetLvl()));
    }

    // Coroutine to post highscore to the server
    private IEnumerator PostHighscore(string playerName, float score, int level)
    {
        WWWForm form = new WWWForm(); // Create a new form to send data to the server
        form.AddField("playerName", playerName); // Add player name to the form
        form.AddField("score", score.ToString()); // Add player score to the form
        form.AddField("level", "level" + level); // Add level information to the form

        using (UnityWebRequest www = UnityWebRequest.Post(url, form)) // Create a new POST request with the form data
        {
            yield return www.SendWebRequest(); // Send the request

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Error posting highscore: " + www.error);
            }
            else
            {
                Debug.Log("Highscore posted successfully.");
            }
        }
    }
}