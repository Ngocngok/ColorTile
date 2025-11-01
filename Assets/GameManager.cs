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
        // Load high score
        highScore = PlayerPrefs.GetInt("HighScore", 0);
        
        // Wait for grid to be ready
        Invoke("StartGame", 0.5f);
    }

    void Update()
    {
        if (gameActive)
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

    public void StartGame()
    {
        gameActive = true;
        timeRemaining = gameDuration;

        // Play game background music
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayBgGame();
        }

        // Reset grid
        if (GridManager.Instance != null)
        {
            GridManager.Instance.ResetGrid();
        }

        // Spawn player
        if (player == null)
        {
            SpawnPlayer();
        }
        else
        {
            player.GetComponent<PlayerController>().ResetPosition();
        }

        // Spawn bots
        if (bots == null || bots.Length != numberOfBots)
        {
            SpawnBots();
        }
        else
        {
            foreach (GameObject bot in bots)
            {
                if (bot != null)
                {
                    bot.GetComponent<BotController>().ResetPosition();
                }
            }
        }

        UIManager.Instance?.UpdateGameState(true);
    }

    void SpawnPlayer()
    {
        Vector3 spawnPos = new Vector3(0, 0.5f, 0);
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
            
            Vector3 spawnPos = new Vector3(0, 0.5f, 0);
            bots[i] = Instantiate(botPrefabs[i], spawnPos, Quaternion.identity);
            
            BotController botController = bots[i].GetComponent<BotController>();
            if (botController != null)
            {
                botController.myTileState = botStates[i];
            }
        }
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
            PlayerPrefs.SetInt("HighScore", highScore);
            PlayerPrefs.Save();
        }
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
        StartGame();
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
}

