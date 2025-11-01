using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public class ForceReload
{
    static ForceReload()
    {
        // EditorApplication.delayCall += () =>
        // {
        //     EditorUtility.RequestScriptReload();
        //     Debug.Log("Requesting script reload to apply input settings...");
        // };
    }
}
