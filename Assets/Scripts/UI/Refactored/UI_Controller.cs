using System;
using Controllers;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class UI_Controller : MonoBehaviour
{
  [FormerlySerializedAs("DeathScreen")] public UI_DeathScreenCanvas DeathScreenMenu;
  public UI_FadeToBlackCanvas FadeToBlack;
  public UI_PosterIntroCanvas PosterIntro;
  public UI_PlayerCanvas Player;
  public UI_PauseMenuController PauseMenu;

  private void Awake()
  {
    DeathScreenMenu.gameObject.SetActive(true);
    FadeToBlack.gameObject.SetActive(true);
    PosterIntro.gameObject.SetActive(true); 
    PauseMenu.gameObject.SetActive(true);

    DeathScreenMenu.OnRespawnPressed =
      ResetGame;
    DeathScreenMenu.OnMainMenuPressed = () =>
      GameManager.Instance.MainMenuScene();
  }

  public void ResetGame()
  {
      FadeToBlack.StartFade(() =>
      {
        GameManager.Instance.ResetScene();
      });
  }
}
