using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbleBlocker : MonoBehaviour
{
    public string name;
    private void Awake() {
        name = gameObject.name;
    }
}
