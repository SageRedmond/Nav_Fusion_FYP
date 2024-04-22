using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goal : MonoBehaviour
{
    public Vector3 position;
    public FloorLevel Level = FloorLevel.GroundFloor;

    private void Awake() {
        position = transform.position;

        if(position.y >= -1.26) {
            Level = FloorLevel.SecondFloor;
        }
        else if (position.y <= -5.6) {
            Level = FloorLevel.GroundFloor;
        }
        else {
            Level = FloorLevel.FirstFloor;
        }
    }
}

public enum FloorLevel {
    SecondFloor,
    FirstFloor,
    GroundFloor
}