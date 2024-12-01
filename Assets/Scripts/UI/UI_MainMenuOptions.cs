using System;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
  public class UI_MainMenuOptions : MonoBehaviour, IOptionsMenu
  {
    [Header("UI Elements")] 
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _sfxSlider;
    [SerializeField] private Slider _noiseSlider;
    public float MusicLevel => _musicSlider.value;
    public float SFXLevel => _sfxSlider.value;
    public float NoiseLevel => _noiseSlider.value;
    public void SetLevels(float musicLevel, float sfxLevel, float noiseLevel)
    {
      _musicSlider.value = musicLevel;
      _sfxSlider.value = sfxLevel;
      _noiseSlider.value = noiseLevel;
    }
    
    [Header("Buttons")] 
    [SerializeField] private Button closeButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button controlsButton;

    public Action OnCloseButtonPressed;
    public Action OnCreditsButtonPressed;
    public Action OnControlsButtonPressed;

    private void Awake()
    {
      closeButton.onClick.AddListener(() => OnCloseButtonPressed?.Invoke()); 
      creditsButton.onClick.AddListener(() => OnCreditsButtonPressed?.Invoke()); 
      controlsButton.onClick.AddListener(() => OnControlsButtonPressed?.Invoke()); 
    }
  }
}