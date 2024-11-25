using System.Collections;
using Controllers;
using Controllers.Controller;
using Hand;
using Player;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace State_Machine.GameStates
{
    public class SetupState : State
    {
        [FormerlySerializedAs("_posterUI")]
        [Header("Poster intro")] 
        [SerializeField] private PanelIntroTimer _panelIntroTimer;
        [SerializeField] private FadeToBlackCanvas _fadeToBlackCanvas;
        [SerializeField] private bool _playPosterIntro = true;

        [SerializeField] private GameObject _playerCanvas;
    
        public override void EnterState()
        {
            base.EnterState();
            Time.timeScale = 1f;

            if(_playPosterIntro && PlayerPrefs.HasKey("PlayPosterIntro") && PlayerPrefs.GetInt("PlayPosterIntro") == 1)
                StartCoroutine(PlayPosterIntro());
            else
            {
                _playerCanvas.SetActive(true);
                GameManager.Instance.SwitchState<PlayingState>();
            }
        }

        private IEnumerator PlayPosterIntro()
        {
            _panelIntroTimer.gameObject.SetActive(true);
            _panelIntroTimer.StartFadeInIntro();
            yield return new WaitUntil(() => _panelIntroTimer.IsFinished);
        
            _fadeToBlackCanvas.gameObject.SetActive(true);
            _fadeToBlackCanvas.StartFade();
            yield return new WaitUntil(() => _fadeToBlackCanvas.IsFinished);
        
            _fadeToBlackCanvas.gameObject.SetActive(false);
            _panelIntroTimer.gameObject.SetActive(false);
        
            PlayerPrefs.SetInt("PlayPosterIntro", 0);
        
            // start game
            _playerCanvas.SetActive(true);
            GameManager.Instance.SwitchState<PlayingState>();
        }
    }
}
