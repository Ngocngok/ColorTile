using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : MonoBehaviour
{
    [Header("UI References")]
    public Toggle musicToggle;
    public Toggle sfxToggle;
    public Button closeButton;

    void Start()
    {
        // Initialize toggles with current settings
        if (SoundManager.Instance != null)
        {
            if (musicToggle != null)
            {
                musicToggle.isOn = SoundManager.Instance.IsMusicEnabled();
                musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
            }
            
            if (sfxToggle != null)
            {
                sfxToggle.isOn = SoundManager.Instance.IsSFXEnabled();
                sfxToggle.onValueChanged.AddListener(OnSFXToggleChanged);
            }
        }

        // Setup close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePopup);
        }
    }

    void OnMusicToggleChanged(bool isOn)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetMusicEnabled(isOn);
        }
    }

    void OnSFXToggleChanged(bool isOn)
    {
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.SetSFXEnabled(isOn);
        }
    }

    public void ClosePopup()
    {
        // Play button click sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }
        
        gameObject.SetActive(false);
    }

    public void OpenPopup()
    {
        // Play button click sound
        if (SoundManager.Instance != null)
        {
            SoundManager.Instance.PlayButtonClick();
        }
        
        gameObject.SetActive(true);
        
        // Refresh toggle states
        if (SoundManager.Instance != null)
        {
            if (musicToggle != null)
            {
                musicToggle.isOn = SoundManager.Instance.IsMusicEnabled();
            }
            
            if (sfxToggle != null)
            {
                sfxToggle.isOn = SoundManager.Instance.IsSFXEnabled();
            }
        }
    }
}
