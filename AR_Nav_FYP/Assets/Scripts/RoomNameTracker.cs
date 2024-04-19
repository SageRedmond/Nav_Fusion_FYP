using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;
using Immersal.XR;

public class RoomNameTracker : MonoBehaviour
{
    //public Localizer ImmersalLocalizer;

    [SerializeField] public TMP_Text UI_RoomText;

    private Dictionary<int, string> RoomIDs = new Dictionary<int, string>();

    //private List<XRMap> maps = new List<XRMap>();

    public string CurrentRoomName = "Unknown?";
    public int CurrentRoomID = 0;

    private void Start() {
        //ImmersalLocalizer = FindObjectOfType<Localizer>();
        //ImmersalLocalizer.OnSuccessfulLocalizations.AddListener(ShowRoomName);
        GetMaps();
        //AddRoomIDs();


    }

    public void ShowRoomName(int[] mapIDs) {
        Debug.Log("Room Name function invoked");
        Debug.Log(string.Join(",",mapIDs));
        string roomName;
        try {
            roomName = RoomIDs[mapIDs[0]];
        } catch(KeyNotFoundException) {
            roomName = "Unknown";
        }
        UI_RoomText.text = roomName;

        CurrentRoomName = roomName;
        Debug.Log(CurrentRoomName);
    }

    public void AddRoomIDs() {
        RoomIDs.Add(96011, "Upstairs Lab");
        RoomIDs.Add(96096, "2nd Floor Hallway");
        RoomIDs.Add(96206, "Engineering Staff Offices");
        RoomIDs.Add(97734, "1rst Floor Elevator Landing");
        RoomIDs.Add(97972, "1rst Floor Stairs Landing");
    }
    
    public void GetMaps() {
        XRMap[] allMaps = FindObjectsOfType<XRMap>();

        foreach( XRMap map in allMaps) {
            string mapName = string.Concat(map.mapName.Select(x => char.IsUpper(x) ? " " + x : x.ToString())).TrimStart(' ');
            if(map.mapId == 98492) { //I don't like the map name I gave it in Immersal. I could probably change it on the immersal portal but ehh
                RoomIDs.Add(map.mapId, "Foyer");
            }
            else {
                RoomIDs.Add(map.mapId, mapName);
                //Debug.Log(str);
            }
        }
    }
}
