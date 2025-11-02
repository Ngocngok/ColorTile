using UnityEngine;
using UnityEditor;

public class SetupDecoratorManager
{
    public static void Execute()
    {
        // Find DecoratorManager in scene
        DecoratorManager decoratorManager = GameObject.FindObjectOfType<DecoratorManager>();
        if (decoratorManager == null)
        {
            Debug.LogError("DecoratorManager not found in scene!");
            return;
        }

        // Load FBX models
        GameObject barrierStrut = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/fbx/barrierStrut.fbx");
        GameObject flag_teamRed = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/fbx/flag_teamRed.fbx");
        GameObject flag_teamYellow = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/fbx/flag_teamYellow.fbx");
        GameObject plantB_forest = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/fbx/plantB_forest.fbx");
        GameObject spikeRoller = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/fbx/spikeRoller.fbx");
        GameObject targetStand = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/fbx/targetStand.fbx");
        GameObject tree_forest = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/fbx/tree_forest.fbx");
        GameObject tree_desert = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/fbx/tree_desert.fbx");

        // Assign to DecoratorManager using reflection
        var type = typeof(DecoratorManager);
        type.GetField("barrierStrut").SetValue(decoratorManager, barrierStrut);
        type.GetField("flag_teamRed").SetValue(decoratorManager, flag_teamRed);
        type.GetField("flag_teamYellow").SetValue(decoratorManager, flag_teamYellow);
        type.GetField("plantB_forest").SetValue(decoratorManager, plantB_forest);
        type.GetField("spikeRoller").SetValue(decoratorManager, spikeRoller);
        type.GetField("targetStand").SetValue(decoratorManager, targetStand);
        type.GetField("tree_forest").SetValue(decoratorManager, tree_forest);
        type.GetField("tree_desert").SetValue(decoratorManager, tree_desert);

        // Find GridManager and link DecoratorManager
        GridManager gridManager = GameObject.FindObjectOfType<GridManager>();
        if (gridManager != null)
        {
            var gridType = typeof(GridManager);
            gridType.GetField("decoratorManager").SetValue(gridManager, decoratorManager);
            EditorUtility.SetDirty(gridManager);
        }

        EditorUtility.SetDirty(decoratorManager);

        Debug.Log("DecoratorManager setup complete!");
        Debug.Log($"barrierStrut: {barrierStrut != null}");
        Debug.Log($"flag_teamRed: {flag_teamRed != null}");
        Debug.Log($"flag_teamYellow: {flag_teamYellow != null}");
        Debug.Log($"plantB_forest: {plantB_forest != null}");
        Debug.Log($"spikeRoller: {spikeRoller != null}");
        Debug.Log($"targetStand: {targetStand != null}");
        Debug.Log($"tree_forest: {tree_forest != null}");
        Debug.Log($"tree_desert: {tree_desert != null}");
    }
}
