using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CandyCoded;
using CandyCoded.HapticFeedback;
using UnityEngine.UI;

public class VibrationOnLine : MonoBehaviour
{
    [SerializeField] private NavController NavManager;
    [SerializeField] PolicyManager policyInfo;
    [SerializeField] private Slider VibrationLevel;
    private bool UserSettingVibrate = false;

    private bool shouldVibrate = false;
    private float followingPathRadius = 60.0f;

    public Transform cameraPose;

    private void Start() {
        cameraPose = Camera.main.transform;
    }
    private void Update() {
        if((policyInfo.UserProfile == Profiles.Vision) || UserSettingVibrate) {
            CheckAngle();
            if (shouldVibrate) {
                VibrateAtLevel((int) VibrationLevel.value);
            }
        }
    }

    public void InitiateVibration() {
        shouldVibrate = true;
    }

    public void StopVibration() {
        shouldVibrate = false;
    }

    private void VibrateAtLevel(int level) {
        switch (level) {
            case (1):{
                    HapticFeedback.LightFeedback();
                    break;
                }
            case (2): {
                    HapticFeedback.MediumFeedback();
                    break;
                }
            case (3): {
                    HapticFeedback.HeavyFeedback();
                    break;
                }
            case (4): {
                    Handheld.Vibrate();
                    break;
                }
        }
    }

    private void CheckAngle() {
        Vector3 nextCornerOnPath = NavManager.nextPathPointVector();
        Vector3 userPosition = cameraPose.position;

        Vector3 toNextCorner = nextCornerOnPath - userPosition;
        toNextCorner.y = 0;

        if (Vector3.Dot(toNextCorner.normalized, cameraPose.forward)
            > Mathf.Cos(followingPathRadius * 0.5f * Mathf.Deg2Rad)){

            //Debug.Log("On path");
            shouldVibrate = false;
        }
        else {
            Debug.Log("Off path");
            shouldVibrate = true;
        }
    }

    public void toggleVibration() {
        UserSettingVibrate = !UserSettingVibrate;
    }
}
