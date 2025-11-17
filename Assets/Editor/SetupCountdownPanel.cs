using UnityEngine;
using UnityEditor;

public class SetupCountdownPanel
{
    [MenuItem("Tools/Setup Countdown Panel")]
    static void Setup()
    {
        GameObject countdownPanel = GameObject.Find("CountdownPanel");
        if (countdownPanel != null)
        {
            countdownPanel.SetActive(false);
            Debug.Log("Countdown panel set to inactive");
        }
        else
        {
            Debug.LogError("CountdownPanel not found!");
        }
    }
}
