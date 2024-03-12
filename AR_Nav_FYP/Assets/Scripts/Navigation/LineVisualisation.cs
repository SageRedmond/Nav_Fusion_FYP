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
        Transform nextPoint = NavController.path[NavController.currNodeIndex].transform;
        Vector3[] positions = new Vector3[2];

        //Add Camera position
        //Then add nextPoint
        line.SetPositions(positions);
    }
}
