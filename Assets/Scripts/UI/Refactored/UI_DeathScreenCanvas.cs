using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class UI_DeathScreenCanvas : MonoBehaviour
{
  [Header("Settings")]
  [SerializeField] private float _enableDelay = 2f;
  
  [Header("References")]
  [SerializeField] private Canvas _canvas;
  [SerializeField] private TextMeshProUGUI _scoreText;
  [SerializeField] private TextMeshProUGUI _highScoreText;
  
  public void SetScoreText(int score) => _scoreText.text = score.ToString();
  public void SetHighScoreText(int score) => _highScoreText.text = score.ToString();

  private void Awake() => _canvas.enabled = false;

  public void StartDelayedEnable() => StartCoroutine(DelayedEnable());

  IEnumerator DelayedEnable()
  {
    yield return new WaitForSeconds(_enableDelay);
    _canvas.enabled = true;
  }
}
