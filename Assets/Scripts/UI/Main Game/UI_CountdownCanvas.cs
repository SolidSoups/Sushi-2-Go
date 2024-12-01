using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI_CountdownCanvas : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private TextMeshProUGUI _countdownText;
  
  public void StartTimer(Action OnFinished = null) => 
    StartCoroutine(StartTimerRoutine(OnFinished));

  private IEnumerator StartTimerRoutine(Action OnFinished = null)
  {
    for (int i = 3; i > 0; i--)
    {
      _countdownText.text = i.ToString(); 
      yield return new WaitForSecondsRealtime(1f);
    }
    OnFinished?.Invoke();
  }
}
