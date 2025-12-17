using UnityEngine;
using Immersal;
using Immersal.XR;

class LocalizationConfidenceTracker : MonoBehaviour
{
    [SerializeField]
    private DataGatheringModule m_dataGatheringModule;

    private ImmersalSDK m_Sdk;

    /// <summary>
    /// Time in seconds to capture the camera's poses at
    /// </summary>
    [SerializeField] private float captureRate = 0.3f;

    void Start()
    {
        if (m_dataGatheringModule == null)
        {
            m_dataGatheringModule = FindFirstObjectByType<DataGatheringModule>();
        }
        m_Sdk = ImmersalSDK.Instance;

        InvokeRepeating(nameof(UpdateConfidence), 2.0f, captureRate);
    }

    private void UpdateConfidence()
    {
        if (m_Sdk == null)
            return;

        int q = m_Sdk.TrackingStatus?.TrackingQuality ?? 0;
    }
}