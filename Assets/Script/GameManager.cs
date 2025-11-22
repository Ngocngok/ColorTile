using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    public float gameDuration = 60f;
    public int numberOfBots = 2;

    [Header("Prefabs")]
    public GameObject playerPrefab;
    public GameObject bot1Prefab; // Pig - Red
    public GameObject bot2Prefab; // Duck - Yellow
    public GameObject bot3Prefab; // Sheep - Green

    private float timeRemaining;
    private bool gameActive = false;
    private bool gamePaused = false;

    private GameObject player;
    private GameObject[] bots;

    private int playerScore = 0;
    private int highScore = 0;

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
    }

    void Start()
    {
        // Wait for grid to be ready, then initialize game
        Invoke("InitializeGame", 0.5f);
    }

    void Update()
    {
        if (gameActive && !gamePaused)
        {
            timeRemaining -= Time.deltaTime;
            
            // Check if time ran out
            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                EndGame();
                return;
            }
            
            // Check if all tiles are occupied
            if (AreAllTilesOccupied())
            {
                EndGame();
            }
        }
    }

    // Initialize game: spawn characters and pause
    void InitializeGame()
    {
        // Reset grid
        if (GridManager.Instance != null)
        {
            GridManager.Instance.ResetGrid();
        }

        // Load high score for the current map size
        string key = GetHighScoreKey();
        highScore = PlayerPrefs.GetInt(key, 0);

        // Spawn player
        if (player == null)
        {
            SpawnPlayer();
        }
        else
        {
            player.GetComponent<PlayerController>().ResetPosition(GetSpawnPosition(0));
        }

        // Spawn bots
        if (bots == null || bots.Length != numberOfBots)
        {
            SpawnBots();
        }
        else
        {
            for (int i = 0; i < bots.Length; i++)
            {
                if (bots[i] != null)
                {
                    bots[i].GetComponent<BotController>().ResetPosition(GetSpawnPosition(i + 1));
                }
            }
        }

        // Pause the game initially
        PauseGame();

        // Tutorial will be shown by TutorialManager if needed
        // After tutorial (or if skipped), TutorialManager or this will trigger countdown
    }

    // Called after tutorial ends or immediately if no tutorial
    public void StartGameCountdown()
    {
        if (CountdownManager.Instance != null)
        {
            CountdownManager.Instance.StartCountdown();
        }
        else
        {
            // Fallback: start game immediately if no countdown manager
            StartGame();
        }
    }

    // Actually start the game (called after countdown)
    public void StartGame()
    {
        gameActive = true;
        gamePaused = false; // Unpause the game
        timeRemaining = gameDuration;

        // Play game background music
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBgGame();
        }

        UIManager.Instance?.UpdateGameState(true);
    }

    void SpawnPlayer()
    {
        Vector3 spawnPos = GetSpawnPosition(0);
        player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
    }

    void SpawnBots()
    {
        // Clean up old bots
        if (bots != null)
        {
            foreach (GameObject bot in bots)
            {
                if (bot != null)
                {
                    Destroy(bot);
                }
            }
        }

        bots = new GameObject[numberOfBots];
        TileState[] botStates = { TileState.Bot1, TileState.Bot2, TileState.Bot3 };
        GameObject[] botPrefabs = { bot1Prefab, bot2Prefab, bot3Prefab };

        for (int i = 0; i < numberOfBots && i < botPrefabs.Length; i++)
        {
            if (botPrefabs[i] == null) continue;
            
            Vector3 spawnPos = GetSpawnPosition(i + 1);
            bots[i] = Instantiate(botPrefabs[i], spawnPos, Quaternion.identity);
            
            BotController botController = bots[i].GetComponent<BotController>();
            if (botController != null)
            {
                botController.myTileState = botStates[i];
            }
        }
    }

    Vector3 GetSpawnPosition(int index)
    {
        if (GridManager.Instance == null) return new Vector3(0, 0.5f, 0);

        int width = GridManager.Instance.gridWidth;
        int height = GridManager.Instance.gridHeight;
        float tileSize = GridManager.Instance.tileSize;

        int x = 0;
        int y = 0;

        // Define spawn points (inset by 2 to avoid corners/walls)
        // Ensure inset is not too large for small grids
        int inset = 2;
        if (width < 6 || height < 6)
        {
            inset = 1;
        }
        
        switch (index)
        {
            case 0: // Player: Bottom-Left
                x = inset;
                y = inset;
                break;
            case 1: // Bot 1: Top-Right
                x = width - 1 - inset;
                y = height - 1 - inset;
                break;
            case 2: // Bot 2: Top-Left
                x = inset;
                y = height - 1 - inset;
                break;
            case 3: // Bot 3: Bottom-Right
                x = width - 1 - inset;
                y = inset;
                break;
            default:
                x = width / 2;
                y = height / 2;
                break;
        }
        
        // Clamp to grid bounds
        x = Mathf.Clamp(x, 0, width - 1);
        y = Mathf.Clamp(y, 0, height - 1);

        return new Vector3(x * tileSize, 0.5f, y * tileSize);
    }

    void EndGame()
    {
        gameActive = false;

        // Stop game background music
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.StopBackgroundMusic();
        }

        // Calculate scores
        CalculateScores();

        // Show end screen
        UIManager.Instance?.ShowEndScreen();
    }

    void CalculateScores()
    {
        if (GridManager.Instance == null)
            return;

        playerScore = GridManager.Instance.CountTilesByState(TileState.Player);

        // Update high score
        if (playerScore > highScore)
        {
            highScore = playerScore;
            string key = GetHighScoreKey();
            PlayerPrefs.SetInt(key, highScore);
            PlayerPrefs.Save();
        }
    }

    private string GetHighScoreKey()
    {
        int size = 20; // Default
        if (GridManager.Instance != null)
        {
            size = GridManager.Instance.gridWidth;
        }
        else if (GameSettings.Instance != null)
        {
            size = GameSettings.Instance.GetMapSize();
        }
        
        return $"HighScore_{size}";
    }

    public bool IsGameActive()
    {
        return gameActive;
    }

    public float GetTimeRemaining()
    {
        return timeRemaining;
    }

    public int GetPlayerScore()
    {
        return playerScore;
    }

    public int GetHighScore()
    {
        return highScore;
    }

    public int GetBotScore(int botIndex)
    {
        if (GridManager.Instance == null)
            return 0;

        TileState botState = TileState.Bot1;
        if (botIndex == 0) botState = TileState.Bot1;
        else if (botIndex == 1) botState = TileState.Bot2;
        else if (botIndex == 2) botState = TileState.Bot3;

        return GridManager.Instance.CountTilesByState(botState);
    }

    public int GetTotalTiles()
    {
        if (GridManager.Instance == null)
            return 400;
        
        return GridManager.Instance.gridWidth * GridManager.Instance.gridHeight;
    }

    public void RestartGame()
    {
        InitializeGame();
        
        // Start countdown after a brief delay
        Invoke("StartGameCountdown", 0.1f);
    }

    public void LoadMainMenu()
    {
        // For now, just restart the scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public int GetNumberOfBots()
    {
        return numberOfBots;
    }

    bool AreAllTilesOccupied()
    {
        if (GridManager.Instance == null)
            return false;

        // Check if there are any empty tiles left
        int emptyTiles = GridManager.Instance.CountTilesByState(TileState.Empty);
        return emptyTiles == 0;
    }

    public void PauseGame()
    {
        gamePaused = true;
    }

    public void ResumeGame()
    {
        gamePaused = false;
    }

    public bool IsGamePaused()
    {
        return gamePaused;
    }
}

