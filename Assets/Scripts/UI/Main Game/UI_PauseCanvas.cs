using System;
using Controllers;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_PauseCanvas : MonoBehaviour
{
  [Header("References")]
  [SerializeField] private Button _resumeButton;
  [SerializeField] private Button _settingsButton;
  [SerializeField] private Button _mainMenuButton;

  public Action OnResumeButtonPressed = null;
  public Action OnSettingsButtonPressed = null;
  
  private void OnEnable()
  {
    _resumeButton.onClick.AddListener( () => OnResumeButtonPressed?.Invoke());
    _settingsButton.onClick.AddListener( () => OnSettingsButtonPressed?.Invoke());
    _mainMenuButton.onClick.AddListener( () => GameManager.Instance.MainMenuScene());
  }

  private void OnDestroy()
  {
    _resumeButton.onClick.RemoveAllListeners();
    _settingsButton.onClick.RemoveAllListeners();
    _mainMenuButton.onClick.RemoveAllListeners();
  }
}