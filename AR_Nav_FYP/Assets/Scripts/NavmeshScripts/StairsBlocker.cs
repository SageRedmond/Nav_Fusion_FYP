using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairsBlocker : MonoBehaviour
{
    public string name;

    private void Awake() {
        name = gameObject.name;
    }
}
