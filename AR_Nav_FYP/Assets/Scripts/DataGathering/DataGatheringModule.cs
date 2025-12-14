using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Globalization;

public class DataGatheringModule : MonoBehaviour
{
    private iDataService JsonService = new JsonDataService();

    private List<XRCoordinates> m_XRCoordinates = new List<XRCoordinates>();
    private List<UnityCoordinates> m_UnityCoordinates = new List<UnityCoordinates>();
    private List<UwbBeaconRange> m_UwbBeaconRange = new List<UwbBeaconRange>();
    private List<TriggeredEventStruct> m_triggeredEvents = new List<TriggeredEventStruct>();

    private bool isGatheringData = false;

    #region Structs
    /// <summary>
    /// Position Relative to XR Immersal Map
    /// </summary>
    public struct XRCoordinates
    {
        public float X;
        public float Y;
        public float Z;
        public string TimeStamp;

        public XRCoordinates(Vector3 coords)
        {
            X = coords.x;
            Y = coords.y;
            Z = coords.z;
            TimeStamp = GetTimeStamp();
        }
    }

    /// <summary>
    /// Position Relative to Unity Origin
    /// </summary>
    public struct UnityCoordinates
    {
        public float X;
        public float Y;
        public float Z;
        public string TimeStamp;

        public UnityCoordinates(Vector3 coords)
        {
            X = coords.x;
            Y = coords.y;
            Z = coords.z;
            TimeStamp = GetTimeStamp();
        }
    }

    /// <summary>
    /// Range too and id of UWB beacon
    /// </summary>
    public struct UwbBeaconRange
    {
        public float Range;
        public string BeaconID;
        public string TimeStamp;

        public UwbBeaconRange(string beaconID, float range)
        {
            Range = range;
            BeaconID = beaconID;
            TimeStamp = GetTimeStamp();
        }
    }

    /// <summary>
    /// Some sort of event like "Localisation Started", "Position Lost", "Destintation Reached", etc
    /// </summary>
    public struct TriggeredEventStruct
    {
        public string Name;
        public string Description;
        public string TimeStamp;

        public TriggeredEventStruct(string name, string description)
        {
            Name = name;
            Description = description;
            TimeStamp = GetTimeStamp();
        }
    }
    #endregion

    #region Adding Functions
    public void AddXRCoordinate(Vector3 pose)
    {
        if (isGatheringData)
        {
            m_XRCoordinates.Add(new XRCoordinates(pose));
        }
    }

    public void AddUnityCoordinate(Vector3 pose)
    {
        if (isGatheringData)
        {
            m_UnityCoordinates.Add(new UnityCoordinates(pose));
        }
    }

    public void AddBeaconRange(string beaconId, float range)
    {
        if (isGatheringData)
        {
            m_UwbBeaconRange.Add(new UwbBeaconRange(beaconId, range));
        }
    }

    public void AddTriggeredEvent(TriggeredEvent eventName, string description)
    {
        if (isGatheringData)
        {
            string name = eventName.GetName();

            m_triggeredEvents.Add(new TriggeredEventStruct(name, description));
        }
    }

    #endregion

    #region Save Functions
    public void SaveData()
    {
        SaveXRCoordinates();
        SaveUnityCoordinates();
        SaveBeaconRange();
        SaveTriggeredEvents();
    }

    private void SaveXRCoordinates()
    {
        string m_JSONname = "/XRCoordinates.json";
        if (JsonService.SaveData(m_JSONname, m_XRCoordinates))
        {
            Debug.Log("XR Coordinates Saved");
        }
        else
        {
            Debug.LogError("Could not save XR Coordinates!");
        }
    }

    private void SaveUnityCoordinates()
    {
        string m_JSONname = "/UnityCoordinates.json";
        if (JsonService.SaveData(m_JSONname, m_UnityCoordinates))
        {
            Debug.Log("Unity Coordinates Saved");
        }
        else
        {
            Debug.LogError("Could not save Unity Coordinates!");
        }
    }

    private void SaveBeaconRange()
    {
        string m_JSONname = "/UwbBeaconRanges.json";
        if (JsonService.SaveData(m_JSONname, m_UwbBeaconRange))
        {
            Debug.Log("UWB Beacon Ranges Saved");
        }
        else
        {
            Debug.LogError("Could not save UWB Beacon Ranges!");
        }
    }
    private void SaveTriggeredEvents()
    {
        string m_JSONname = "/TriggeredEvents.json";
        if (JsonService.SaveData(m_JSONname, m_triggeredEvents))
        {
            Debug.Log("Triggered Events Saved");
        }
        else
        {
            Debug.LogError("Could not save Triggered Events!");
        }
    }

    #endregion

    #region Misc Functions

    private static string GetTimeStamp()
    {
        return DateTime.UtcNow.ToString("o", CultureInfo.InvariantCulture);
    }

    private void ClearAllData()
    {
        m_XRCoordinates.Clear();
        m_UnityCoordinates.Clear();
        m_UwbBeaconRange.Clear();
        m_triggeredEvents.Clear();
    }

    public void PauseGatheringData()
    {
        isGatheringData = false;
    }

    public void ResumeGatheringData()
    {
        isGatheringData = true;
    }

    #endregion

    //TODO Restart experiment button to delete all data after it has been removed

    //TODO Begin new experiment writing to the same file

    #region TestFunctions
    public void TriggerButtonEvent()
    {
        AddTriggeredEvent(TriggeredEvent.ButtonPressed, "Test Button");
    }
    #endregion  
}
