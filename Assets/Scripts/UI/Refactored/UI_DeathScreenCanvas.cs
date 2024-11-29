using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UI_DeathScreenCanvas : MonoBehaviour
{
  [Header("Settings")]
  [SerializeField] private float _enableDelay = 2f;
  
  [Header("References")]
  [SerializeField] private Canvas _canvas;
  [SerializeField] private TextMeshProUGUI _scoreText;
  [SerializeField] private TextMeshProUGUI _highScoreText;
  [SerializeField] private Button _respawnButton;
  [SerializeField] private Button _mainMenuButton;

  public Action OnRespawnPressed = null;
  public Action OnMainMenuPressed = null;
  
  private void Awake()
  {
    _respawnButton.onClick.AddListener(() => OnRespawnPressed?.Invoke());
    _mainMenuButton.onClick.AddListener(() => OnMainMenuPressed?.Invoke());
    
    _canvas.enabled = false;
  }

  public void SetScoreText(int score) => _scoreText.text = score.ToString();
  public void SetHighScoreText(int score) => _highScoreText.text = score.ToString();


  public void StartDelayedEnable() => StartCoroutine(DelayedEnable());

  IEnumerator DelayedEnable()
  {
    yield return new WaitForSeconds(_enableDelay);
    _canvas.enabled = true;
  }
}
