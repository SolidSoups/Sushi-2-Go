using System;
using System.Timers;
using Controllers;
using Events;
using Hand;
using Player;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.SocialPlatforms.Impl;

namespace State_Machine.GameStates
{
    public class PlayingState : State
    {
        [Header("Settings")]
        [SerializeField] private int ScorePerSecond = 10;
        private void OnValidate() => _scoreTimer.ScorePerSecond = ScorePerSecond;
        private ScoreTimer _scoreTimer = new();
        public ScoreTimer ScoreTimer => _scoreTimer;
        
        [Header("References")]
        private ConveyorController _conveyorController; 
        

        

        [Header("UI")]
        [SerializeField] private GameObject PauseCanvas;
        [SerializeField] private GameObject OptionsCanvas;
        [FormerlySerializedAs("ui_Controller")] [SerializeField] private UI_Controller _uiController;

        [Header("Events")]
        public GameEvent OnPlayAudio;
        public GameEvent OnStopAudio;

        [Header("Sounds")]
        [SerializeField] private string _Music;
        [SerializeField] private string _BackgroundSounds;

        private void Awake()
        {
            _conveyorController = GameObject.FindGameObjectWithTag("ConveyorController").GetComponent<ConveyorController>();
        }


        public override void EnterState()
        {
            base.EnterState();
            _scoreTimer.Initialize();
            Time.timeScale = 1f;
            
            // setup audio
            OnPlayAudio?.Raise(this, _Music);
            OnPlayAudio?.Raise(this, _BackgroundSounds);

            // enable ui
            PauseCanvas.SetActive(false);
            OptionsCanvas.SetActive(false);
        }

        public override void ExitState()
        {
            base.ExitState();
            OnStopAudio?.Raise(this, _Music);
            OnStopAudio?.Raise(this, _BackgroundSounds);
        }

        public override void UpdateState()
        {
            base.UpdateState();
            _scoreTimer.Tick();
            
            _conveyorController.UpdateController();
            
            _uiController.Player.SetHighScore(_scoreTimer.Score);
            UpdatePlayerPrefsHighscore(_scoreTimer.Score);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.SwitchState<PauseState>();
            }
        }

        void UpdatePlayerPrefsHighscore(int score)
        {
            //Checks if there are a highscore
            if (PlayerPrefs.HasKey("SavedHighScore"))
            {
                //If the new score is higher than the saved one
                if(score > PlayerPrefs.GetInt("SavedHighScore"))
                {
                    //Sets the new highscore
                    PlayerPrefs.SetInt("SavedHighScore", score);
                }   
            }
            else
            {
                //if there is no highscore. set it
                PlayerPrefs.SetInt("SavedHighScore", score);
            }
            PlayerPrefs.Save();
        
            //updating the TMP
            //_finalScoreText.text = score.ToString();
            _uiController.DeathScreen.SetScoreText(score);
            //_highScoreText.text =  PlayerPrefs.GetInt("SavedHighScore").ToString();
            _uiController.DeathScreen.SetHighScoreText(PlayerPrefs.GetInt("SavedHighScore"));
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();
            _conveyorController.FixedUpdateController();
        }

        public void OnPlayerHitObstacle(Component sender, object data)
        {
            if (sender is not Player.Player)
                return;

            // we died
            GameManager.Instance.SwitchState<GameOverState>();
        }
    
        public void PauseMenu()
        {
            GameManager.Instance.SwitchState<PauseState>();    
        }
    }
}
