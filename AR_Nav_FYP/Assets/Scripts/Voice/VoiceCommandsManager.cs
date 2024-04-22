using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TextSpeech;
using Recognissimo.Components;

public class VoiceCommandsManager : MonoBehaviour
{
    [SerializeField] RoomNameTracker roomNameTracker;

    const string LANG_CODE = "en-US";

    private void Awake() {
        var voiceControl = gameObject.GetComponent<VoiceControl>();

        voiceControl.Commands = new List<VoiceControlCommand> {
            new VoiceControlCommand("Where am I", () => getRoomName()),
            new VoiceControlCommand(@"\b(?:elevator.*button|button.*elevator)\b", () => ElevatorPressButtonPrompt()),
            new VoiceControlCommand("What floor is my destination on", () => getDestinationFloorLevel()),
        };
    }

    private void Start() {
        SetupTextToSpeach(LANG_CODE);

        TextToSpeech.Instance.onStartCallBack = OnSpeakStart;
        TextToSpeech.Instance.onDoneCallback = OnSpeakStop;
    }

    #region Voice Commands
    public void getRoomName() {
        Debug.Log("Your are in " + roomNameTracker.CurrentRoomName);

        StartSpeeking("You are in the " + roomNameTracker.CurrentRoomName);

    }

    public void getDestinationFloorLevel() {
        StartSpeeking("Your destination is on the " + roomNameTracker.DestinationFloorLevelString);
    }

    public void getMyFloorLevel() {
        //StartSpeeking("You are in the " + roomName.CurrentRoomName);
    }

    public void ElevatorPressButtonPrompt() { //
        switch (roomNameTracker.DestinationFloorLevelEnum) {
            case FloorLevel.GroundFloor:
                StartSpeeking("Press Elevator Button 0");
                break;
            case FloorLevel.FirstFloor:
                StartSpeeking("Press Elevator Button 1");
                break;
            case FloorLevel.SecondFloor:
                StartSpeeking("Press Elevator Button 2");
                break;
            default:
                break;
        }
    }
    #endregion

    #region Text to Speech
    void SetupTextToSpeach(string code) {
        TextToSpeech.Instance.Setting(code, 1, 1);
    }
    public void StartSpeeking(string message) {
        TextToSpeech.Instance.StartSpeak(message);
    }

    public void StopSpeeking(string message) {
        TextToSpeech.Instance.StopSpeak();
    }

    void OnSpeakStart() {
        Debug.Log("Started Speaking... ");
    }

    void OnSpeakStop() {
        Debug.Log("Stopped Speaking");
    }
    #endregion
}
