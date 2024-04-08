using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TextSpeech;

public class VoiceCommandsManager : MonoBehaviour
{
    [SerializeField] RoomNameTracker roomName;

    const string LANG_CODE = "en-US";

    private void Start() {
        SetupTextToSpeach(LANG_CODE);

        TextToSpeech.Instance.onStartCallBack = OnSpeakStart;
        TextToSpeech.Instance.onDoneCallback = OnSpeakStop;
    }

    #region Voice Commands
    public void getRoomName() {
        Debug.Log("Your are in " + roomName.CurrentRoomName);

        StartSpeeking("You are in the " + roomName.CurrentRoomName);

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
