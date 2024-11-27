using System;
using System.Collections.Generic;
using Controllers;
using Events;
using MySingelton;
using Sets;
using UnityEngine;
using UnityEngine.Serialization;

namespace Hand
{
    [CreateAssetMenu(fileName = "HandData", menuName = "HandController/HandData")]
    public class HandData : ScriptableObject
    {
        public string HandName;
        
        [Header("Speed")]
        public float MovementSpeed;
        
        [Header("Timing")] 
        public float GrabAnimationTiming;
        public float PlaceAnimationTiming;

        [Header("Positions")] 
        public Vector3 HiddenPosition;
        public Vector3 IdlePosition;
        public Vector3 AnimationPosition;
    }

    public class HandController : MonoBehaviour
    {
        [Header("Hand Data")]
        [SerializeField] private HandData _handData;
        
        [Header("References")]
        [FormerlySerializedAs("_bone")] [SerializeField] private GameObject _handBone;
        private Animator _animator;

        [Header("Events")] 
        public GameEvent OnPlaySound;

        [Header("Sounds")] 
        public string _placeSound;
        public string _grabSound;
        public string _scarySound;
        
        private HandAnimationController _handAnimationController;

        
        private void Awake()
        {
            _animator = GetComponentInChildren<Animator>();
            
            _handAnimationController = gameObject.AddComponent<HandAnimationController>();
        }

        private void Start()
        {
            _handAnimationController.Initialize(_handData, _handBone);
            _handAnimationController.OnPlayScarySound += PlayScarySound;
            _handAnimationController.OnPlayGrabSound += PlayGrabSound;
            _handAnimationController.OnPlayPlaceSound += PlayPlaceSound;
        }

        private void OnDisable()
        {
            _handAnimationController.OnPlayScarySound -= PlayScarySound;
            _handAnimationController.OnPlayGrabSound -= PlayGrabSound;
            _handAnimationController.OnPlayPlaceSound -= PlayPlaceSound;
        }

        private void Update()
        {
            _animator.speed = Singelton.Instance.DifficultyController.SpeedScale;
        }

        public bool TryAttachObstacle(Obstacle obstacle) => _handAnimationController.TryEnqueueObstacle(obstacle);
        
        public void PlayScarySound() => OnPlaySound?.Raise(this, _scarySound);
        public void PlayGrabSound() => OnPlaySound?.Raise(this, _grabSound);
        public void PlayPlaceSound() => OnPlaySound?.Raise(this, _placeSound);
    }
}
