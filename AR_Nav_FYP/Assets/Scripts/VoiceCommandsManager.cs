using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class VoiceCommandsManager : MonoBehaviour
{
    [SerializeField] RoomNameTracker roomName;

    //[SerializeField] public TMP_Text ListeningText;

    public void getRoomName() {
        Debug.Log(roomName.CurrentRoomName);
    }
}
