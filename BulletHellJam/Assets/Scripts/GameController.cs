using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController instance = null;

    private float currentGameTime;
    private int enemiesDestroyed;
    private bool isPlaying;

    public enum TextLanguage
    {
        Spanish,
        English
    };
    public TextLanguage textLanguage;
    public string userName;

    private void Awake() {
        if(instance==null)
            instance=this;
        else
            Destroy(this.gameObject);

        DontDestroyOnLoad(this);
    }

    private void Start() {
        textLanguage = TextLanguage.Spanish;
        isPlaying = false;
        enemiesDestroyed = 0;
        currentGameTime = 0f;
    }

    private void Update() 
    {
        if(!isPlaying)
            return;

        if(UIController.instance==null)
            return;
            
        currentGameTime += Time.deltaTime;
        UIController.instance.UpdateGameTime(currentGameTime);
    }

    public void AddEnemyDestroyed()
    {
        enemiesDestroyed++;
        UIController.instance.UpdateEnemiesDestroyed(enemiesDestroyed);
    }

    public void ResetGame()
    {
        isPlaying = true;
        enemiesDestroyed = 0;
        currentGameTime = 0f;
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
    }

    public void ExitGame()
    {
        //SALIR
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("MainMenu", LoadSceneMode.Single);
    }

    public void SetGameOver()
    {
        isPlaying = false;
        int score = GetFinalScore();
        int highScore = PlayerPrefs.GetInt("HighScore", 0);
        UploadScore(score);
        if(score > highScore)
        {
            PlayerPrefs.SetInt("HighScore", score);
        }
        SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
    }

    public float GetCurrentGameTime()
    {
        return currentGameTime;
    }

    public int GetEnemiesDestroyed()
    {
        return enemiesDestroyed;
    }

    public int GetFinalScore()
    {
        return (int)currentGameTime * enemiesDestroyed;
    }

    private void UploadScore(int score)
    {
        StartCoroutine(DatabaseUpload(GameController.instance.userName, score));
    }

    IEnumerator DatabaseUpload(string userame, int score) //Called when sending new score to Website
    {
        const string privateCode = "4VmMvqwQWE-rC-3LSlcY7wxO8UakI5dEKdprR_KvfUHg";  //Key to Upload New Info
        const string webURL = "http://dreamlo.com/lb/"; //  Website the keys are for

        WWW www = new WWW(webURL + privateCode + "/add/" + WWW.EscapeURL(userame) + "/" + score);
        yield return www;

        if (string.IsNullOrEmpty(www.error))
        {
            print("Upload Successful");
        }
        else print("Error uploading" + www.error);
    }

}
