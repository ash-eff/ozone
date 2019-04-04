using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public GameObject pausedPanel;
    public GameObject gameOverPanel;
    public GameObject tint;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI finalScoreText;
    public TextMeshProUGUI topScoreText;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI liveText;
    public TextMeshProUGUI gameOverText;
    public Color scoreColor;
    public Color resetColor;
    public Slider musicSlider;
    public Slider soundSlider;

    DebrisSpawner ds;
    CameraShake camShake;
    Ozone ozone;

    [SerializeField]
    private int maxPlayerLives;
    [SerializeField]
    private float cameraShakeDuration;
    [SerializeField]
    private float cameraShakeAmount;
    [SerializeField]
    private AudioClip countShort;
    [SerializeField]
    private AudioClip countLong;
    [SerializeField]
    private AudioClip explosionEarth;
    [SerializeField]
    private AudioClip explosionOzone;
    [SerializeField]
    private AudioClip powerUp;
    [SerializeField]
    private AudioClip powerDown;

    [SerializeField]
    AudioSource audioSource;
    [SerializeField]
    AudioSource musicSource;

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

        if (!PlayerPrefs.HasKey("MusicVol"))
        {
            PlayerPrefs.SetFloat("MusicVol", 1f);
        }

        if (!PlayerPrefs.HasKey("FXVol"))
        {
            PlayerPrefs.SetFloat("FXVol", 1f);
        }

        musicSlider.value = PlayerPrefs.GetFloat("MusicVol");
        soundSlider.value = PlayerPrefs.GetFloat("FXVol");
        musicSource.volume = musicSlider.value;
        audioSource.volume = soundSlider.value / 2;
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
        countdownText.gameObject.SetActive(true);

        while(sec > 0)
        {
            //countdownText.alignment = TextAlignmentOptions.Top;
            countdownText.text = "three";
            audioSource.PlayOneShot(countShort);
            yield return new WaitForSeconds(1);
            sec--;

            //countdownText.alignment = TextAlignmentOptions.Midline;
            countdownText.text = "two";
            audioSource.PlayOneShot(countShort);
            yield return new WaitForSeconds(1);
            sec--;

            //countdownText.alignment = TextAlignmentOptions.Bottom;
            countdownText.text = "one";
            audioSource.PlayOneShot(countShort);
            yield return new WaitForSeconds(1);
            sec--;

            //countdownText.alignment = TextAlignmentOptions.Midline;
            countdownText.text = "GO!";
            audioSource.PlayOneShot(countLong);
            yield return new WaitForSeconds(1);
            sec--;
        }

        countdownText.gameObject.SetActive(false);
        gameStarted = true;
        ozone.StartOzoneLayer();
        audioSource.PlayOneShot(powerUp);
        StartCoroutine(ds.SpawnDebris());
        scoreText.gameObject.SetActive(true);
        if (!musicSource.isPlaying)
        {
            musicSource.Play();
        }
        StartCoroutine(ScoreAlpha());
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

    IEnumerator ScoreAlpha()
    {
        float currentLerpTime = 0;
        while (currentLerpTime < 1)
        {
            currentLerpTime += Time.deltaTime;

            float perc = currentLerpTime / 10f;
            scoreText.color = Color.Lerp(scoreText.color, scoreColor, perc);
            yield return null;
        }

        liveText.gameObject.SetActive(true);
    }

    void MainMenu()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !gameOver)
        {
            paused = !paused;
            PlayerPrefs.SetFloat("MusicVol", musicSlider.value);
            PlayerPrefs.SetFloat("FXVol", soundSlider.value);
        }

        if (paused)
        {
            Time.timeScale = 0;
            liveText.gameObject.SetActive(false);
            pausedPanel.SetActive(true);
            Debug.Log(musicSlider.value);
            musicSource.volume = musicSlider.value;
            audioSource.volume = soundSlider.value / 2;
        }
        else
        {
            Time.timeScale = 1;
            liveText.gameObject.SetActive(true);
            pausedPanel.SetActive(false);
        }
    }

    void GameOver()
    {
        gameOver = true;
        tint.SetActive(true);
        gameOverText.text = "Game\nOver";
        scoreText.color = resetColor;
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
        audioSource.PlayOneShot(powerDown);
    }

    public void UpdateScore(int i)
    {
        audioSource.PlayOneShot(explosionOzone);
        playerScore += i;
    }

    public void UpdateLives(int i)
    {
        audioSource.PlayOneShot(explosionEarth);
        StartCoroutine(camShake.Shake(cameraShakeDuration, cameraShakeAmount));
        StartCoroutine(ScreenTint(cameraShakeDuration));
        playerLives -= i;
        if(playerLives <= 0)
        {
            GameOver();
        }
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