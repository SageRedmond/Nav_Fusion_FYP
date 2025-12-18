using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class WriteClosestBeaconText : MonoBehaviour
{
    [SerializeField] public TMP_Text closestBeaconText;

    // Start is called before the first frame update
    void Start()
    {
        closestBeaconText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        string distanceText = BeaconRangeTracker.ClosestBeaconId; ;
        ClosestBeaconText(distanceText);
    }

    void ClosestBeaconText(string text)
    {
        closestBeaconText.text = "Closest Beacon ID: \n" + text;
    }
}