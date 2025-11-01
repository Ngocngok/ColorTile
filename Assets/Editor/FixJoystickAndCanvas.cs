using UnityEngine;
using UnityEditor;
using UnityEngine.UI;

public class FixJoystickAndCanvas
{
    [MenuItem("Tools/Fix Joystick And Canvas Settings")]
    public static void Execute()
    {
        // Fix Joystick references
        GameObject joystickArea = GameObject.Find("Canvas/JoystickArea");
        if (joystickArea != null)
        {
            VirtualJoystick joystick = joystickArea.GetComponent<VirtualJoystick>();
            if (joystick != null)
            {
                GameObject background = GameObject.Find("Canvas/JoystickArea/JoystickBackground");
                GameObject handle = GameObject.Find("Canvas/JoystickArea/JoystickBackground/JoystickHandle");

                if (background != null && handle != null)
                {
                    SerializedObject so = new SerializedObject(joystick);
                    so.FindProperty("joystickBackground").objectReferenceValue = background.GetComponent<RectTransform>();
                    so.FindProperty("joystickHandle").objectReferenceValue = handle.GetComponent<RectTransform>();
                    so.FindProperty("handleRange").floatValue = 50f;
                    so.ApplyModifiedProperties();
                    EditorUtility.SetDirty(joystick);
                    Debug.Log("Joystick references fixed!");
                }
            }
        }

        // Fix all Canvas Scalers in current scene
        Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
        foreach (Canvas canvas in canvases)
        {
            CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
            if (scaler == null)
            {
                scaler = canvas.gameObject.AddComponent<CanvasScaler>();
            }

            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1080, 1920);
            scaler.matchWidthOrHeight = 0f;
            scaler.referencePixelsPerUnit = 100f;
            
            EditorUtility.SetDirty(scaler);
            Debug.Log("Fixed Canvas Scaler for: " + canvas.name);
        }

        Debug.Log("All fixes applied!");
    }
}
