using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbleBlockerManager : MonoBehaviour
{
    public List<AbleBlocker> AbleBlockers = new List<AbleBlocker>();

    private void Start() {
        AbleBlocker[] AbleBlock = FindObjectsOfType<AbleBlocker>();
        AbleBlockers.AddRange(AbleBlock);
    }

    public void SwitchOn() {
        foreach (AbleBlocker block in AbleBlockers) {
            block.gameObject.SetActive(true);
        }
    }

    public void SwitchOff() {
        foreach (AbleBlocker block in AbleBlockers) {
            block.gameObject.SetActive(false);
        }
    }
}
