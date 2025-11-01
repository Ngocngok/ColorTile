using UnityEngine;
using UnityEditor;

public class SetupEndScreen
{
    [MenuItem("Tools/Setup End Screen")]
    public static void Execute()
    {
        GameObject endScreenPanel = GameObject.Find("Canvas/EndScreenPanel");
        if (endScreenPanel != null)
        {
            endScreenPanel.SetActive(false);
            Debug.Log("End Screen Panel set to inactive");
        }
        else
        {
            Debug.LogError("End Screen Panel not found");
        }
    }
}
