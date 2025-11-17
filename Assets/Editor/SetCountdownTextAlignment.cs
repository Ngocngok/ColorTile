using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public static class SetCountdownTextAlignment
{
    public static void Execute()
    {
        GameObject countdownText = GameObject.Find("CountdownText");
        if (countdownText != null)
        {
            Text text = countdownText.GetComponent<Text>();
            if (text != null)
            {
                text.alignment = TextAnchor.MiddleCenter;
                EditorUtility.SetDirty(countdownText);
                Debug.Log("Countdown text alignment set to MiddleCenter");
            }
        }
        else
        {
            Debug.LogError("CountdownText not found!");
        }
    }
}
