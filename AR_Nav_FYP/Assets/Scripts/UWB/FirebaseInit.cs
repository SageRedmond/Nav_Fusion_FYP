using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Firebase;
using Firebase.Extensions;

public class FirebaseInit : MonoBehaviour
{
  public UnityEvent OnFirebaseInit = new UnityEvent();

  private void Start()
  {
    FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
    {
      if (task.Exception != null)
      {
        Debug.LogError($"Failed to init Firebase with {task.Exception}");
        return;
      }

      OnFirebaseInit.Invoke();
    });
  }
}
