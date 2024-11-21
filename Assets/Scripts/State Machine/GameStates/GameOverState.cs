using System.Collections;
using Events;
using Player;
using UI;
using UnityEngine;
using UnityEngine.Serialization;

namespace State_Machine.GameStates
{
    public class GameOverState : State
    {
        [Header("References")]
        [SerializeField] private GameObject _gameOverCanvas;
        [SerializeField] private UI_PlayerCanvas uiPlayerCanvas;
        [SerializeField] private CameraController _cameraController;
        [SerializeField] private UIbuttonFunctions _uibuttonFunctions;

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

            //uiPlayerCanvas.HighScoreUpdate();
            StartCoroutine(TimedGameOverCanvas());
        }

        public override void UpdateState()
        {
            base.UpdateState();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                _uibuttonFunctions.ResetGame(); 
            }
            else if (Input.GetKeyDown(KeyCode.Escape))
            {
                _uibuttonFunctions.MainMenu();
            }
        }

        private IEnumerator TimedGameOverCanvas()
        {
            yield return new WaitForSecondsRealtime(_gameOverCanvasTimer);
            _gameOverCanvas.SetActive(true);
        }
    }
}
