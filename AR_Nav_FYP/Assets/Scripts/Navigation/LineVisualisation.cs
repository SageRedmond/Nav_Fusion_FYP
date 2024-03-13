using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using AR_NAV_FYP.Navigation;

public class LineVisualisation : MonoBehaviour
{
    [SerializeField]
    private NavigationController NavController;

    [SerializeField]
    private LineRenderer line;

    private void Update() {
        //Transform nextPoint = NavController.path[NavController.currNodeIndex].transform;

        int currentIndex = NavController.currNodeIndex;
        List<Node> path = NavController.path;

        Vector3[] positions = new Vector3[path.Count];

        //Add Camera position

        int i = 0;
        foreach(Node node in path) {
            positions[i] = node.position;
            positions[i].y -= 1.0f;
            i++;
        }

        line.positionCount = positions.Length;
        line.SetPositions(positions);
    }
}
