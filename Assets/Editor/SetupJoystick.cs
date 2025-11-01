using UnityEngine;
using UnityEditor;

public class SetupJoystick
{
    [MenuItem("Tools/Setup Joystick References")]
    public static void Execute()
    {
        GameObject joystickArea = GameObject.Find("Canvas/JoystickArea");
        if (joystickArea == null)
        {
            Debug.LogError("JoystickArea not found!");
            return;
        }

        VirtualJoystick joystick = joystickArea.GetComponent<VirtualJoystick>();
        if (joystick == null)
        {
            Debug.LogError("VirtualJoystick component not found!");
            return;
        }

        GameObject background = GameObject.Find("Canvas/JoystickArea/JoystickBackground");
        GameObject handle = GameObject.Find("Canvas/JoystickArea/JoystickBackground/JoystickHandle");

        if (background == null || handle == null)
        {
            Debug.LogError("Joystick UI elements not found!");
            return;
        }

        SerializedObject so = new SerializedObject(joystick);
        so.FindProperty("joystickBackground").objectReferenceValue = background.GetComponent<RectTransform>();
        so.FindProperty("joystickHandle").objectReferenceValue = handle.GetComponent<RectTransform>();
        so.FindProperty("handleRange").floatValue = 50f;
        so.ApplyModifiedProperties();

        EditorUtility.SetDirty(joystick);
        
        Debug.Log("Joystick references set successfully!");
    }
}
