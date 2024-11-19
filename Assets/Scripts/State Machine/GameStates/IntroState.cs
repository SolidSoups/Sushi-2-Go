using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;

public class IntroState : State
{
    [Header("References")]
    [SerializeField] private PlayerMovementController _playerMovement;
    [SerializeField] private TimerScore _timer;
    [SerializeField] private WorldMover _worldMover;
    [SerializeField] private SetSpawner _setSpawner;
    [SerializeField] private CameraController _cameraController;
    [SerializeField] private DifficultyController _difficultyController;
    [SerializeField] private HandDelegator _handDelegator;
    [SerializeField] private MyObjectPool _myObjectPool;

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
        
        // initialize
        _worldMover.Initialize(); // THIS MUST BE INITIALIZED FIRST
        _setSpawner.Initialize();
        _myObjectPool.Initialize();
        _cameraController.Initialize();
        _playerMovement.Initialize();
        _timer.Initialize();
        _difficultyController.Initialize();
        _handDelegator.Initialize();

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
