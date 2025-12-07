using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public readonly struct UwbBeaconRangeData
{
    public readonly string beaconId;
    public readonly float distance;
}

public class BeaconRangeTracker : MonoBehaviour
{
    // Should match SetNativeStateCallback typedef in Assets/Plugins/iOS/UwbBeaconRange.h
    private delegate void BeaconRangeCallback(UwbBeaconRangeData newRangeData);

    /* Reverse P/Invoke wrapped method to set state value. iOS is an AOT platform hence the decorator.
       See section on calling managed methods from native code: docs.unity3d.com/Manual/ScriptingRestrictions.html */
    [AOT.MonoPInvokeCallback(typeof(BeaconRangeCallback))]
    private static void SaveBeaconRange(UwbBeaconRangeData newRangeData)
    {
        Debug.Log("" + newRangeData.beaconId);
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