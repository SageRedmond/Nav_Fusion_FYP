using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class ClickHandler : MonoBehaviour
{
    public UnityEvent upEvent;
    public UnityEvent downEvent;

    private void OnMouseUp() {
        upEvent?.Invoke();
    }

    private void OnMouseDown() {
        downEvent?.Invoke();
    }
}
