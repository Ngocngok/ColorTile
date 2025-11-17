using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("HUD")]
    public Text timerText;
    public Text playerScoreText;
    public Text playerPercentageText;
    public Text bot1ScoreTextHUD;
    public Text bot2ScoreTextHUD;
    public Text bot3ScoreTextHUD;
    public Button pauseButton;
    public GameObject pauseMenu;

    [Header("End Screen")]
    public GameObject endScreenPanel;
    public Text resultTitleText;
    public Text playerFinalScoreText;
    public Text bot1ScoreText;
    public Text bot2ScoreText;
    public Text bot3ScoreText;
    public Text highScoreText;
    public Button retryButton;
    public Button menuButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        pauseButton.onClick.AddListener(ShowPauseMenu);
    }

    void Start()
    {
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(false);
        }

        if (retryButton != null)
        {
            retryButton.onClick.AddListener(OnRetryClicked);
        }

        if (menuButton != null)
        {
            menuButton.onClick.AddListener(OnMenuClicked);
        }
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.IsGameActive())
        {
            UpdateHUD();
        }
    }

    void ShowPauseMenu()
    {
        pauseMenu.SetActive(true);
        GameManager.Instance.PauseGame();
    }

    void UpdateHUD()
    {
        // Update timer
        if (timerText != null)
        {
            float time = GameManager.Instance.GetTimeRemaining();
            int minutes = Mathf.FloorToInt(time / 60);
            int seconds = Mathf.FloorToInt(time % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

        // Update player score
        if (playerScoreText != null && GridManager.Instance != null)
        {
            int score = GridManager.Instance.CountTilesByState(TileState.Player);
            playerScoreText.text = score.ToString();
        }

        // Update percentage
        if (playerPercentageText != null && GridManager.Instance != null)
        {
            int score = GridManager.Instance.CountTilesByState(TileState.Player);
            int total = GameManager.Instance.GetTotalTiles();
            float percentage = (float)score / total * 100f;
            playerPercentageText.text = string.Format("{0:F1}%", percentage);
        }

        // Update bot scores
        if (GridManager.Instance != null)
        {
            if (bot1ScoreTextHUD != null)
            {
                int bot1Score = GridManager.Instance.CountTilesByState(TileState.Bot1);
                bot1ScoreTextHUD.text = bot1Score.ToString();
            }
            
            if (bot2ScoreTextHUD != null)
            {
                int bot2Score = GridManager.Instance.CountTilesByState(TileState.Bot2);
                bot2ScoreTextHUD.text = bot2Score.ToString();
            }
            
            if (bot3ScoreTextHUD != null)
            {
                int bot3Score = GridManager.Instance.CountTilesByState(TileState.Bot3);
                bot3ScoreTextHUD.text = bot3Score.ToString();
            }
        }
    }

    public void UpdateGameState(bool active)
    {
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(!active);
        }
    }

    public void ShowEndScreen()
    {
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(true);
        }

        // Get scores
        int playerScore = GameManager.Instance.GetPlayerScore();
        int totalTiles = GameManager.Instance.GetTotalTiles();
        float playerPercentage = (float)playerScore / totalTiles * 100f;

        // Determine winner
        bool playerWon = true;
        int numberOfBots = GameManager.Instance.GetNumberOfBots();
        
        for (int i = 0; i < numberOfBots; i++)
        {
            int botScore = GameManager.Instance.GetBotScore(i);
            if (botScore > playerScore)
            {
                playerWon = false;
                break;
            }
        }

        // Play win/lose sound
        if (SoundManager.Instance != null)
        {
            if (playerWon)
            {
                SoundManager.Instance.PlayWin();
            }
            else
            {
                SoundManager.Instance.PlayLose();
            }
        }

        // Update result title
        if (resultTitleText != null)
        {
            resultTitleText.text = playerWon ? "VICTORY!" : "DEFEAT!";
            resultTitleText.color = playerWon ? Color.green : Color.red;
        }

        // Update player score
        if (playerFinalScoreText != null)
        {
            playerFinalScoreText.text = string.Format("{0} tiles ({1:F1}%)", playerScore, playerPercentage);
        }

        // Update bot scores
        if (bot1ScoreText != null)
        {
            int bot1Score = GameManager.Instance.GetBotScore(0);
            float bot1Percentage = (float)bot1Score / totalTiles * 100f;
            bot1ScoreText.text = string.Format("{0} tiles ({1:F1}%)", bot1Score, bot1Percentage);
            
            if (numberOfBots < 1)
                bot1ScoreText.gameObject.SetActive(false);
        }

        if (bot2ScoreText != null)
        {
            if (numberOfBots >= 2)
            {
                int bot2Score = GameManager.Instance.GetBotScore(1);
                float bot2Percentage = (float)bot2Score / totalTiles * 100f;
                bot2ScoreText.text = string.Format("{0} tiles ({1:F1}%)", bot2Score, bot2Percentage);
            }
            else
            {
                bot2ScoreText.gameObject.SetActive(false);
            }
        }

        if (bot3ScoreText != null)
        {
            if (numberOfBots >= 3)
            {
                int bot3Score = GameManager.Instance.GetBotScore(2);
                float bot3Percentage = (float)bot3Score / totalTiles * 100f;
                bot3ScoreText.text = string.Format("{0} tiles ({1:F1}%)", bot3Score, bot3Percentage);
            }
            else
            {
                bot3ScoreText.gameObject.SetActive(false);
            }
        }

        // Update high score
        if (highScoreText != null)
        {
            highScoreText.text = "Best Score: " + GameManager.Instance.GetHighScore() + " tiles";
        }
    }

    void OnRetryClicked()
    {
        // Play button click sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }

        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(false);
        }
        GameManager.Instance?.RestartGame();
    }

    void OnMenuClicked()
    {
        // Play button click sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene("Home");
    }
}

