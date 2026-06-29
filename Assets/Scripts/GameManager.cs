using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public GridManager gridManager;
    public PieceSpawner pieceSpawner;

    public TMP_Text scoreText;
    public GameObject gameOverPanel;
    public TMP_Text gameOverText;

    public float startFallInterval = 0.8f;
    public float minFallInterval = 0.2f;
    public int scorePerSpeedStep = 200;
    public float speedIncreasePerStep = 0.04f;

    public GameObject startPanel;
    public bool IsGameStarted { get; private set; }
    public bool IsGameOver { get; private set; }

    private int score;
    private bool hasSpawnedFirstPiece;

    public AudioClip hardDropSound;
    public AudioClip lockSound;
    public AudioClip clearLayerSound;
    public AudioClip gameOverSound;

    public AudioClip backgroundMusic;

    private AudioSource sfxAudioSource;
    private AudioSource musicAudioSource;

    private void Start()
    {
        AudioSource[] audioSources = GetComponents<AudioSource>();

        if (audioSources.Length > 0)
        {
            sfxAudioSource = audioSources[0];
        }

        if (audioSources.Length > 1)
        {
            musicAudioSource = audioSources[1];
        }

        StartBackgroundMusic();

        score = 0;
        IsGameOver = false;
        IsGameStarted = false;

        UpdateScoreText();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }

        pieceSpawner.Initialize(gridManager, this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscape();
            return;
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartScene();
        }

        if (!IsGameStarted && !IsGameOver)
        {
            if (Input.GetKeyDown(KeyCode.Return))
            {
                StartGame();
            }

            return;
        }
    }

    private void StartGame()
    {
        IsGameStarted = true;

        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }

        StartBackgroundMusic();

        if (!hasSpawnedFirstPiece)
        {
            pieceSpawner.SpawnNewPiece();
            hasSpawnedFirstPiece = true;
        }
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreText();
    }

    public float GetCurrentFallInterval()
    {
        int speedStep = score / scorePerSpeedStep;

        float currentInterval = startFallInterval - speedStep * speedIncreasePerStep;

        if (currentInterval < minFallInterval)
        {
            currentInterval = minFallInterval;
        }

        return currentInterval;
    }

    public void GameOver()
    {
        IsGameOver = true;

        PlayGameOverSound();

        if (startPanel != null)
        {
            startPanel.SetActive(false);
        }

        if (gameOverText != null)
        {
            gameOverText.text =
                "GAME OVER" +
                "\nFinal Score: " + score +
                "\nPress R to Restart" +
                "\nPress Esc to Quit";
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Debug.Log("Game Over!");
    }

    private void UpdateScoreText()
    {
        if (scoreText != null)
        {
            scoreText.text =
                "Score: " + score +
                "\nFall: " + GetCurrentFallInterval().ToString("0.00") + "s" +
                "\nMove: WASD" +
                "\nRotate: Q/E" +
                "\nDrop: Space" +
                "\nRestart: R" +
                "\nPause: ESC";
        }
    }

    private void RestartScene()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }

    public void PlayHardDropSound()
    {
        PlaySound(hardDropSound);
    }

    public void PlayLockSound()
    {
        PlaySound(lockSound);
    }

    public void PlayClearLayerSound()
    {
        PlaySound(clearLayerSound);
    }

    public void PlayGameOverSound()
    {
        PlaySound(gameOverSound);
    }

    private void PlaySound(AudioClip clip)
    {
        if (sfxAudioSource == null)
        {
            return;
        }

        if (clip == null)
        {
            return;
        }

        sfxAudioSource.PlayOneShot(clip);
    }

    private void StartBackgroundMusic()
    {
        if (musicAudioSource == null)
        {
            return;
        }

        if (backgroundMusic != null)
        {
            musicAudioSource.clip = backgroundMusic;
        }

        musicAudioSource.loop = true;
        musicAudioSource.Play();
    }

    private void HandleEscape()
    {
        if (!IsGameStarted || IsGameOver)
        {
            QuitGame();
            return;
        }

        ReturnToStartPanel();
    }

    private void ReturnToStartPanel()
    {
        IsGameStarted = false;

        if (startPanel != null)
        {
            startPanel.SetActive(true);
        }

        if (musicAudioSource != null)
        {
            musicAudioSource.Pause();
        }
    }

    private void QuitGame()
    {
        Debug.Log("Quit Game");
        Application.Quit();
    }

}