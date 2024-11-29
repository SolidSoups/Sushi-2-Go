using System;
using System.Collections;
using Controllers;
using Events;
using Player;
using UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace State_Machine.GameStates
{
    public class GameOverState : State
    {
        [Header("References")] 
        [SerializeField] private GameObject _gameOverCanvas;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private UI_Controller _uiController;

        [FormerlySerializedAs("OnEnterGameOver")] [Header("Events")]
        public GameEvent OnPlaySounds;

        [Header("Sound")]
        [SerializeField] private string _gameOverSound;
        [SerializeField] private string[] _deathSounds;

        [Header("Settings")] [SerializeField] private float _gameOverCanvasTimer = 3f;

        public override void EnterState()
        {
            base.EnterState();

            _cameraController.enabled = false;
            OnPlaySounds?.Raise(this, _gameOverSound);
        
            int randomIndex = UnityEngine.Random.Range(0, _deathSounds.Length);
            if(_deathSounds.Length != 0)
                OnPlaySounds?.Raise(this, _deathSounds[randomIndex]);

            //StartCoroutine(TimedGameOverCanvas());
            _uiController.DeathScreenMenu.StartDelayedEnable();
        }

        private bool _isGameResetting = false;
        public override void UpdateState()
        {
            base.UpdateState();

            if (Input.GetKeyDown(KeyCode.Space) && !_isGameResetting)
            {
                _isGameResetting = true;
                _uiController.ResetGame();
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                GameManager.Instance.MainMenuScene();
            }
        }

        private IEnumerator TimedGameOverCanvas()
        {
            yield return new WaitForSecondsRealtime(_gameOverCanvasTimer);
            _gameOverCanvas.SetActive(true);
        }
    }
}
