using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using CandyCoded.HapticFeedback;
using Immersal.XR;

class FeedbackProvider : MonoBehaviour
{
    [SerializeField] private Slider VibrationLevel;

    private Localizer m_Localizer;

    private NavController m_NavController;

    void Start()
    {
        if (m_Localizer == null)
        {
            m_Localizer = FindFirstObjectByType<Localizer>();
        }
        if (m_NavController == null)
        {
            m_NavController = FindFirstObjectByType<NavController>();
        }

        m_Localizer.OnFirstSuccessfulLocalization.AddListener(VibrateForPositionFound);

        m_NavController.atDestination.AddListener(VibrateForDestinationEvent);
    }

    private void VibrateAtLevel(int level)
    {
        switch (level)
        {
            case (1):
                {
                    HapticFeedback.LightFeedback();
                    break;
                }
            case (2):
                {
                    HapticFeedback.MediumFeedback();
                    break;
                }
            case (3):
                {
                    HapticFeedback.HeavyFeedback();
                    break;
                }
            case (4):
                {
                    Handheld.Vibrate();
                    break;
                }
        }
    }

    private int VibrationCounter = 30;
    void VibrateForDestinationEvent()
    {
        VibrationCounter = 30;
        InvokeRepeating(nameof(EventVibration), 0, 0.1f);
    }

    void VibrateForPositionFound()
    {
        VibrationCounter = 18;
        InvokeRepeating(nameof(EventVibration), 0, 0.1f);
    }

    void EventVibration()
    {
        VibrationCounter--;
        // VibrateAtLevel((int)VibrationLevel.value);
        Handheld.Vibrate();
        if (VibrationCounter <= 0) CancelInvoke(nameof(EventVibration));
    }

    public void TestButtonVibration()
    {
        // VibrateAtLevel((int)VibrationLevel.value);
        VibrateForDestinationEvent();
    }
}