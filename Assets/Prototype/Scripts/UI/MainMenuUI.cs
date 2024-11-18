using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private GameObject _optionCanvas;
    [SerializeField] private GameObject _creditCanvas;
    [SerializeField] private GameObject _controllCanvas;
    [SerializeField] private AudioManager _audioManager;
    public void StartGame()
    {
        PlayerPrefs.SetInt("PlayPosterIntro", 1);
        SceneManager.LoadScene(1);
    }

    private void Start()
    {
        if (!PlayerPrefs.HasKey("NotFirstTime"))
        {
            PlayerPrefs.SetInt("SavedHighScore", 0);
            _audioManager.ResetSliders();
            PlayerPrefs.Save();
        }
        _audioManager.Play(this, "MainMenu");        
    }

    public void OpenSettings()
    {
        _optionCanvas.SetActive(true);
    }
    public void CloseSettings()
    {
        _optionCanvas.SetActive(false);
    }
    public void OpenCredits()
    {
        _creditCanvas.SetActive(true);
    }
    public void CloseCredits()
    {
        _creditCanvas.SetActive(false);
    }
    public void OpenCrontroll()
    {
        _controllCanvas.SetActive(true);
    }
    public void CloseControll()
    {
        _controllCanvas.SetActive(false);
    }
    public void ExitGame()
    {
        Application.Quit();
    }
}

