using AudioScripts;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MainMenuUI : MonoBehaviour, IOptionsMenu
    {
        [SerializeField] private GameObject _optionCanvas;
        [SerializeField] private GameObject _creditCanvas;
        [SerializeField] private GameObject _controllCanvas;
        [SerializeField] private AudioManager _audioManager;

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
        
        public void StartGame()
        {
            PlayerPrefs.SetInt("PlayPosterIntro", 1);
            SceneManager.LoadScene(1);
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
}

