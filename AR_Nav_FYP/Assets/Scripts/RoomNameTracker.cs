using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using TMPro;

public class RoomNameTracker : MonoBehaviour
{
    //public Localizer ImmersalLocalizer;

    [SerializeField] public TMP_Text UI_RoomText;

    private Dictionary<int, string> RoomIDs = new Dictionary<int, string>();

    public string CurrentRoomName = "Unknown?";
    public int CurrentRoomID = 0;

    private void Start() {
        //ImmersalLocalizer = FindObjectOfType<Localizer>();
        //ImmersalLocalizer.OnSuccessfulLocalizations.AddListener(ShowRoomName);
        
        AddRoomIDs();
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
}
