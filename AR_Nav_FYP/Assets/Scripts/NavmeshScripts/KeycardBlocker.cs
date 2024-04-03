using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardBlocker : MonoBehaviour
{
    public string name;
    public bool HasAccess = false;

    private void Awake() {
        name = gameObject.name;
    }
}
