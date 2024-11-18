using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.XR;

public class PlayingState : State
{
    [SerializeField] private PlayerMovementController _playerMovement;
    [SerializeField] private TimerScore _timer;
    [SerializeField] private WorldMover _worldMover;
    [SerializeField] private SetSpawner _setSpawner;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private DifficultyController _difficultyController;
    [SerializeField] private HandDelegator _handDelegator;
    [SerializeField] private UI_Player _uiPlayer;
    [SerializeField] private Player _player;
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
        _worldMover.DoFixedUpdate();
    }

    public override void UpdateState()
    {
        base.UpdateState();
        
        _timer.DoUpdate();
        _setSpawner.DoUpdate();
        _playerMovement.DoUpdate();
        _difficultyController.DoUpdate();
        _handDelegator.DoUpdate();
        _particleEffectController.DoUpdate();

        _uiPlayer.SetHighScore(_timer.Score);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            GameManager.Instance.SwitchState<PauseState>();
        }
    }

    public void OnPlayerHitObstacle(Component sender, object data)
    {
        if (sender is not Player)
            return;

        // we died
        GameManager.Instance.SwitchState<GameOverState>();
    }
    
    public void PauseMenu()
    {
        GameManager.Instance.SwitchState<PauseState>();    
    }
}
