using UnityEngine;
using Immersal;
using Immersal.XR;
using TMPro;
using UnityEngine.UI;
using System.Diagnostics;

class TriggeredEventTracker : MonoBehaviour
{
    [SerializeField]
    private DataGatheringModule m_dataGatheringModule;

    private ImmersalSDK m_Sdk;

    private Localizer m_Localizer;

    private NavController m_NavController;

    [SerializeField]
    private Button m_WaypointButton;

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

        if (m_WaypointButton)
        {
            m_WaypointButton.onClick.AddListener(WaypointButtonStart);
        }
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

    private void LogAtWaypoint()
    {
        m_dataGatheringModule.AddTriggeredEvent(TriggeredEvent.AtWaypoint, "");
    }

    private void LogLeavingWaypoint()
    {
        m_dataGatheringModule.AddTriggeredEvent(TriggeredEvent.LeavingWaypoint, "");
    }

    private void WaypointButtonStart()
    {
        UnityEngine.Debug.Log("Waypoint pressed!");
        StartCoroutine(TimeAtWaypoints());
    }


    System.Collections.IEnumerator TimeAtWaypoints()
    {
        m_WaypointButton.gameObject.SetActive(false);
        LogAtWaypoint();

        yield return new WaitForSeconds(20);

        LogLeavingWaypoint();
        m_WaypointButton.gameObject.SetActive(true);
    }
    #endregion
}