using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance { get; private set; }

    [Header("Audio Sources")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Background Music")]
    public AudioClip backgroundMusic;

    [Header("Sound Effects")]
    public AudioClip buttonClickSFX;
    public AudioClip winSFX;
    public AudioClip loseSFX;
    public AudioClip claimTileSFX;

    private bool musicEnabled = true;
    private bool sfxEnabled = true;

    void Awake()
    {
        // Singleton pattern with DontDestroyOnLoad
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            
            // Create audio sources if not assigned
            if (musicSource == null)
            {
                musicSource = gameObject.AddComponent<AudioSource>();
                musicSource.loop = true;
                musicSource.playOnAwake = false;
            }
            
            if (sfxSource == null)
            {
                sfxSource = gameObject.AddComponent<AudioSource>();
                sfxSource.loop = false;
                sfxSource.playOnAwake = false;
            }
            
            LoadSettings();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Start playing background music if enabled
        if (musicEnabled && backgroundMusic != null)
        {
            PlayBackgroundMusic();
        }
    }

    void LoadSettings()
    {
        // Load settings from PlayerPrefs (default: ON = 1)
        musicEnabled = PlayerPrefs.GetInt("MusicEnabled", 1) == 1;
        sfxEnabled = PlayerPrefs.GetInt("SFXEnabled", 1) == 1;
    }

    void SaveSettings()
    {
        PlayerPrefs.SetInt("MusicEnabled", musicEnabled ? 1 : 0);
        PlayerPrefs.SetInt("SFXEnabled", sfxEnabled ? 1 : 0);
        PlayerPrefs.Save();
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusic != null && musicEnabled)
        {
            musicSource.clip = backgroundMusic;
            musicSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        musicSource.Stop();
    }

    public void PlayButtonClick()
    {
        PlaySFX(buttonClickSFX);
    }

    public void PlayWin()
    {
        PlaySFX(winSFX);
    }

    public void PlayLose()
    {
        PlaySFX(loseSFX);
    }

    public void PlayClaimTile()
    {
        PlaySFX(claimTileSFX);
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null && sfxEnabled)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void SetMusicEnabled(bool enabled)
    {
        musicEnabled = enabled;
        
        if (musicEnabled)
        {
            if (!musicSource.isPlaying && backgroundMusic != null)
            {
                PlayBackgroundMusic();
            }
        }
        else
        {
            StopBackgroundMusic();
        }
        
        SaveSettings();
    }

    public void SetSFXEnabled(bool enabled)
    {
        sfxEnabled = enabled;
        SaveSettings();
    }

    public bool IsMusicEnabled()
    {
        return musicEnabled;
    }

    public bool IsSFXEnabled()
    {
        return sfxEnabled;
    }

    public void ToggleMusic()
    {
        SetMusicEnabled(!musicEnabled);
    }

    public void ToggleSFX()
    {
        SetSFXEnabled(!sfxEnabled);
    }
}
