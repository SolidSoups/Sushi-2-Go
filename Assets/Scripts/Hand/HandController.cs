using System;
using System.Collections;
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
        private Animator _animator;
        private SkinnedMeshRenderer _mesh;
        
        [Header("Hand Data")]
        [SerializeField] private HandData _handData;
        
        [Header("References")]
        [FormerlySerializedAs("_bone")] [SerializeField] private GameObject _handBone;

        [Header("Events")] 
        public GameEvent OnPlaySound;

        [Header("Sounds")] 
        public string _placeSound;
        public string _grabSound;
        public string _scarySound;
        
        // VALIDATION STRATEGIES
        private IHandValidator _hasEnoughTimeStrategy;
        private IHandValidator _isReadyForGrabStrategy;
        // HAND MOVEMENT STRATEGIES
        private IHandMovement _movementStrategy;
        

        // next obstacle data
        //private Obstacle _nextObstacle;
        private Obstacle _obstacleCopy;
        private Renderer _nextObstacleRenderer;
        private Renderer _obstacleCopyRenderer;
        // Handqueue
        private LinkedList<Obstacle> ObstacleList = new LinkedList<Obstacle>();
        private HandObstacleQueue _handObstacleQueue;
    
        private DifficultyController _diffC;
        public bool IsPlaying { get; private set; }

        private void Awake()
        {
            _diffC = Singelton.Instance.DifficultyController;
            
            // get references
            _animator = GetComponentInChildren<Animator>();
            _mesh = GetComponentInChildren<SkinnedMeshRenderer>();
            _mesh.enabled = false;
        }   

        private void Start()
        {
            // Initialize validation strategies
            _hasEnoughTimeStrategy = new HasEnoughTimeStrategy();
            HasEnoughTimeStrategy.HiddenToIdleDistance = Mathf.Abs(_handData.IdlePosition.x - _handData.HiddenPosition.x);
            HasEnoughTimeStrategy.HiddenToIdleSpeed = _handData.MovementSpeed;
            HasEnoughTimeStrategy.GrabAnimationTiming = _handData.GrabAnimationTiming;
            HasEnoughTimeStrategy.PlaceAnimationTiming = _handData.PlaceAnimationTiming;

            _isReadyForGrabStrategy = new IsReadyForGrabStrategy();
            IsReadyForGrabStrategy.MyZPosition = transform.position.z;
            IsReadyForGrabStrategy.GrabAnimationTiming = _handData.GrabAnimationTiming;
            IsReadyForGrabStrategy.PlaceAnimationTiming = _handData.PlaceAnimationTiming;

            // Initialize movement strategy
            _movementStrategy = new MoveHandStrategy
            {
                HiddenToIdleSpeed = _handData.MovementSpeed,
                HiddenPosition = _handData.HiddenPosition,
                IdlePosition = _handData.IdlePosition,
                HandTransform = transform
            };

            _handObstacleQueue = new HandObstacleQueue(transform, _hasEnoughTimeStrategy);
        }

        private void Update()
        {
            _animator.speed = _diffC.SpeedScale;
        }


        public bool TryAttachObstacle(Obstacle obstacle)
        {
            bool returnValue = _handObstacleQueue.TryAddObstacleLast(obstacle);
            if (returnValue)
            {
                CheckQueue();
                return true;
            }

            return false;
        }

        void CheckQueue()
        {
            if (IsPlaying)
                return;

            if (!_handObstacleQueue.TryDequeueNextObstacle())
                return;

            // create a copy
            _obstacleCopy = Instantiate(_handObstacleQueue.NextObstacle);//_nextObstacle);
            _obstacleCopy.transform.parent = _handBone.transform;
            _obstacleCopy.transform.position = new Vector3(0, -3.33f, 33f);
            _obstacleCopy.IsVisible = false;
        
            StartCoroutine(PerformAnimations());
        }

        private bool _timeToDoObjectThing = false;
        private bool _hasAnimationEnded = false;
        private IEnumerator PerformAnimations()
        {
            IsPlaying = true;
            transform.position = _handData.HiddenPosition;
            _mesh.enabled = true;
            Obstacle nextObstacle = _handObstacleQueue.NextObstacle;

            yield return new WaitUntil(() => _movementStrategy.MoveFromHiddenToIdle());
            if (!nextObstacle || nextObstacle.transform.position.z - transform.position.z <= 0 )
            {
                Debug.Log($"!Next Obstacle: {!nextObstacle}");
                Debug.Log($"obstacle Z: {nextObstacle.transform.position.z} - transform z : {transform.position.z}");
                Debug.LogError("The obstacle is not at a valid position for grabbing (ignorable error)");
                transform.position = _handData.IdlePosition;
                yield return new WaitUntil(() => _movementStrategy.MoveFromIdleToHidden());
                IsPlaying = false;
                _mesh.enabled = false;
                DropObstacle();
                yield break;
            }

            transform.position = _handData.IdlePosition;
            yield return new WaitUntil(() => _isReadyForGrabStrategy
                .IsValid(nextObstacle.transform.position.z, nextObstacle.GrabType));
        
        
            transform.position = _handData.AnimationPosition;
            if(nextObstacle.GrabType == GrabType.PLACED)
            {
                _obstacleCopy.IsVisible = true;
                _animator.SetTrigger("DoPlace");
                _obstacleCopy.transform.localPosition = Vector3.zero;
            }
            else
                _animator.SetTrigger("DoGrab");

            OnPlaySound?.Raise(this, _scarySound);
            _timeToDoObjectThing = false;
            yield return new WaitUntil(() => _timeToDoObjectThing);

            if (nextObstacle.GrabType == GrabType.GRABBED)
            {
                nextObstacle.IsVisible = false;
                _obstacleCopy.IsVisible = true;
                _obstacleCopy.transform.localPosition = Vector3.zero;
                OnPlaySound?.Raise(this, _grabSound); 
            }
            else if(nextObstacle.GrabType == GrabType.PLACED)
            {
                nextObstacle.IsVisible = true;
                _obstacleCopy.IsVisible = false;
                OnPlaySound?.Raise(this, _placeSound);
            }

            _hasAnimationEnded = false;
            yield return new WaitUntil(() => _hasAnimationEnded);
        
            IsPlaying = false;
            _mesh.enabled = false;        
            _animator.ResetTrigger("DoGrab");
        
            DropObstacle();
        
            CheckQueue();
        }

        void DropObstacle()
        {
            Destroy(_obstacleCopy.gameObject);
            // reset values
            _handObstacleQueue.ClearNextObstacle();

            //_nextGrabType = GrabType.NONE;

        }

        public void OnHandAnimationEnd()
        {
            _hasAnimationEnded = true;
        }

        public void DoObstacleThing()
        {
            _timeToDoObjectThing = true;
        }

        public void HideObstacle()
        {
            _handObstacleQueue.NextObstacle.IsVisible = false;
            _obstacleCopy.IsVisible = false;
            _mesh.enabled = false;
        }
    }
}
