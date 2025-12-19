using UnityEngine;

[System.Serializable]
public class Beacon : MonoBehaviour
{
    public string beaconId;
    public string roomId;
    public Vector3 XRPose;
    // public Vector3 Rotation;

    public string BeaconId => beaconId;
    public string RoomId => roomId;

    void Start()
    {
        BeaconRegistry.Instance.RegisterBeacon(this);
    }
}