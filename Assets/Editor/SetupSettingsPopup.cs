using UnityEngine;
using UnityEditor;

public class SetupSettingsPopup
{
    public static void Execute()
    {
        GameObject settingsPopup = GameObject.Find("Canvas/SettingsPopup");
        if (settingsPopup != null)
        {
            settingsPopup.SetActive(false);
            Debug.Log("Settings popup set to inactive");
        }
        
        GameObject settingsButton = GameObject.Find("Canvas/SettingsButton");
        if (settingsButton != null)
        {
            var button = settingsButton.GetComponent<UnityEngine.UI.Button>();
            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => {
                    if (settingsPopup != null)
                    {
                        var popup = settingsPopup.GetComponent<SettingsPopup>();
                        if (popup != null)
                        {
                            popup.OpenPopup();
                        }
                    }
                });
                Debug.Log("Settings button wired up");
            }
        }
    }
}
