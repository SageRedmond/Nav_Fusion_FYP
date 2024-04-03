using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeycardAccessManager : MonoBehaviour
{
    public List<KeycardBlocker> KeycardAccessBlockers = new List<KeycardBlocker>();

    public Dictionary<KeycardBlocker, bool> CanAccessDoor = new Dictionary<KeycardBlocker, bool>();

    private void Start() {
        KeycardBlocker[] Keyblock = FindObjectsOfType<KeycardBlocker>();
        KeycardAccessBlockers.AddRange(Keyblock);

        foreach(KeycardBlocker keyblock in Keyblock) {
            CanAccessDoor.Add(keyblock, keyblock.HasAccess);
        }

        ApplyKeycardAccess();
    }

    public void SwitchOn() {
        foreach (KeycardBlocker block in KeycardAccessBlockers) {
            block.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void SwitchOff() {
        foreach (KeycardBlocker block in KeycardAccessBlockers) {
            block.transform.GetChild(0).gameObject.SetActive(false);
        }
    }

    public void ApplyKeycardAccess() {
        foreach(KeyValuePair<KeycardBlocker, bool> blocker in CanAccessDoor) {
            if (blocker.Value) {
                blocker.Key.transform.GetChild(0).gameObject.SetActive(false);
            }
            else {
                blocker.Key.transform.GetChild(0).gameObject.SetActive(true);
            }
        }
    }
}
