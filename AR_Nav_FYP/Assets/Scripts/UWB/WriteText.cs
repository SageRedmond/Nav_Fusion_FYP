using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class WriteText : MonoBehaviour
{
    [SerializeField] public TMP_Text DistanceText;

    // private int counter;
    // Start is called before the first frame update
    void Start()
    {
        DistanceText = GetComponent<TMP_Text>();
    }

    void Update()
    {
        NativeState state = NativeStateManager.State;
        string distanceText = state.distance.ToString("F2");
        writeDistanceText(distanceText);
    }

    void writeDistanceText(string text)
    {
        DistanceText.text = text;
    }
}
