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
      // DebugPlayerPrefs();
      if (!PlayerPrefs.HasKey("NotFirstTime"))
      {
        UI_DebugController.Instance.AddToQueue("Set up PlayerPrefs"); 
        PlayerPrefs.SetInt("NotFirstTime", 1);
        PlayerPrefs.SetInt("SavedHighScore", 0);
        UI_DebugController.Instance.AddToQueue("Set PlayerPrefs.SavedHighScore to 0"); 
        _audioManager.ResetSliders();
        PlayerPrefs.Save();
      }
      else
        UI_DebugController.Instance.AddToQueue("PlayerPrefs already set up");
      _audioManager.Play(this, "MainMenu");        
      
      
      // UI_DebugController.Instance.AddToQueue("Finished Start");
    }
    
    

    private void DebugPlayerPrefs()
    {
      PlayerPrefs.DeleteKey("NotFirstTime");
      UI_DebugController.Instance.AddToQueue("Deleted PlayerPrefs.NotFirstTime");
      PlayerPrefs.Save();
      UI_DebugController.Instance.AddToQueue("Saved PlayerPrefs");
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