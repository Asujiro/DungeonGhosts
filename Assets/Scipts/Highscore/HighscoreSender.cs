using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;
public class HighscoreSender : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerName;
    [SerializeField] private Timer timer;
    private HighscoreDownloader highscoreDownloader;
    public void SendHighscoreToServer()
    {
        highscoreDownloader = GetComponent<HighscoreDownloader>();
        StartCoroutine(PostHighscore(playerName.text, timer.GetTime(), highscoreDownloader.GetLvl()));
    }

    private IEnumerator PostHighscore(string playerName, float score, int level)
    {
        string url = "https://asujiro.com/upload_highscore.php"; // Ersetzen Sie durch Ihre Server-URL
        WWWForm form = new WWWForm();
        form.AddField("playerName", playerName);
        form.AddField("score", score.ToString());
        form.AddField("level", "level" + level); // Ändern Sie hier, um dem Level den richtigen Namen zuzuweisen

        using (UnityWebRequest www = UnityWebRequest.Post(url, form))
        {
            yield return www.SendWebRequest();

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