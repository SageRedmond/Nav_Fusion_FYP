using System;
using UnityEngine;
using Immersal.Samples;

public class SurveyModeCompanion : MonoBehaviour
{
    [SerializeField] private GameObject surveyModeUI;
    [SerializeField] private TrackedImageManager imageTracker;
    [SerializeField] private BeaconSceneCompanion beaconSceneCompanion;
    [SerializeField] private LocalisationManager localisationManager;
    [SerializeField] private LocalizerSettingsPanel localizerSettingsPanel;

    public bool surveyMode = false;

    void Start()
    {
        if (beaconSceneCompanion == null)
        {
            beaconSceneCompanion = FindFirstObjectByType<BeaconSceneCompanion>();
        }
        if (localisationManager == null)
        {
            localisationManager = FindFirstObjectByType<LocalisationManager>();
        }
        if (localizerSettingsPanel == null)
        {
            localizerSettingsPanel = FindFirstObjectByType<LocalizerSettingsPanel>();
        }
        surveyModeUI.SetActive(surveyMode);
    }
    public void ToggleSurveyMode()
    {
        surveyMode = !surveyMode;
        surveyModeUI.SetActive(surveyMode);
        if (surveyMode)
        {
            RoomsRegistry.Instance.EnableAllRooms();
            localisationManager.SwitchToMapLocalisation();
            localizerSettingsPanel.StartLocalizing();
        }
        else
        {
            RoomsRegistry.Instance.DisableAllRooms();
            localisationManager.SwitchToBeaconRoomLocalisation();
            localizerSettingsPanel.StopLocalizing();
        }
    }

    public void SurveyInBeacon()
    {
        beaconSceneCompanion.SaveTrackedImageBeacon(imageTracker.imageMarker.transform);
    }
}
