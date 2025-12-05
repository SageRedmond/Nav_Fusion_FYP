using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataGatheringModule : MonoBehaviour
{
    private iDataService JsonService = new JsonDataService();
    private TestDataStruct testDataStruct;

    public struct TestDataStruct
    {
        public string Name;
        public string Description;

        public TestDataStruct(string name, string description)
        {
            Name = name;
            Description = description;
        }
    }

    public void saveData()
    {
        Debug.Log("Save Button Pressed");
        string m_JSONname = "/ExperimentFiles/BeaconRanges.json";
        testDataStruct = new TestDataStruct("Sage", "Bleh bleh bleh");

        if (JsonService.SaveData(m_JSONname, testDataStruct))
        {
            Debug.Log("Data Saved");
        }
        else
        {
            Debug.LogError("Could not save file!");
        }

    }

    //TODO Restart experiment button to delete all data after it has been removed

    //TODO Begin new experiment writing to the same file

    //TODO End Trial -> Writes all collected JSON to file
}
