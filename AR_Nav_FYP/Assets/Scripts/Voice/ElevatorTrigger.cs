using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Immersal.Samples;
using Immersal.XR;

public class ElevatorTrigger : MonoBehaviour
{
    [SerializeField] VoiceCommandsManager SpeechCommands;
    [SerializeField] Collider player; //this is the variable that will hold the colliding object
    [SerializeField] LocalizerSettingsPanel Localizer;
    private bool triggered = false; //If we only want to detect the first time it's triggered

    private void OnTriggerEnter(Collider other) {
        Debug.Log("Elevator Trigger Zone Entered");
        if (other != player || triggered) //The colliding object isn't our object
        {
            return; //don't do anything if it's not our target
        }
        triggered = true;
        SpeechCommands.ElevatorPressButtonPrompt();
        Localizer.Pause();
    }

    private void OnTriggerExit(Collider other) {
        Debug.Log("Elevator Trigger Zone Left");
        triggered = false;
        Localizer.Resume();
    }
}
