using UnityEngine;
using Immersal;
using Immersal.XR;
using Immersal.Samples;
/// <summary>
/// Controls when Immersal should be localising,
/// Based on available information.
/// Manages Point Cloud activation for floor levels
/// </summary>
public class LocalisationManager : MonoBehaviour
{
    [SerializeField]
    private LocalizerSettingsPanel localizerSettingsPanel;
    [SerializeField]
    private Localizer immersalLocalizer;

    // [SerializeField]
    // private DataGatheringModule m_dataGatheringModule;

    private int currentFloor = 100;

    [SerializeField] private float updateRate = 0.3f;
    [SerializeField] private float callDelay = 1.0f;

    private bool foundABeacon = false;

    void Start()
    {
        currentFloor = 100;  // So that after first localisation, the MonitorFloorActivation method will turn on the other floors
        if (localizerSettingsPanel == null)
        {
            localizerSettingsPanel = FindFirstObjectByType<LocalizerSettingsPanel>();
        }
        if (immersalLocalizer == null)
        {
            immersalLocalizer = FindFirstObjectByType<Immersal.XR.Localizer>();
        }
        // if (m_dataGatheringModule == null)
        // {
        //     m_dataGatheringModule = FindFirstObjectByType<DataGatheringModule>();
        // }

        // Once localisation has occured with the beacon-room based method, 
        // this will switch it to the floor based method
        immersalLocalizer.OnFirstSuccessfulLocalization.AddListener(SwitchToFloorLocalisation);

        // Don't start localising until a beacon has been found and the room has been set active
        // localizerSettingsPanel.Pause();
        StartBeaconRoomMonitoring();
    }

    public void SwitchToBeaconRoomLocalisation()
    {
        CancelInvoke();
        localizerSettingsPanel.Pause();
        LocalisationState.SetState(LocalisationType.BeaconRoomBased);
        localizerSettingsPanel.Resume();
        StartBeaconRoomMonitoring();
    }

    public void StartBeaconRoomMonitoring()
    {
        InvokeRepeating(nameof(MonitorBeaconRoom), callDelay, updateRate);
        Debug.Log("Started Monitoring Beacon Room");
    }

    public void MonitorBeaconRoom()
    {
        if (LocalisationState.State == LocalisationType.BeaconRoomBased)
        {
            // Check if closest beacon is registered (which should only happen if the beacon is deployed)
            string closestsBeaconId = BeaconRangeTracker.ClosestBeaconId;
            if (BeaconRegistry.Instance.CheckBeaconRegistryWithId(closestsBeaconId))
            {
                RoomsRegistry.Instance.EnableOnlyRoomWithID(BeaconRegistry.Instance.GetRoomIdByBeaconId(closestsBeaconId));
                if (!foundABeacon)
                {
                    Debug.Log("Beacon Room: Found a Beacon");
                    // A beacon has been found and the room it is in has been activated.
                    // We now turn on the localizer 
                    foundABeacon = true;
                    localizerSettingsPanel.StartLocalizing();
                }
            }
            // Debug.Log("Beacon Room: beacon not registered");
        }
        else
        {
            Debug.LogError("Localisation state and monitoring do not match");
        }
    }

    public void SwitchToFloorLocalisation()
    {
        CancelInvoke();
        localizerSettingsPanel.Pause();
        LocalisationState.SetState(LocalisationType.FloorBased);
        localizerSettingsPanel.Resume();
        StartFloorMonitoring();
    }

    private void StartFloorMonitoring()
    {
        InvokeRepeating(nameof(MonitorFloorActivation), callDelay, updateRate);
        Debug.Log("Started Monitoring Floor Level");
    }

    private void MonitorFloorActivation()
    {
        if (LocalisationState.State == LocalisationType.FloorBased)
        {
            string closestsBeaconId = BeaconRangeTracker.ClosestBeaconId;
            if (BeaconRegistry.Instance.CheckBeaconRegistryWithId(closestsBeaconId))
            {
                int floorOfClosestBeacon = RoomsRegistry.Instance.GetFloorNumberByRoomId(BeaconRegistry.Instance.GetRoomIdByBeaconId(BeaconRangeTracker.ClosestBeaconId)); ;
                if (floorOfClosestBeacon != currentFloor)
                {
                    currentFloor = floorOfClosestBeacon;
                    RoomsRegistry.Instance.EnableOnlyRoomsWithFloorNumber(currentFloor);
                }
            }

        }
        else
        {
            Debug.LogError("Localisation state and monitoring do not match");
        }
    }
}