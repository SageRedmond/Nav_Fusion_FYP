using Recognissimo.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceStartup : MonoBehaviour
{
    private void Awake() {
        // Create components.
        var voiceControl = gameObject.AddComponent<VoiceControl>();
        var languageModelProvider = gameObject.AddComponent<StreamingAssetsLanguageModelProvider>();
        var speechSource = gameObject.AddComponent<MicrophoneSpeechSource>();
        // Setup speech source and language model provider as in the previous example.
        languageModelProvider.language = SystemLanguage.English;
        // Set paths to language models.
        languageModelProvider.languageModels = new List<StreamingAssetsLanguageModel>
        {
            new() {language = SystemLanguage.English, path = "LanguageModels/en-US"},
        };

        speechSource.DeviceName = null;
        speechSource.TimeSensitivity = 0.25f;

        // Bind speech processor dependencies.
        voiceControl.LanguageModelProvider = languageModelProvider;
        voiceControl.SpeechSource = speechSource;
        // Setup voice control

        voiceControl.AsapMode = true;
        //voiceControl.Commands = new List<VoiceControlCommand>
        //{
        //    new VoiceControlCommand("start|begin", () => Debug.Log("Start")),
        //    new VoiceControlCommand("stop", HandleStop)
        //};
        voiceControl.StartProcessing();
    }
    private void HandleStop() {
        Debug.Log("Stop");
    }

    private void Start() {
        foreach (var device in Microphone.devices) {
            Debug.Log("Name: " + device);
        }
    }
}
