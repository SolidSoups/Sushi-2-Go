using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class UI_OptionCanvas : MonoBehaviour
{
  [Header("References")] 
  [SerializeField] private Button _closeButton;
  
  [SerializeField] private Slider MusicSlider;
  [SerializeField] private Slider SFXSlider;
  [SerializeField] private Slider NoiseSlider;
  public float MusicLevel => MusicSlider.value;
  public float SFXLevel => SFXSlider.value;
  public float NoiseLevel => NoiseSlider.value;
  public void SetLevels(float music, float sfx, float background)
  {
    MusicSlider.value = music;
    SFXSlider.value = sfx;
    NoiseSlider.value = background;
  }
  
  
  public Action OnCloseOptions = null;
  
  private void Awake()
  {
    _closeButton.onClick.AddListener(() => OnCloseOptions?.Invoke());
  }

  private void OnDestroy()
  {
    _closeButton.onClick.RemoveAllListeners(); 
  }
}