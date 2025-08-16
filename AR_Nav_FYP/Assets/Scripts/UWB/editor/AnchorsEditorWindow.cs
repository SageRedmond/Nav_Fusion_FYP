using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.Networking;

public class AnchorsEditorWindow : EditorWindow
{

  AnchorStorageManager AnchorManager;
  static string ip = "192.168.0.71:8080";

  UnityWebRequest www;

  [MenuItem("Window/Anchors")]
  public static void ShowWindow()
  {
    GetWindow<AnchorsEditorWindow>("AnchorsEditorWindow");
  }

  void OnGUI()
  {
    Debug.Log("On Gui");
    AnchorManager = GameObject.Find("AnchorManager").GetComponent<AnchorStorageManager>();

    GUILayout.Label("Get Anchors from Database", EditorStyles.boldLabel);

    if (GUILayout.Button("Load"))
    {
      LoadAnchors();
    }
  }

  public void LoadAnchors()
  {
    Debug.Log("Loading Database");
    /* 
      ! NOTE: I've enbled "Allows downloads over HTTP" in player settings for dev builds. 
      ! This allows unsecure HTTP connections!
    */
    www = UnityWebRequest.Get($"http://{ip}/anchors");
    var request = www.SendWebRequest();
    request.completed += finishedLoading;

  }

  private void finishedLoading(AsyncOperation request)
  {
    Debug.Log("Request Finished");
    if (www.result != UnityWebRequest.Result.Success)
    {
      Debug.Log("REST error");
      Debug.Log(www.error);
    }
    else
    {
      Debug.Log("REST Success");
      string requestResult = www.downloadHandler.text;
      Debug.Log(requestResult);

      AnchorManager.EditorLoadAnchors(requestResult);
    }
  }
}