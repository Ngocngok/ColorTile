using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;

public class LoadingManager : MonoBehaviour
{
    [Header("UI Elements")]
    public Slider loadingBar;
    public Text loadingText;

    [Header("Settings")]
    public float loadingDuration = 2f;
    public string nextSceneName = "Home";

    void Start()
    {
        StartCoroutine(LoadingSequence());
    }

    IEnumerator LoadingSequence()
    {
        float elapsed = 0f;

        while (elapsed < loadingDuration)
        {
            elapsed += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsed / loadingDuration);

            // Update UI
            if (loadingBar != null)
            {
                loadingBar.value = progress;
            }

            if (loadingText != null)
            {
                loadingText.text = "Loading... " + Mathf.RoundToInt(progress * 100f) + "%";
            }

            yield return null;
        }

        // Ensure we reach 100%
        if (loadingBar != null)
        {
            loadingBar.value = 1f;
        }
        if (loadingText != null)
        {
            loadingText.text = "Loading... 100%";
        }

        yield return new WaitForSeconds(0.5f);

        // Load next scene
        SceneManager.LoadScene(nextSceneName);
    }
}
