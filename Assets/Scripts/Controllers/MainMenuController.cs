using System;
using AudioScripts;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Controllers
{
  public class MainMenuController : MonoBehaviour
  {
    [Header("References")]
    [SerializeField] private UI_MainMenusController _uiMainMenusController;
    [SerializeField] private AudioManager _audioManager;

    private void Awake()
    {
      var uiMainMenu = _uiMainMenusController.MainMenu;
      uiMainMenu.OnStartGameButtonPressed = () =>
      {
        PlayerPrefs.SetInt("PlayPosterIntro", 1);
        LoadGameScene();
      };
      uiMainMenu.OnQuitButtonPressed = Application.Quit;
    }
    private void Start()
    {
      if (!PlayerPrefs.HasKey("NotFirstTime"))
      {
        PlayerPrefs.SetInt("NotFirstTime", 1);
        PlayerPrefs.SetInt("SavedHighScore", 0);
        _audioManager.ResetSliders();
        PlayerPrefs.Save();
      }
      _audioManager.Play(this, "MainMenu");        
    }

    private void Update()
    {
      if (Input.GetKeyDown(KeyCode.Escape))
      {
        _uiMainMenusController.RemoveLatestFromStack();
      } 
    }


    public void LoadGameScene() => SceneManager.LoadScene(1);
  }
}