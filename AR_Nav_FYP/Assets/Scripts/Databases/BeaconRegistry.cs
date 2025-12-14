using UnityEngine;
using System.Collections.Generic;
using System.Linq;

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
}