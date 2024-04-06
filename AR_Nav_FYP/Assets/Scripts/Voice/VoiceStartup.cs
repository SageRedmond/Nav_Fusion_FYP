using Recognissimo.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceStartup : MonoBehaviour
{
    [SerializeField] SpeechRecognizer speechRecognizer;

    private void Start() {
        foreach (var device in Microphone.devices) {
            Debug.Log("Name: " + device);
        }
        //speechRecognizer.StartProcessing();
    }
}
