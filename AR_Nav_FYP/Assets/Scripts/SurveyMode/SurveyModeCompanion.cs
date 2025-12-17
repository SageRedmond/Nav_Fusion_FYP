using System;
using UnityEngine;

public class SurveyModeCompanion : MonoBehaviour
{
    [SerializeField] private GameObject surveyModeUI;
    [SerializeField] private TrackedImageManager imageTracker;
    [SerializeField] private BeaconSceneCompanion beaconSceneCompanion;
    public bool surveyMode = false;

    void Start()
    {
        surveyModeUI.SetActive(surveyMode);
    }
    public void ToggleSurveyMode()
    {
        surveyMode = !surveyMode;
        surveyModeUI.SetActive(surveyMode);
    }

    public void SurveyInBeacon()
    {
        beaconSceneCompanion.SaveTrackedImageBeacon(imageTracker.imageMarker.transform);
    }
}
