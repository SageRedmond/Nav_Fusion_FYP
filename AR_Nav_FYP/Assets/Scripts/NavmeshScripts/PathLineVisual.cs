using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class PathLineVisual : MonoBehaviour
{
    [SerializeField] private NavController navigationController;
    [SerializeField] private LineRenderer line;
    [SerializeField] private Slider navigationYOffset;

    private NavMeshPath path;
    private Vector3[] calculatedPathAndOffset;

    void Start() {
        line = GetComponent<LineRenderer>();

        line.numCapVertices = 1;
        line.numCornerVertices = 1;
    }

    private void Update() {
        path = navigationController.CalculatedPath;
        AddOffsetToPath();
        AddLineOffset();
        SetLineRendererPositions();


    }

    private void AddOffsetToPath() {
        calculatedPathAndOffset = new Vector3[path.corners.Length];
        for (int i = 0; i < path.corners.Length; i++) {
            calculatedPathAndOffset[i] = new Vector3(path.corners[i].x, path.corners[i].y, path.corners[i].z);
        }
    }

    private void AddLineOffset() {
        if (navigationYOffset.value != 0) {
            for (int i = 0; i < calculatedPathAndOffset.Length; i++) {
                calculatedPathAndOffset[i] += new Vector3(0, navigationYOffset.value, 0);
            }
        }
    }

    private void SetLineRendererPositions() {
        line.positionCount = calculatedPathAndOffset.Length;
        line.SetPositions(calculatedPathAndOffset);
    }
}
