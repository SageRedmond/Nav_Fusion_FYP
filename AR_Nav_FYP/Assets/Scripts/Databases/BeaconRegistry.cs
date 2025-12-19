using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

public class BeaconRegistry : MonoBehaviour
{
    private static BeaconRegistry instance;
    public static BeaconRegistry Instance
    {
        get
        {
            if (instance == null)
            {
                var go = new GameObject("BeaconRegistry");
                instance = go.AddComponent<BeaconRegistry>();
                DontDestroyOnLoad(go);
            }
            return instance;
        }
    }

    private List<Beacon> Beacons = new List<Beacon>();

    public void RegisterBeacon(Beacon beacon)
    {
        IEnumerable<Beacon> query = Beacons.Where(b => b.BeaconId == beacon.BeaconId);

        if (query.Count() < 1)
        {
            Beacons.Add(beacon);
            Debug.Log($"Registered beacon: {beacon.BeaconId}");
        }
    }

    public Beacon GetBeaconById(string beaconId) => Beacons.FirstOrDefault(b => b.BeaconId == beaconId);

    public string GetRoomIdByBeaconId(string beaconId) => Beacons.FirstOrDefault(b => b.BeaconId == beaconId).RoomId;

    public bool CheckBeaconRegistryWithId(string beaconId)
    {
        IEnumerable<Beacon> query = Beacons.Where(b => b.BeaconId == beaconId);
        return query.Count() >= 1;
    }
}