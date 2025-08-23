using System;
// using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
public class LocalisationTimer : MonoBehaviour
{
  [SerializeField] public TMP_Text LocalisationTimeText;
  [SerializeField] public GameObject LocalisationButton;

  private DateTime m_startTime;
  private bool m_isTiming = false;

  void Start()
  {
    LocalisationTimeText = GetComponent<TMP_Text>();
  }

  public void startTiming()
  {
    m_startTime = DateTime.Now;
    m_isTiming = true;
    LocalisationButton.SetActive(false);
  }

  public void stopTiming()
  {
    if (m_isTiming)
    {
      m_isTiming = false;

      DateTime end = DateTime.Now;

      TimeSpan timeDiff = end - m_startTime;

      string timeText = (Convert.ToInt32(timeDiff.TotalMilliseconds)).ToString();
      writeLocalisationTimeText(timeText);
      
      LocalisationButton.SetActive(true);
    }
  }
  void writeLocalisationTimeText(string text)
  {
    LocalisationTimeText.text = text;
  }
}
