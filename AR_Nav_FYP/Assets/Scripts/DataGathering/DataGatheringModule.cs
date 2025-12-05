using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

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
        string date = DateTime.Now.ToLongTimeString();
        Debug.Log("Save Button Pressed");
        string m_JSONname = "/TestFile.json";
        testDataStruct = new TestDataStruct("Sage", date);

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
