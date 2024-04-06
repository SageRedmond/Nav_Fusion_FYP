using Recognissimo.Components;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecognitionListener : MonoBehaviour
{
    [SerializeField] public TMP_Text ListeningText;
    public void OnPartialResult(PartialResult partialResult) {
        Debug.Log($"<color=yellow>{partialResult.partial}</color>");
        ListeningText.text = "";
        ListeningText.text = partialResult.partial;
    }

    public void OnResult(Result result) {
        Debug.Log($"<color=green>{result.text}</color>");
        ListeningText.text = "";
        ListeningText.text = result.text;
    }
}
