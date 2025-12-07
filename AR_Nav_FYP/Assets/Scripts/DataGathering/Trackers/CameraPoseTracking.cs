using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Immersal.XR;

public class CameraPoseTracker : MonoBehaviour
{
    [SerializeField]
    private DataGatheringModule m_dataGatheringModule;

    [SerializeField]
    private XRSpace m_XRSpace;

    /// <summary>
    /// Time in seconds to capture the camera's poses at
    /// </summary>
    [SerializeField] private float captureRate = 0.3f;

    private Transform cam;

    void Start()
    {
        cam = Camera.main.transform;
        if (m_dataGatheringModule == null)
        {
            m_dataGatheringModule = FindFirstObjectByType<DataGatheringModule>();
        }
        InvokeRepeating(nameof(UpdatePose), 2.0f, captureRate);
    }


    private void UpdatePose()
    {
        Vector3 cameraUnityPose = cam.localPosition;
        Vector3 cameraXRPose = UnityToXRSpace(m_XRSpace.transform, m_XRSpace.InitialPose, cameraUnityPose);

        m_dataGatheringModule.AddUnityCoordinate(cameraUnityPose);
        m_dataGatheringModule.AddXRCoordinate(cameraXRPose);
    }

    private Vector3 UnityToXRSpace(Transform XRSpace, Matrix4x4 XRSpaceOffset, Vector3 pos)
    {
        pos = XRSpaceOffset.inverse.MultiplyPoint(pos);
        Debug.LogAssertion(pos);
        Matrix4x4 m = XRSpace.localToWorldMatrix;
        pos = m.MultiplyPoint(pos);
        Debug.LogAssertion(pos);
        return pos;
    }
}