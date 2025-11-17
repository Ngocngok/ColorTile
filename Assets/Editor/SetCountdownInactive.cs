using UnityEngine;
using UnityEditor;

public static class SetCountdownInactive
{
    public static void Execute()
    {
        GameObject countdownPanel = GameObject.Find("CountdownPanel");
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
            EditorUtility.SetDirty(countdownPanel);
            Debug.Log("Countdown panel set to inactive");
        }
        else
        {
            Debug.LogError("CountdownPanel not found!");
        }
    }
}
