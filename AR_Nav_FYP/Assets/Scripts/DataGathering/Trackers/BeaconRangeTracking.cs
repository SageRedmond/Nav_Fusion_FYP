using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class BeaconRangeTracker : MonoBehaviour
{
    [SerializeField]
    private DataGatheringModule m_dataGatheringModule;

    private static BeaconRangeTracker s_instance;

    void Awake()
    {
        // Set the static reference to this instance, so that m_dataGatheringModule can be refrenced
        s_instance = this;
    }

    void Start()
    {
        if (m_dataGatheringModule == null)
        {
            m_dataGatheringModule = FindFirstObjectByType<DataGatheringModule>();
        }
    }

    // Should match BeaconRangeCallback typedef in Assets/Plugins/iOS/UwbBeaconRange.h
    private delegate void BeaconRangeCallback(UwbBeaconRangeData newRangeData);

    /* Reverse P/Invoke wrapped method to set range value. iOS is an AOT platform hence the decorator.
       See section on calling managed methods from native code: docs.unity3d.com/Manual/ScriptingRestrictions.html */
    [AOT.MonoPInvokeCallback(typeof(BeaconRangeCallback))]
    private static void SaveBeaconRange(UwbBeaconRangeData newRangeData)
    {
        Debug.Log("" + newRangeData.beaconId);
        if (s_instance != null && s_instance.m_dataGatheringModule != null)
        {
            s_instance.m_dataGatheringModule.AddBeaconRange(newRangeData.beaconId, newRangeData.distance);
        }
        else
        {
            Debug.LogError("BeaconRangeTracker instance or DataGatheringModule is null!");
            return;
        }
    }

    /* Imported from Plugins/iOS/UwbBeaconRange.m to pass instance of
       SetNativeStateCallback to C. See section on using delegates: docs.unity3d.com/Manual/PluginsForIOS.html */
    [DllImport("__Internal")]
    private static extern void OnSendBeaconRange(BeaconRangeCallback callback);

    static BeaconRangeTracker()
    {
#if !UNITY_EDITOR
        OnSendBeaconRange(SaveBeaconRange);
#endif
    }
}