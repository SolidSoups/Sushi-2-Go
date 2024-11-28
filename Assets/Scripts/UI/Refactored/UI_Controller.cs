using System;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

public class UI_Controller : MonoBehaviour
{
  public UI_DeathScreenCanvas DeathScreen;
  public UI_FadeToBlackCanvas FadeToBlack;
  public UI_PosterIntroCanvas PosterIntro;

  private void Awake()
  {
    DeathScreen.gameObject.SetActive(true);
    FadeToBlack.gameObject.SetActive(true);
    PosterIntro.gameObject.SetActive(true); 
  }
}
