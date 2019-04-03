using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameController : MonoBehaviour
{
    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;
    public GameObject pausedPanel;
    public GameObject gameOverPanel;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI topScoreText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI liveText;

    DebrisSpawner ds;

    [SerializeField]
    private int maxPlayerLives;

    private bool gameStarted;
    private bool gameOver;
    private bool paused;

    private int playerLives;
    private int playerScore;

    public bool GameStarted
    {
        get { return gameStarted; }
    }

    public bool GetGameOver
    {
        get { return gameOver; }
    }

    private void Start()
    {
        ds = FindObjectOfType<DebrisSpawner>();
        if (!PlayerPrefs.HasKey("TopScore"))
        {
            PlayerPrefs.SetInt("TopScore", 0);
        }
        StartCoroutine(GameStartCountDown());
        playerLives = maxPlayerLives;
    }

    private void Update()
    {
        if (!gameStarted)
        {
            return;
        }

        MainMenu();
        liveText.text = "Lives: " + playerLives.ToString();
        scoreText.text = playerScore.ToString("0000");
    }

    IEnumerator GameStartCountDown()
    {
        int sec = 4;
        countdownPanel.SetActive(true);

        while(sec > 0)
        {
            countdownText.text = "3";
            yield return new WaitForSeconds(1);
            sec--;

            countdownText.text = "2";
            yield return new WaitForSeconds(1);
            sec--;

            countdownText.text = "1";
            yield return new WaitForSeconds(1);
            sec--;

            countdownText.text = "GO!";
            yield return new WaitForSeconds(1);
            sec--;
        }

        countdownPanel.SetActive(false);
        gameStarted = true;
        ds.StartDebrisSpawner();
    }

    void MainMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            paused = !paused;
        }

        if (paused)
        {
            Time.timeScale = 0;
            pausedPanel.SetActive(true);
        }
        else
        {
            Time.timeScale = 1;
            pausedPanel.SetActive(false);
        }
    }

    public void UpdateScore(int i)
    {
        playerScore += i;
    }

    public void UpdateLives(int i)
    {
        Debug.Log("Hit by debris");
        playerLives -= i;
        if(playerLives <= 0)
        {
            GameOver();
        }
    }

    void GameOver()
    {
        gameOver = true;
        if (playerScore > PlayerPrefs.GetInt("TopScore"))
        {
            PlayerPrefs.SetInt("TopScore", playerScore);
        }

        finalScoreText.text = "Your Score: " + playerScore.ToString();
        topScoreText.text = "Your Top Score: " + PlayerPrefs.GetInt("TopScore").ToString();

        gameOverPanel.SetActive(true);
    }

    public void PlayAgain()
    {
        gameOverPanel.SetActive(false);
        liveText.text = "Lives: 3";
        scoreText.text = "0000";
        playerLives = maxPlayerLives;
        playerScore = 0;
        gameOver = false;
        gameStarted = false;
        StartCoroutine(GameStartCountDown());
    }
}