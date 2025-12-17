using UnityEngine;
using UnityEditor;

public class BeaconSpawnEditorWindow : EditorWindow
{
    BeaconSceneCompanion companion;

    [MenuItem("Window/Beacons")]
    public static void ShowWindow()
    {
        GetWindow<BeaconSpawnEditorWindow>("BeaconSpawnEditorWindow");
    }

    void OnGUI()
    {
        companion = GameObject.Find("BeaconSceneCompanion").GetComponent<BeaconSceneCompanion>();

        GUILayout.Label("Get Beacons from JSON", EditorStyles.boldLabel);

        if (GUILayout.Button("Load"))
        {
            LoadBeacons();
        }
    }

    public void LoadBeacons()
    {
        Debug.Log("Loading Beacons");
        companion.EditorLoadAnchors();
    }
}