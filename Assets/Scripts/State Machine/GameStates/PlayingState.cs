using System;
using System.Timers;
using Controllers;
using Events;
using Hand;
using Player;
using UI;
using UnityEngine;
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
        [SerializeField] private UI_PlayerCanvas uiPlayerCanvas;
        [SerializeField] private GameObject PauseCanvas;
        [SerializeField] private GameObject OptionsCanvas;

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
            
            uiPlayerCanvas.SetHighScore(_scoreTimer.Score);
            uiPlayerCanvas.HighScoreUpdate(_scoreTimer.Score);
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.SwitchState<PauseState>();
            }
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
