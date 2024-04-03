using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Immersal.XR;

public class NavController : MonoBehaviour
{
    private XRSpace m_XRSpace = null;
    public Vector3 TargetPosition { get; set; } = Vector3.zero;
    public NavMeshPath CalculatedPath { get; private set; }

    public Transform cameraPose;

    private bool managerInitialized = false;

    [Header("Path Visual")]
    [SerializeField]
    private float ArrivedDistanceThreshold = 1.0f;

    [SerializeField]
    private float pathWidth = 0.3f;

    [SerializeField]
    private float heightOffset = 0.5f;

    private void Start() {
        InitializeNavManager();
        CalculatedPath = new NavMeshPath();
    }

    private void Update() {
        if (managerInitialized && TargetPosition != Vector3.zero) {
            MakePath();
        }
    }

    private void MakePath() {

        List<Vector3> corners;

        Vector3 startPosition = cameraPose.position;
        Vector3 targetPosition = TargetPosition;

        startPosition = XRSpaceToUnity(m_XRSpace.transform, m_XRSpace.InitialPose, startPosition);
        targetPosition = XRSpaceToUnity(m_XRSpace.transform, m_XRSpace.InitialPose, targetPosition);

        Vector3 delta = targetPosition - startPosition;
        float distanceToTarget = new Vector3(delta.x, delta.y, delta.z).magnitude;

        if (distanceToTarget < ArrivedDistanceThreshold) {
            Debug.Log("Arrived");
            return;
        }
        corners = FindPathNavMesh(startPosition, targetPosition);

        //if (NavMesh.CalculatePath(cameraPose.transform.position, TargetPosition, NavMesh.AllAreas, CalculatedPath)) {
        //    Debug.Log("Path Found");

        //}
        //else {
        //    Debug.Log("Path Not Found");
        //}
    }

    private List<Vector3> FindPathNavMesh(Vector3 startPosition, Vector3 targetPosition) {
        NavMeshPath path = new NavMeshPath();
        List<Vector3> collapsedCorners = new List<Vector3>();

        if (NavMesh.CalculatePath(startPosition, targetPosition, NavMesh.AllAreas, path)) {
            List<Vector3> corners = new List<Vector3>(path.corners);

            for (int i = 0; i < corners.Count; i++) {
                //corners[i] = corners[i] + new Vector3(0f, m_heightOffset, 0f);
                corners[i] = UnityToXRSpace(m_XRSpace.transform, m_XRSpace.InitialPose, corners[i]);
            }

            for (int i = 0; i < corners.Count - 1; i++) {
                Vector3 currentPoint = corners[i];
                Vector3 nextPoint = corners[i + 1];
                float threshold = 0.75f;

                if (Vector3.Distance(currentPoint, nextPoint) > threshold) {
                    collapsedCorners.Add(currentPoint);
                }
            }

            collapsedCorners.Add(corners[corners.Count - 1]);
        }

        CalculatedPath = path;
        return collapsedCorners;
    }

    public Vector3 nextPathPointVector() {
        //return CalculatedPath.corners[0];
        try {
            return CalculatedPath.corners[0];
        }
        catch (System.IndexOutOfRangeException) {
            return Vector3.zero;
        }
    }

    private void InitializeNavManager() {
        if (m_XRSpace == null) {
            m_XRSpace = FindObjectOfType<XRSpace>();

            if (m_XRSpace == null) {
                Debug.LogWarning("NavigationManager: No XR Space found in scene, ensure one exists.");
                return;
            }
        }

        cameraPose = Camera.main.transform;

        managerInitialized = true;
    }

    private Vector3 XRSpaceToUnity(Transform XRSpace, Matrix4x4 XRSpaceOffset, Vector3 pos) {
        Matrix4x4 m = XRSpace.worldToLocalMatrix;
        pos = m.MultiplyPoint(pos);
        pos = XRSpaceOffset.MultiplyPoint(pos);
        return pos;
    }

    private Vector3 XRSpaceToUnity(Transform XRSpace, Vector3 pos) {
        pos = XRSpaceToUnity(XRSpace, Matrix4x4.identity, pos);
        return pos;
    }

    private Vector3 UnityToXRSpace(Transform XRSpace, Matrix4x4 XRSpaceOffset, Vector3 pos) {
        pos = XRSpaceOffset.inverse.MultiplyPoint(pos);
        Matrix4x4 m = XRSpace.localToWorldMatrix;
        pos = m.MultiplyPoint(pos);
        return pos;
    }

    private Vector3 UnityToXRSpace(Transform XRSpace, Vector3 pos) {
        pos = UnityToXRSpace(XRSpace, Matrix4x4.identity, pos);
        return pos;
    }
}
