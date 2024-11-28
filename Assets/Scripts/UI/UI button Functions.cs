using System.Collections;
using State_Machine.GameStates;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace UI
{
    public class UIbuttonFunctions : MonoBehaviour
    {
        [SerializeField] private GameObject _optionCanvas;
        [FormerlySerializedAs("_fadeToBlackCanvas")] [SerializeField] private UI_FadeToBlackCanvas uiFadeToBlackCanvas;
        [SerializeField] private UI_Controller _uiController;
        [SerializeField] private PauseState _pauseState;

        public void PlayButton()
        {
            _pauseState.UnPause();
        }

        public void MainMenu()
        {
            SceneManager.LoadScene(0);
        }

        public void ResetGame()
        {
            //StartCoroutine(RestartGame());
            _uiController.FadeToBlack.StartFade( () => SceneManager.LoadScene(1));
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
}
