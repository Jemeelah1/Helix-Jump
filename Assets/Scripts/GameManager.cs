using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
// using UnityEngine.Advertisements;

public class GameManager : MonoBehaviour {

    public static GameManager singleton;
    public int best;
    public int score;
    public int currentStage = 0;
    public enum GameMode { Classic, Timed, Endless, DoubleBounce }
    public GameMode currentGameMode;

    // Timer variables
    public float timePerLevel = 30f;  // Initial time for the first level
    private float levelTimer;
    public Text timerText;
    public GameObject gameOverPanel;
    public Button restartButton;
    private bool isGamePaused = false;

    [System.Serializable]
    public class LeaderboardEntry
    {
        public string playerName;
        public int score;
    }
    private List<LeaderboardEntry> leaderboard = new List<LeaderboardEntry>();
    

    private void Awake()
    {
        // Advertisement.Initialize("5720488");

        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy(gameObject);

        // Load the saved highscore
        best = PlayerPrefs.GetInt("Highscore");
    }

    private void Start()
    {
        currentGameMode = GameMode.Timed; // Set to Timed for testing

        LoadLeaderboard(); // Load the leaderboard on start
        StartLevelTimer();  // Start timer for the first level
        gameOverPanel.SetActive(false); // Hide Game Over panel initially
        restartButton.gameObject.SetActive(false);
        restartButton.onClick.AddListener(RestartLevel);
    }

    private void Update()
    {
        UpdateLevelTimer();  // Update timer each frame
    }

    public void StartLevelTimer()
    {
        levelTimer = timePerLevel;
        UpdateTimerUI();
    }

    private void UpdateLevelTimer()
    {
        // Only update timer if the game mode is Timed
        if (currentGameMode == GameMode.Timed && levelTimer > 0)
        {
            levelTimer -= Time.deltaTime;
            UpdateTimerUI();

            if (levelTimer <= 0)
            {
                levelTimer = 0;
                TimerRanOut(); // Trigger game over if time runs out
            }
        }
    }

    private void UpdateTimerUI()
    {
        timerText.text = $"Time: {Mathf.Ceil(levelTimer)}s";
    }

    private void TimerRanOut()
    {
        gameOverPanel.SetActive(true);
        restartButton.gameObject.SetActive(true);
        restartButton.onClick.AddListener(RestartLevel); // Restart the level or trigger Game Over
        Time.timeScale = 0; // Pause the game
    }

    public void NextLevel()
    {
        currentStage++;
        StartLevelTimer(); // Restart the timer for the new level

        FindObjectOfType<BallController>().ResetBall();
        FindObjectOfType<HelixController>().LoadStage(currentStage);
    }

    public void RestartLevel()
    {
        // Debug.Log("Show Adds");

        // Show Adds Advertisement.Show();
        // Advertisement.Show("Show Adds");

        Debug.Log("Restart button clicked!");
        Time.timeScale = 1; // Resume the game
        gameOverPanel.SetActive(false); // Hide the Game Over panel
        restartButton.gameObject.SetActive(false); // Hide the restart button
        currentStage = 0; // Reset current stage if needed
        
        singleton.score = 0;
        StartLevelTimer(); // Start the timer for the first level again
        FindObjectOfType<BallController>().ResetBall();
        FindObjectOfType<HelixController>().LoadStage(currentStage);

    }

    public void AddScore(int scoreToAdd)
    {
        score += scoreToAdd;

        if (score > best)
        {
            PlayerPrefs.SetInt("Highscore", score);
            best = score;
        }
    }

    public void SelectGameMode(GameMode selectedMode)
    {
        currentGameMode = selectedMode;
        StartLevelTimer();
        // ResetGame();
    }

    public void ReportScoreToLeaderboard(int score)
    {
        if (Social.localUser.authenticated)
        {
            Social.ReportScore(score, "YOUR_LEADERBOARD_ID", (bool success) => {
                Debug.Log(success ? "Score reported successfully!" : "Failed to report score.");
            });
        }
    }

    private void LoadLeaderboard()
    {
        // Placeholder for loading leaderboard data.
        leaderboard = new List<LeaderboardEntry>();
    }
}
