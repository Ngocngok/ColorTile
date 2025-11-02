using UnityEngine;
using UnityEditor;

public class ReplaceTileModel
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

        // Load the FBX model
        GameObject fbxModel = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/fbx/tileLow_teamRed.fbx");
        if (fbxModel == null)
        {
            Debug.LogError("tileLow_teamRed.fbx not found!");
            return;
        }

        // Open prefab for editing
        string prefabPath = AssetDatabase.GetAssetPath(prefab);
        GameObject prefabInstance = PrefabUtility.LoadPrefabContents(prefabPath);

        // Instantiate the FBX model as a child
        GameObject modelInstance = (GameObject)PrefabUtility.InstantiatePrefab(fbxModel, prefabInstance.transform);
        modelInstance.name = "TileModel";

        // Get the bounds of the model to calculate scale
        MeshFilter[] meshFilters = modelInstance.GetComponentsInChildren<MeshFilter>();
        if (meshFilters.Length > 0 && meshFilters[0].sharedMesh != null)
        {
            Bounds bounds = meshFilters[0].sharedMesh.bounds;
            Debug.Log($"Original model bounds: {bounds.size}");
            
            // Target size is approximately 1x1x1 (the tile size in world space)
            // The original cube had scale 0.9, 0.1, 0.9
            // Calculate scale to match
            float targetSize = 0.9f;
            float targetHeight = 0.1f;
            
            float scaleX = targetSize / bounds.size.x;
            float scaleY = targetHeight / bounds.size.y;
            float scaleZ = targetSize / bounds.size.z;
            
            modelInstance.transform.localScale = new Vector3(scaleX, scaleY, scaleZ);
            modelInstance.transform.localPosition = Vector3.zero;
            modelInstance.transform.localRotation = Quaternion.identity;
            
            Debug.Log($"Applied scale: {modelInstance.transform.localScale}");
        }

        // Save the prefab
        PrefabUtility.SaveAsPrefabAsset(prefabInstance, prefabPath);
        PrefabUtility.UnloadPrefabContents(prefabInstance);

        Debug.Log("Tile model replaced successfully!");
    }
}
