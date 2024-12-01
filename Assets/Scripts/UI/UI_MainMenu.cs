using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
  [SerializeField] private Button startGameButton;
  [SerializeField] private Button optionsButton;
  [SerializeField] private Button quitButton;

  public Action OnStartGameButtonPressed = null;
  public Action OnOptionsButtonPressed = null;
  public Action OnQuitButtonPressed = null;

  private void Awake()
  {
    startGameButton.onClick.AddListener(() => OnStartGameButtonPressed?.Invoke()); 
    optionsButton.onClick.AddListener(() => OnOptionsButtonPressed?.Invoke()); 
    quitButton.onClick.AddListener(() => OnQuitButtonPressed?.Invoke()); 
  }
}
