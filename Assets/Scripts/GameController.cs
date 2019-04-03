using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject countdownPanel;
    public TextMeshProUGUI countdownText;
    public GameObject pausedPanel;
    public GameObject gameOverPanel;
    public GameObject tint;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI topScoreText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI liveText;
    public TextMeshProUGUI gameOverText;

    DebrisSpawner ds;
    CameraShake camShake;
    Ozone ozone;

    [SerializeField]
    private int maxPlayerLives;
    [SerializeField]
    private float cameraShakeDuration;
    [SerializeField]
    private float cameraShakeAmount;

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
        ozone = FindObjectOfType<Ozone>();
        camShake = FindObjectOfType<CameraShake>();
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
            countdownText.alignment = TextAlignmentOptions.Top;
            countdownText.text = "three";
            yield return new WaitForSeconds(1);
            sec--;

            countdownText.alignment = TextAlignmentOptions.Midline;
            countdownText.text = "two";
            yield return new WaitForSeconds(1);
            sec--;

            countdownText.alignment = TextAlignmentOptions.Bottom;
            countdownText.text = "one";
            yield return new WaitForSeconds(1);
            sec--;

            countdownText.alignment = TextAlignmentOptions.Midline;
            countdownText.text = "GO!";
            yield return new WaitForSeconds(1);
            sec--;
        }

        countdownPanel.SetActive(false);
        gameStarted = true;
        ozone.StartOzoneLayer();
        ds.StartDebrisSpawner();
        scoreText.gameObject.SetActive(true);
        liveText.gameObject.SetActive(true);
    }

    void MainMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver)
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
        StartCoroutine(camShake.Shake(cameraShakeDuration, cameraShakeAmount));
        StartCoroutine(ScreenTint(cameraShakeDuration));
        playerLives -= i;
        if(playerLives <= 0)
        {
            GameOver();
        }
    }

    IEnumerator ScreenTint(float _duration)
    {
        float elapsed = 0.0f;

        while (elapsed < _duration)
        {
            tint.SetActive(true);

            elapsed += Time.deltaTime;

            yield return null;
        }

        tint.SetActive(false);
    }

    void GameOver()
    {
        gameOver = true;
        tint.SetActive(true);
        gameOverText.text = "Game\nOver";
        scoreText.gameObject.SetActive(false);
        liveText.gameObject.SetActive(false);
        if (playerScore > PlayerPrefs.GetInt("TopScore"))
        {
            PlayerPrefs.SetInt("TopScore", playerScore);
        }

        finalScoreText.text = "Final Score: " + playerScore.ToString();
        topScoreText.text = "Top Score: " + PlayerPrefs.GetInt("TopScore").ToString();

        gameOverPanel.SetActive(true);
        ozone.StopOzoneLayer();
    }

    public void PlayAgain()
    {
        tint.SetActive(false);
        gameOverPanel.SetActive(false);
        gameOverText.text = "";
        liveText.text = "Lives: 3";
        scoreText.text = "0000";
        playerLives = maxPlayerLives;
        playerScore = 0;
        gameOver = false;
        gameStarted = false;
        StartCoroutine(GameStartCountDown());
    }

    public void QuitGame()
    {
        SceneManager.LoadScene(0);
    }
}