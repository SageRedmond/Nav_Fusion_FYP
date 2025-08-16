using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Should be attached to the prefab used by AnchorStorageManager
public class AnchorMarker : MonoBehaviour
{
    public String AnchorID;

    // public void StoreContent()
    // {
    //     Debug.Log("Storing Anchor");

    //     NativeState state = NativeStateManager.State;
    //     AnchorID = state.beaconId;

    //     if (!AnchorStorageManager.Instance.sceneAnchorList.Contains(this))
    //     {
    //         AnchorStorageManager.Instance.sceneAnchorList.Add(this);
    //     }
    //     // AnchorStorageManager.Instance.SaveAnchors();
    // }

    // public void RemoveContent()
    // {
    //     if (AnchorStorageManager.Instance.sceneAnchorList.Contains(this))
    //     {
    //         AnchorStorageManager.Instance.sceneAnchorList.Remove(this);
    //     }
    //     // AnchorStorageManager.Instance.SaveAnchors();
    //     Destroy(gameObject);
    // }
}
