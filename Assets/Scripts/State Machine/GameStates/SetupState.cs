using Controllers;
using UnityEngine;

namespace State_Machine.GameStates
{
    public class SetupState : State
    {
        [Header("Poster intro")] 
        [SerializeField] private UI_Controller _uiController;

        public override void ExitState()
        {
            base.ExitState();
            _uiController.Player.gameObject.SetActive(true);
        }

        public override void EnterState()
        {
            base.EnterState();
            Time.timeScale = 1f;

            if(PlayerPrefs.HasKey("PlayPosterIntro") && PlayerPrefs.GetInt("PlayPosterIntro") == 1)
            {
                _uiController.PosterIntro.gameObject.SetActive(true);
                _uiController.PosterIntro.StartFadeInIntro(() =>
                {
                    _uiController.FadeToBlack.StartFade(() =>
                    {
                        _uiController.FadeToBlack.Reset();
                        _uiController.PosterIntro.gameObject.SetActive(false);
                        //_uiController.Player.gameObject.SetActive(true);
                        PlayerPrefs.SetInt("PlayPosterIntro", 0);
                        GameManager.Instance.SwitchState<PlayingState>();
                    });
                });
            }
            else
            {
                //_uiController.Player.gameObject.SetActive(true);
                _uiController.PosterIntro.gameObject.SetActive(false);
                GameManager.Instance.SwitchState<PlayingState>();
            }
        }
    }
}
