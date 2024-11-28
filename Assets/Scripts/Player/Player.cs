using System;
using System.Collections;
using Controllers;
using Controllers.Controller;
using Events;
using State_Machine.GameStates;
using UnityEngine;

namespace Player
{
    public class Player : MonoBehaviour
    {
        // this is a player script
        [Header("References")]
        [SerializeField] private Animator _animator;

        [Header("Death")]
        [SerializeField] private Transform _cameraTransform;
        [SerializeField] private float _deathFlySpeed;
        [SerializeField] private Vector3 _add;
    
        [Header("Events")]
        public GameEvent OnPlayerHitObstacle;
        public GameEvent OnPlaySound;

        [Header("Sounds")]
        [SerializeField] private string[] _randomSounds;
    
        [Header("Random sounds timer")]
        [SerializeField] private float _minTimeBetweenSounds = 3f;
        [SerializeField] private float _maxTimeBetweenSounds = 10f;

        private void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Obstacle"))
            {
                OnPlayerHitObstacle?.Raise(this);
                _animator.SetTrigger("DoDeath");
                StartCoroutine(LerpPosition(_add, 2));
            }
        }

        private IEnumerator LerpPosition(Vector3 add, float t)
        {
            Vector3 endScale = _cameraTransform.position + add;
            while (true)
            {
                transform.position = Vector3.Lerp(transform.position, endScale, t*Time.deltaTime);
                if (Vector3.Distance(transform.localScale, endScale) < 0.01)
                    yield break;
                yield return null;
            } 
        }

        private IEnumerator  Start()
        {
            yield return new WaitUntil( () => GameManager.Instance.IsState<PlayingState>());
            StartCoroutine(PlayRandomSounds());
        }

        private IEnumerator PlayRandomSounds()
        {
            while (true)
            {
                if (GameManager.Instance.IsState<PlayingState>())
                    yield break;
                
                float randomTime = _minTimeBetweenSounds + (_maxTimeBetweenSounds - _minTimeBetweenSounds) * UnityEngine.Random.value; 
                int randomIndex = UnityEngine.Random.Range(0, _randomSounds.Length);
                yield return new WaitForSeconds(randomTime);
       
                if(_randomSounds.Length != 0)
                    OnPlaySound?.Raise(this, _randomSounds[randomIndex]);         
            }
        }
    }
}
