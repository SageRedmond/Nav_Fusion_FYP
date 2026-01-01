using UnityEngine;
using Immersal;
using Immersal.XR;

class TriggeredEventTracker : MonoBehaviour
{
    [SerializeField]
    private DataGatheringModule m_dataGatheringModule;

    private ImmersalSDK m_Sdk;

    private Localizer m_Localizer;

    private NavController m_NavController;

    void Start()
    {
        if (m_dataGatheringModule == null)
        {
            m_dataGatheringModule = FindFirstObjectByType<DataGatheringModule>();
        }

        m_Sdk = ImmersalSDK.Instance;

        if (m_Localizer == null)
        {
            m_Localizer = FindFirstObjectByType<Localizer>();
        }
        if (m_NavController == null)
        {
            m_NavController = FindFirstObjectByType<NavController>();
        }

        m_Sdk.OnInitializationComplete.AddListener(LogImmersalInitialised);

        m_Localizer.OnFirstSuccessfulLocalization.AddListener(LogFirstSuccessfulLocalisation);

        m_NavController.atDestination.AddListener(LogAtDestination);
    }

    #region Logs
    private void LogImmersalInitialised()
    {
        m_dataGatheringModule.AddTriggeredEvent(TriggeredEvent.ImmersalInitialised, "");
    }

    private void LogFirstSuccessfulLocalisation()
    {
        m_dataGatheringModule.AddTriggeredEvent(TriggeredEvent.FirstSuccessfulLocalization, "");
    }

    private void LogAtDestination()
    {
        m_dataGatheringModule.AddTriggeredEvent(TriggeredEvent.DestinationReached, "");
    }
    #endregion
}