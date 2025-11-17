using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CountdownManager : MonoBehaviour
{
    public static CountdownManager Instance { get; private set; }

    [Header("Countdown UI")]
    public GameObject countdownPanel;
    public Text countdownText;

    [Header("Countdown Settings")]
    public float countdownDuration = 1f; // Duration for each number (3, 2, 1)
    public float startTextDuration = 0.5f; // Duration for "START!" text

    [Header("Animation Settings")]
    public float scaleStart = 0.5f;
    public float scaleEnd = 1.5f;
    public AnimationCurve scaleCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("Colors")]
    public Color countdownColor = Color.white;
    public Color startColor = Color.green;

    private bool countdownActive = false;

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
        // Hide countdown panel initially
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }
    }

    public void StartCountdown()
    {
        if (!countdownActive)
        {
            StartCoroutine(CountdownSequence());
        }
    }

    private IEnumerator CountdownSequence()
    {
        countdownActive = true;

        // Show countdown panel
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(true);
        }

        // Countdown: 3, 2, 1
        yield return ShowCountdownNumber("3", countdownColor);
        yield return ShowCountdownNumber("2", countdownColor);
        yield return ShowCountdownNumber("1", countdownColor);
        
        // Show START!
        yield return ShowCountdownNumber("START!", startColor);

        // Hide countdown panel
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
        }

        countdownActive = false;

        // Resume the game
        if (GameManager.Instance != null)
        {
            GameManager.Instance.ResumeGame();
        }
    }

    private IEnumerator ShowCountdownNumber(string text, Color color)
    {
        if (countdownText == null)
            yield break;

        // Set text and color
        countdownText.text = text;
        countdownText.color = color;

        // Play countdown sound
        if (SoundManager.Instance != null && text != "START!")
        {
            SoundManager.Instance.PlayButtonClick(); // You can add a specific countdown sound
        }
        else if (text == "START!" && SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick(); // You can add a specific start sound
        }

        // Determine duration
        float duration = (text == "START!") ? startTextDuration : countdownDuration;

        // Animate the text
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime; // Use unscaled time since game is paused
            float t = elapsed / duration;

            // Scale animation
            float scale = Mathf.Lerp(scaleStart, scaleEnd, scaleCurve.Evaluate(t));
            countdownText.transform.localScale = Vector3.one * scale;

            // Fade out towards the end
            Color currentColor = color;
            if (t > 0.7f)
            {
                float fadeT = (t - 0.7f) / 0.3f;
                currentColor.a = Mathf.Lerp(1f, 0f, fadeT);
            }
            countdownText.color = currentColor;

            yield return null;
        }

        // Reset scale and alpha
        countdownText.transform.localScale = Vector3.one;
        Color resetColor = color;
        resetColor.a = 1f;
        countdownText.color = resetColor;
    }

    public bool IsCountdownActive()
    {
        return countdownActive;
    }
}
