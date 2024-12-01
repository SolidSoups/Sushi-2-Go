using System;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  public class UI_MainMenuCredits : MonoBehaviour
  {
    [Header("Buttons")] 
    [SerializeField] private Button closeButton;

    public Action OnCloseButtonPressed = null;

    private void Awake()
    {
      closeButton.onClick.AddListener(() => OnCloseButtonPressed?.Invoke()); 
    }
  }
}