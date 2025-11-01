using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [Header("Map Size Buttons")]
    public Button button10x10;
    public Button button15x15;
    public Button button20x20;

    void Start()
    {
        // Add button listeners
        if (button10x10 != null)
        {
            button10x10.onClick.AddListener(() => SelectMapSize(10));
        }
        if (button15x15 != null)
        {
            button15x15.onClick.AddListener(() => SelectMapSize(15));
        }
        if (button20x20 != null)
        {
            button20x20.onClick.AddListener(() => SelectMapSize(20));
        }
    }

    void SelectMapSize(int size)
    {
        // Play button click sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }

        // Create GameSettings if it doesn't exist
        GameSettings settings = FindObjectOfType<GameSettings>();
        if (settings == null)
        {
            GameObject settingsObj = new GameObject("GameSettings");
            settings = settingsObj.AddComponent<GameSettings>();
        }

        settings.SetMapSize(size);

        // Load Game scene
        SceneManager.LoadScene("Game");
    }
}
