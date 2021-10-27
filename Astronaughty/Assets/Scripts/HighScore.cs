using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HighScore : MonoBehaviour
{
    const string privateCode = "Ci9za-WyGEWN4aknKjcHjwNs21k7qoEE6gt9WtXcR_uQ";
    const string publicCode = "5fecb71e0af269152884e19b";
    const string webURL = "https://www.dreamlo.com/lb/Ci9za-WyGEWN4aknKjcHjwNs21k7qoEE6gt9WtXcR_uQ";

    public Highscore[] highscoresList;

    public Text highScore_1;
    public Text highScore_2;
    public Text highScore_3;
    public Text highScore_4;
    public Text highScore_5;
    public Text personalHighScore;


    void Awake()
    {
        // AddNewHighScore("Santi", 500);
        // AddNewHighScore("Tommaso", 600);
        // AddNewHighScore("David", 1000);

        //DownloadHighScores();
    }

    public void AddNewHighScore(string username, int score)
    {

        StartCoroutine(UploadNewHighScore(username, score));
    }

    //Uploades scroes to the DB
    IEnumerator UploadNewHighScore(string username, int score)
    {
        // Debug.Log("Starting Upload");
        UnityWebRequest www = UnityWebRequest.Get(webURL + "/add/" + username + "/" + score); //Makes request to the DB
        yield return www.SendWebRequest();

        if (string.IsNullOrEmpty(www.error))
        {
            print("Upload Successful");
        }
        else
        {
            print("Error uploading: " + www.error);
        }
    }

    public void DownloadHighScores()
    {
        StartCoroutine("DownloadHighScoreFromDB");
    }

    //Gets scores from the DB
    IEnumerator DownloadHighScoreFromDB()
    {
        //Debug.Log("HighScore 63");
        UnityWebRequest www = UnityWebRequest.Get("http://dreamlo.com/lb/5fecb71e0af269152884e19b/pipe");
        yield return www.SendWebRequest();
        //Debug.Log("HighScore 66");
        if (string.IsNullOrEmpty(www.error))
        {
            //Debug.Log("HighScore 69");
             FormatHighScores(www.downloadHandler.text); //Calls the method to fromat the score
            //Debug.Log("HighScore 71");
        }
        else
        {
            //Debug.Log("HighScore 75");
            print("Error downloading: " + www.error);
        }
    }

    void FormatHighScores(string textStream)
    {
        Debug.Log("HighScore 82");
        string[] entries = textStream.Split(new char[] { '\n' }, System.StringSplitOptions.RemoveEmptyEntries);
        //Debug.Log("HighScore 84");
        highscoresList = new Highscore[entries.Length];
        //Debug.Log("HighScore 86");
        for (int i = 0; i < entries.Length; i++)
        {
            string[] entryInfo = entries[i].Split(new char[] { '|' });
            string username = entryInfo[0];
            int score = int.Parse(entryInfo[1]);
            highscoresList[i] = new Highscore(username, score);
            int playerPosition = -1;
            // print(highscoresList[i].username + ": " + highscoresList[i].score);

            //Gets the highscore for the player
            // Debug.Log(PlayerPrefs.GetString("UserName"));

            if (highscoresList[i].username.Equals(PlayerPrefs.GetString("UserName")))
            { //Gets the highscor for the player
            Debug.Log("Player is " + PlayerPrefs.GetString("UserName"));
                if (i < 5)
                {
                    Debug.Log("Player is in top 5");
                    playerPosition = i;
                }
                else
                {
                    Debug.Log("Player is outside of top 5");
                    personalHighScore.gameObject.SetActive(true);
                    personalHighScore.text = "" + (i + 1) + ". " + highscoresList[i].username + ": " + highscoresList[i].score;
                }

            }
            switch (i)
            {
                case 0:
                    
                    highScore_1.text = "1. " + highscoresList[i].username + ": " + highscoresList[i].score;
                    if (playerPosition == i)
                    {
                        personalHighScore.gameObject.SetActive(false);
                        highScore_1.color = new Color(255f, 194f, 0f, 255f);
                    }
                    break;
                case 1:
                    highScore_2.text = "2. " + highscoresList[i].username + ": " + highscoresList[i].score;
                    if (playerPosition == i)
                    {
                        personalHighScore.gameObject.SetActive(false);
                        highScore_2.color = new Color(255f, 194f, 0f, 255f);
                    }
                    break;
                case 2:
                    highScore_3.text = "3. " + highscoresList[i].username + ": " + highscoresList[i].score;
                    if (playerPosition == i)
                    {
                        personalHighScore.gameObject.SetActive(false);
                        highScore_3.color = new Color(255f, 194f, 0f, 255f);
                    }
                    break;
                case 3:
                    highScore_4.text = "4. " + highscoresList[i].username + ": " + highscoresList[i].score;
                    if (playerPosition == i)
                    {
                        personalHighScore.gameObject.SetActive(false);
                        highScore_4.color = new Color(255f, 194f, 0f, 255f);
                    }
                    break;
                case 4:
                    highScore_5.text = "5. " + highscoresList[i].username + ": " + highscoresList[i].score;
                    if (playerPosition == i)
                    {
                        personalHighScore.gameObject.SetActive(false);
                        highScore_5.color = new Color(255f, 194f, 0f, 255f);
                    }
                    break;
                default:
                    break;
            }
        }
        Debug.Log("HighScore 178");

    }

}

public struct Highscore //Creates structure for highscore to only display name and score
{
    public string username;
    public int score;

    public Highscore(string _username, int _score)
    {
        username = _username;
        score = _score;
    }
}
