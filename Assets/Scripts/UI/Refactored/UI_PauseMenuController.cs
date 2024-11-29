using System;
using UnityEngine;
using UnityEngine.Serialization;

public class UI_PauseMenuController : MonoBehaviour
{
  [Header("References")]
  public UI_PauseCanvas PauseCanvas;   
  public UI_OptionCanvas OptionCanvas;
  public UI_CountdownCanvas CountdownCanvas;

  public bool IsResuming { get; private set; }
  public Action OnResumeToGame = null;
  public Action OnExit = null;
  
  
  
  
  private void Awake()
  {
    OptionCanvas.gameObject.SetActive(false);
    OptionCanvas.OnCloseOptions = () =>
    {
      OptionCanvas.gameObject.SetActive(false);
      PauseCanvas.gameObject.SetActive(true);
    };
    
    PauseCanvas.gameObject.SetActive(false);
    PauseCanvas.OnResumeButtonPressed = 
      TryResumeToGameWithCountdown;
    PauseCanvas.OnSettingsButtonPressed = () =>
    {
      PauseCanvas.gameObject.SetActive(false);
      OptionCanvas.gameObject.SetActive(true);
    };
    
    CountdownCanvas.gameObject.SetActive(false);
  }
  
  public void EnablePauseMenu()
  {
    DisableAllCanvases();
    PauseCanvas.gameObject.SetActive(true);
  }

  public void DisableAllCanvases()
  {
    CountdownCanvas.gameObject.SetActive(false);
    PauseCanvas.gameObject.SetActive(false);
    OptionCanvas.gameObject.SetActive(false);
  }

  public void OnPressedEscape()
  {
    if (OptionCanvas.gameObject.activeInHierarchy)
    {
      PauseCanvas.gameObject.SetActive(true);
      OptionCanvas.gameObject.SetActive(false);
    }
    else
      TryResumeToGameWithCountdown();
  }
  
  public void TryResumeToGameWithCountdown()
  {
    if (IsResuming)
      return;
    IsResuming = true;
    DisableAllCanvases();
    CountdownCanvas.gameObject.SetActive(true); 
    CountdownCanvas.StartTimer(() =>
    {
      OnResumeToGame?.Invoke();
      CountdownCanvas.gameObject.SetActive(false);
      IsResuming = false;
    });
  }
}