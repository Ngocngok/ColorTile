using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public static TutorialManager Instance { get; private set; }

    [Header("Tutorial UI")]
    public GameObject tutorialOverlay;
    public Image tutorialImage;
    public Button nextButton;
    public Button okButton;
    public Text descriptionText;

    [Header("Tutorial Images")]
    public Sprite tutorialStep1;
    public Sprite tutorialStep2;
    public Sprite tutorialStep3;
    public string[] dess;

    private int currentStep = 0;
    private const string TUTORIAL_SHOWN_KEY = "TutorialShown";

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
        // Setup button listeners
        if (nextButton != null)
        {
            nextButton.onClick.AddListener(OnNextClicked);
        }

        if (okButton != null)
        {
            okButton.onClick.AddListener(OnOkClicked);
        }

        // Check if tutorial should be shown
        if (ShouldShowTutorial())
        {
            ShowTutorial();
        }
        else
        {
            HideTutorial();
            // No tutorial, start countdown immediately
            if (GameManager.Instance != null)
            {
                GameManager.Instance.StartGameCountdown();
            }
        }
    }

    bool ShouldShowTutorial()
    {
        // Check if tutorial has been shown before
        return PlayerPrefs.GetInt(TUTORIAL_SHOWN_KEY, 0) == 0;
    }

    void ShowTutorial()
    {
        if (tutorialOverlay != null)
        {
            tutorialOverlay.SetActive(true);
        }

        currentStep = 0;
        UpdateTutorialStep();

        // Pause game logic
        if (GameManager.Instance != null)
        {
            // We'll add a pause method to GameManager
            GameManager.Instance.PauseGame();
        }
    }

    void HideTutorial()
    {
        if (tutorialOverlay != null)
        {
            tutorialOverlay.SetActive(false);
        }
    }

    void UpdateTutorialStep()
    {
        // Update tutorial image based on current step
        switch (currentStep)
        {
            case 0:
                if (tutorialImage != null && tutorialStep1 != null)
                {
                    tutorialImage.sprite = tutorialStep1;
                    descriptionText.text = dess[currentStep];
                }
                ShowNextButton();
                break;
            case 1:
                if (tutorialImage != null && tutorialStep2 != null)
                {
                    tutorialImage.sprite = tutorialStep2;
                    descriptionText.text = dess[currentStep];
                }
                ShowNextButton();
                break;
            case 2:
                if (tutorialImage != null && tutorialStep3 != null)
                {
                    tutorialImage.sprite = tutorialStep3;
                    descriptionText.text = dess[currentStep];
                }
                ShowOkButton();
                break;
        }
    }

    void ShowNextButton()
    {
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(true);
        }
        if (okButton != null)
        {
            okButton.gameObject.SetActive(false);
        }
    }

    void ShowOkButton()
    {
        if (nextButton != null)
        {
            nextButton.gameObject.SetActive(false);
        }
        if (okButton != null)
        {
            okButton.gameObject.SetActive(true);
        }
    }

    void OnNextClicked()
    {
        // Play button click sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }

        currentStep++;
        if (currentStep <= 2)
        {
            UpdateTutorialStep();
        }
    }

    void OnOkClicked()
    {
        // Play button click sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }

        // Mark tutorial as shown
        PlayerPrefs.SetInt(TUTORIAL_SHOWN_KEY, 1);
        PlayerPrefs.Save();

        // Hide tutorial
        HideTutorial();

        // Start countdown instead of resuming immediately
        if (GameManager.Instance != null)
        {
            GameManager.Instance.StartGameCountdown();
        }
    }

    // Public method to reset tutorial (for testing)
    public void ResetTutorial()
    {
        PlayerPrefs.SetInt(TUTORIAL_SHOWN_KEY, 0);
        PlayerPrefs.Save();
    }
}
