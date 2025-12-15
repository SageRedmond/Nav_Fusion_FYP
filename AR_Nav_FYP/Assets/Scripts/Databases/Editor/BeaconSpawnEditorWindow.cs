using UnityEngine;
using UnityEditor;

public class BeaconSpawnEditorWindow : EditorWindow
{
    BeaconSpawnEditorCompanion companion;

    [MenuItem("Window/Beacons")]
    public static void ShowWindow()
    {
        GetWindow<BeaconSpawnEditorWindow>("BeaconSpawnEditorWindow");
    }

    void OnGUI()
    {
        companion = GameObject.Find("BeaconSpawnEditorCompanion").GetComponent<BeaconSpawnEditorCompanion>();

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