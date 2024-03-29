using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsBlockersManager : MonoBehaviour
{
    public List<StairsBlocker> StairwayBlockers = new List<StairsBlocker>();

    private void Start() {
        StairsBlocker[] Stairblock = FindObjectsOfType<StairsBlocker>();
        StairwayBlockers.AddRange(Stairblock);
    }

    public void SwitchOn() {
        foreach(StairsBlocker block in StairwayBlockers) {
            block.transform.GetChild(0).gameObject.SetActive(true);
        }
    }

    public void SwitchOff() {
        foreach (StairsBlocker block in StairwayBlockers) {
            block.transform.GetChild(0).gameObject.SetActive(false);
        }
    }
}
