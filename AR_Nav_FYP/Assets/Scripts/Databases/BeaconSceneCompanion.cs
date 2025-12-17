using UnityEngine;
using UnityEngine.Serialization;
using System.IO;
using System.Linq;
// using System.Numerics;

public class BeaconSceneCompanion : MonoBehaviour
{
    [Tooltip("Ensure to change file extension from .json to .txt")]
    public TextAsset beaconJson;

    [SerializeField] private GameObject m_BeaconPrefab = null;

    [FormerlySerializedAs("m_ARSpace")]
    [SerializeField] private Immersal.XR.XRSpace m_XRSpace;

    private iDataService JsonService = new JsonDataService();

    private BeaconList m_BeaconList;

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
        public float xpos;
        public float ypos;
        public float zpos;
        // public Vector3 XRPose;

        // public Vector3 Rotation;
    }

    void OnValidate()
    {
        if (m_XRSpace == null)
        {
            m_XRSpace = FindFirstObjectByType<Immersal.XR.XRSpace>();
        }
    }

    void SaveNewBeacon(JSONBeacon beacon)
    {
        Debug.Log("Save New Beacon Called");
        string m_JSONname = "/BeaconList.json";
        if (!File.Exists(Application.persistentDataPath + m_JSONname))
        {
            // If file doesn't exist, create it.
            BeaconList list = new BeaconList();
            JSONBeacon[] beacons = { beacon };
            list.beacons = beacons;
            if (JsonService.SaveData(m_JSONname, list))
            {
                Debug.Log("Beacon saved to new file");
            }
            else
            {
                Debug.LogError("Could not save Beacon!");
            }
        }
        else
        {
            Debug.Log("List exists. Appending");
            // Load previous list
            BeaconList list = JsonService.LoadData<BeaconList>(m_JSONname);
            // Append the new beacon
            JSONBeacon[] newBeaconsList = list.beacons.Append<JSONBeacon>(beacon).ToArray();
            list.beacons = newBeaconsList;
            if (JsonService.SaveData(m_JSONname, list))
            {
                Debug.Log("Beacon saved to existing file");
            }
            else
            {
                Debug.LogError("Could not save Beacon!");
            }
        }
    }

    public void SpawnLoadedBeacons(JSONBeacon beacon)
    {
        GameObject go = Instantiate(m_BeaconPrefab, m_XRSpace.transform);
        // Quaternion rotation = Quaternion.Euler(beacon.Rotation);
        Vector3 xrpose = new Vector3(beacon.xpos, beacon.ypos, beacon.zpos);
        go.transform.SetLocalPositionAndRotation(xrpose, Quaternion.identity);

        Beacon objectBeacon = go.GetComponent<Beacon>();
        objectBeacon.beaconId = beacon.beaconId;
        objectBeacon.roomId = beacon.roomId;
        objectBeacon.XRPose = xrpose;
        // objectBeacon.Rotation = beacon.Rotation;
    }

    public void EditorLoadAnchors()
    {
        BeaconList savefile = JsonUtility.FromJson<BeaconList>(beaconJson.text);

        foreach (JSONBeacon beacon in savefile.beacons)
        {
            SpawnLoadedBeacons(beacon);
        }
    }

    #region Test Functions
    public void ButtonTestSaveBeacon()
    {
        JSONBeacon jSONBeacon = new JSONBeacon();
        jSONBeacon.beaconId = "1";
        jSONBeacon.roomId = "96011";
        jSONBeacon.xpos = 0.0f;
        jSONBeacon.ypos = 0.0f;
        jSONBeacon.zpos = 0.0f;
        SaveNewBeacon(jSONBeacon);
    }
    #endregion
}