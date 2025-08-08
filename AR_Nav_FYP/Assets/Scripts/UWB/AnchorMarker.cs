using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Should be attached to the prefab used by AnchorStorageManager
public class AnchorMarker : MonoBehaviour
{
    // public String AnchorID;

    // When one of these little guys are instantiated, they auto add themselfs to the content list
    private void StoreContent()
    {
        if (!AnchorStorageManager.Instance.contentList.Contains(this))
        {
            AnchorStorageManager.Instance.contentList.Add(this);
        }
        AnchorStorageManager.Instance.SaveAnchors();
    }

    public void RemoveContent()
    {
        if (AnchorStorageManager.Instance.contentList.Contains(this))
        {
            AnchorStorageManager.Instance.contentList.Remove(this);
        }
        AnchorStorageManager.Instance.SaveAnchors();
        Destroy(gameObject);
    }
}
