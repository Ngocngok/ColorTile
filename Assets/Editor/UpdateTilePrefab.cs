using UnityEngine;
using UnityEditor;

public class UpdateTilePrefab
{
    public static void Execute()
    {
        // Load the prefab
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Tile.prefab");
        if (prefab == null)
        {
            Debug.LogError("Tile prefab not found!");
            return;
        }

        // Open prefab for editing
        string prefabPath = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);

        // Find the nested model
        Transform modelTransform = prefabInstance.transform.Find("tileLow_teamRed");
        if (modelTransform == null)
        {
            Debug.LogError("tileLow_teamRed not found in prefab!");
            PrefabUtility.UnloadPrefabContents(prefabInstance);
            return;
        }

        // Get the bounds of the model to calculate scale
        MeshFilter meshFilter = modelTransform.GetComponentInChildren<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            Bounds bounds = meshFilter.sharedMesh.bounds;
            Debug.Log($"Original model bounds: {bounds.size}");
            
            // Target size is 0.9 x 0.1 x 0.9 (like the original cube)
            // Calculate scale needed
            float scaleX = 0.9f / bounds.size.x;
            float scaleY = 0.1f / bounds.size.y;
            float scaleZ = 0.9f / bounds.size.z;
            
            modelTransform.localScale = new Vector3(scaleX, scaleY, scaleZ);
            Debug.Log($"Applied scale: {modelTransform.localScale}");
        }

        // Save the prefab
        PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabInstance);

        Debug.Log("Tile prefab updated successfully!");
    }
}
