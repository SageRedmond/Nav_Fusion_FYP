using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using System.IO;
using System;

public class JsonDataService : iDataService
{
    bool iDataService.SaveData<T>(string RelativePath, T Data)
    {
        string path = Application.persistentDataPath + RelativePath;
        Debug.Log(path);
        //Debug.Log(path);
        try
        {
            if (File.Exists(path))
            {
                Debug.Log("Data Exists. Deleting Old File and Writing a new one!");
                File.Delete(path);
            }
            else
            {
                Debug.Log("Creating file for the first time!");
            }
            using FileStream stream = File.Create(path);
            stream.Close();
            File.WriteAllText(path, JsonConvert.SerializeObject(Data));
            return true;
        }
        catch (Exception e)
        {
            Debug.Log($"Unable to save data due to: {e.Message} {e.StackTrace}");
            return false;
        }
    }
}