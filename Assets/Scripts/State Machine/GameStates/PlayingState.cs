using Controllers;
using Events;
using Hand;
using Player;
using UI;
using UnityEngine;

namespace State_Machine.GameStates
{
    public class PlayingState : State
    {
        private ScoreTimer _scoreTimer = new();
        [SerializeField] private float _scorePerSecond => _scoreTimer.ScorePerSecond;

        [SerializeField] private ConveyorController _conveyorController;
        [SerializeField] private PlayerMovementController _playerMovement;
        [SerializeField] private WorldMover _worldMover;
        [SerializeField] private SetSpawner _setSpawner;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private DifficultyController _difficultyController;
        [SerializeField] private HandDelegator _handDelegator;
        [SerializeField] private UI_PlayerCanvas uiPlayerCanvas;
        [SerializeField] private Player.Player _player;
        [SerializeField] private ParticleEffectController _particleEffectController;
        [SerializeField] private RiceEffectController _riceEffectController;
        [SerializeField] private GameObject PauseCanvas;
        [SerializeField] private GameObject OptionsCanvas;

    [Header("Events")]
        public GameEvent OnPlayAudio;
        public GameEvent OnStopAudio;

        [Header("Sounds")]
        [SerializeField] private string _Music;
        [SerializeField] private string _BackgroundSounds;


        public override void EnterState()
        {
            base.EnterState();
            _conveyorController.EnableControllables();
            _player.Initialize();
            _particleEffectController.Initialize();
            _riceEffectController.Initialize();
            OnPlayAudio?.Raise(this, _Music);
            OnPlayAudio?.Raise(this, _BackgroundSounds);
            Time.timeScale = 1f;

            PauseCanvas.SetActive(false);
            OptionsCanvas.SetActive(false);
        }

        public override void ExitState()
        {
            base.ExitState();
            OnStopAudio?.Raise(this, _Music);
            OnStopAudio?.Raise(this, _BackgroundSounds);
        }

        public override void FixedUpdateState()
        {
            base.FixedUpdateState();
            //_worldMover.DoFixedUpdate();
            _conveyorController.DoFixedUpdate();
        }

        public override void UpdateState()
        {
            base.UpdateState();
        
            _conveyorController.DoUpdate();
            _setSpawner.DoUpdate();
            _playerMovement.DoUpdate();
    //            _difficultyController.DoUpdate();
            _handDelegator.DoUpdate();
            _particleEffectController.DoUpdate();

            //uiPlayerCanvas.SetHighScore(scoreTimer.Score);

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.SwitchState<PauseState>();
            }
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
