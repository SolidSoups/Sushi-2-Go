using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIbuttonFunctions : MonoBehaviour
{
    [SerializeField] private GameObject _optionCanvas;
    [SerializeField] private FadeToBlackCanvas _fadeToBlackCanvas;
    [SerializeField] private PauseState _pauseState;

    public void PlayButton()
    {
        _pauseState.UnPause();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene("StarterMenu");
    }

    public void ResetGame()
    {
        StartCoroutine(RestartGame());
    }

    private IEnumerator RestartGame()
    {
        _fadeToBlackCanvas.gameObject.SetActive(true); 
        _fadeToBlackCanvas.StartFade();
        yield return new WaitUntil(() => _fadeToBlackCanvas.IsFinished);
        SceneManager.LoadScene(1);
    }

    public void OpenSettings()
    {
        _optionCanvas.SetActive(true);
    }
    public void CloseSettings()
    {
        _optionCanvas.SetActive(false);
    }

    public void Pause()
    {
        
    }

    public void Options()
    {

    }

    public void Quit()
    {
        
    }
}
