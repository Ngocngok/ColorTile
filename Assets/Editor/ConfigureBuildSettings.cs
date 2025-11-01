using UnityEditor;
using System.Collections.Generic;

public class ConfigureBuildSettings
{
    [MenuItem("Tools/Configure Build Settings")]
    public static void Execute()
    {
        List<EditorBuildSettingsScene> editorBuildSettingsScenes = new List<EditorBuildSettingsScene>();
        
        // Add scenes in correct order
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Loading.unity", true));
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Home.unity", true));
        editorBuildSettingsScenes.Add(new EditorBuildSettingsScene("Assets/Scenes/Game.unity", true));
        
        EditorBuildSettings.scenes = editorBuildSettingsScenes.ToArray();
        
        UnityEngine.Debug.Log("Build settings configured! Scenes order: Loading (0), Home (1), Game (2)");
    }
}
