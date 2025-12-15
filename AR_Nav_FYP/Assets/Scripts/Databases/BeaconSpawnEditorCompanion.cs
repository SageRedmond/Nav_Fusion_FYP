using UnityEngine;
using UnityEngine.Serialization;

public class BeaconSpawnEditorCompanion : MonoBehaviour
{
    [Tooltip("Ensure to change file extension from .json to .txt")]
    public TextAsset beaconJson;

    [SerializeField] private GameObject m_BeaconPrefab = null;

    [FormerlySerializedAs("m_ARSpace")]
    [SerializeField] private Immersal.XR.XRSpace m_XRSpace;

    [System.Serializable]
    public class BeaconList
    {
        public JSONBeacon[] beacons;
    }
    [System.Serializable]
    public class JSONBeacon
    {
        public string beaconId;
        public string roomId;
        public Vector3 XRPose;
        public Vector3 Rotation;
    }

    void OnValidate()
    {
        if (m_XRSpace == null)
        {
            m_XRSpace = FindFirstObjectByType<Immersal.XR.XRSpace>();
        }
    }

    public void SpawnLoadedBeacons(JSONBeacon beacon)
    {
        GameObject go = Instantiate(m_BeaconPrefab, m_XRSpace.transform);
        Quaternion rotation = Quaternion.Euler(beacon.Rotation);

        go.transform.SetLocalPositionAndRotation(beacon.XRPose, rotation);

        Beacon objectBeacon = go.GetComponent<Beacon>();
        objectBeacon.beaconId = beacon.beaconId;
        objectBeacon.roomId = beacon.roomId;
        objectBeacon.XRPose = beacon.XRPose;
        objectBeacon.Rotation = beacon.Rotation;
    }

    public void EditorLoadAnchors()
    {
        BeaconList savefile = JsonUtility.FromJson<BeaconList>(beaconJson.text);

        foreach (JSONBeacon beacon in savefile.beacons)
        {
            SpawnLoadedBeacons(beacon);
        }
    }
}