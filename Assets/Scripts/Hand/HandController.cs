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
    public class HandController : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Animator _animator;
        [SerializeField] private SkinnedMeshRenderer _mesh;
        [FormerlySerializedAs("_bone")] [SerializeField] private GameObject _handBone;
        [SerializeField] private string _handName;

        [Header("Speeds")] 
        [SerializeField] private float _hiddenToIdleSpeed = 2f;
        private float _currentHiddenToIdleSpeed;
    
        [FormerlySerializedAs("_animationTiming")]
        [Header("Timing")] 
        [SerializeField] private float _grabAnimationTiming = 0.6f;
        [FormerlySerializedAs("_placeTiming")] [SerializeField] private float _placeAnimationTiming = 0.5f;

        [Header("Events")] 
        public GameEvent OnPlaySound;

        [Header("Sounds")] 
        public string _placeSound;
        public string _grabSound;
        public string _scarySound;

        private float _GetAnimationSpeed
        {
            get
            {
                switch (_nextGrabType)
                {
                    case GrabType.PLACED:
                        return _placeAnimationTiming / _diffC.SpeedScale;
                    case GrabType.GRABBED:
                        return _grabAnimationTiming / _diffC.SpeedScale;
                }

                return Mathf.Max(_grabAnimationTiming, _placeAnimationTiming) / _diffC.SpeedScale;
            }
        }
        private float _currentAnimationTiming;

        public bool IsPlaying { get; private set; }
        
        // STRATEGIES
        private IHandValidator _hasEnoughTimeStrategy;
        private IHandValidator _isReadyForGrabStrategy;
        private IHandValidator _isObstacleValidStrategy;
    
        [Header("Positions")] 
        [SerializeField] private Vector3 _hiddenPosition;
        [SerializeField] private Vector3 _idlePosition;
        [FormerlySerializedAs("_grabPosition")] [SerializeField] private Vector3 _animationPosition;

        private Obstacle _nextObstacle;
        private GrabType _nextGrabType;
        private Obstacle _obstacleCopy;
        private Renderer _nextObstacleRenderer;
        private Renderer _obstacleCopyRenderer;
        private LinkedList<Obstacle> ObstacleList = new LinkedList<Obstacle>();
    
        private DifficultyController _diffC;

        private void Awake()
        {
            _mesh.enabled = false;
            _currentHiddenToIdleSpeed = _hiddenToIdleSpeed;
            _currentAnimationTiming = _grabAnimationTiming;
            _diffC = Singelton.Instance.DifficultyController;
        }

        private void Start()
        {
            // Initialize strategies
            _hasEnoughTimeStrategy = new HasEnoughTimeStrategy(
                Singelton.Instance.SpeedController,
                Singelton.Instance.DifficultyController
            );
            HasEnoughTimeStrategy.HiddenToIdleDistance = Mathf.Abs(_idlePosition.x - _hiddenPosition.x);
            HasEnoughTimeStrategy.HiddenToIdleSpeed = _hiddenToIdleSpeed;
            HasEnoughTimeStrategy.GrabAnimationTiming = _grabAnimationTiming;
            HasEnoughTimeStrategy.PlaceAnimationTiming = _placeAnimationTiming;

            _isReadyForGrabStrategy = new IsReadyForGrabStrategy(
                Singelton.Instance.SpeedController
            );
            IsReadyForGrabStrategy.MyZPosition = transform.position.z;
            IsReadyForGrabStrategy.GrabAnimationTiming = _grabAnimationTiming;
            IsReadyForGrabStrategy.PlaceAnimationTiming = _placeAnimationTiming;

            _isObstacleValidStrategy = new IsObstacleValidStrategy();
            IsObstacleValidStrategy.MyZPosition = transform.position.z;
        }

        private void Update()
        {
            _currentHiddenToIdleSpeed = _hiddenToIdleSpeed * _diffC.SpeedScale;
            _animator.speed = _diffC.SpeedScale;
        }

        public bool TryAttachObstacle(Obstacle obstacle)
        {
            float myZ = transform.position.z;
            float obstacleZPosition = obstacle.transform.position.z;
            float distance = obstacleZPosition - myZ;

            // first case, distance has passed
            if (distance <= 0)
            {
                return false;
            }
        
            // second case, we do not have enough time at all
            if (!_hasEnoughTimeStrategy.IsValid(distance, obstacle.GrabType))
            {
                return false;
            }

            // third case, the time between this obstacle and the preceeding is not enough
            if (ObstacleList.Count != 0)
            {
                Obstacle lastObstacle = ObstacleList.Last.Value; 
                float lastObstacleZPosition = lastObstacle.transform.position.z;
                float lDist = obstacleZPosition - lastObstacleZPosition;
                if (!_hasEnoughTimeStrategy.IsValid(lDist, obstacle.GrabType))
                {
                    return false;
                }
            }
            else if(_nextObstacle)
            {
                float nextObstacleZPosition = _nextObstacle.transform.position.z; 
                float nDist = obstacleZPosition - nextObstacleZPosition;
                if (!_hasEnoughTimeStrategy.IsValid(nDist, obstacle.GrabType))
                {
                    return false;
                }
            }
        
            ObstacleList.AddLast(obstacle);
        
            // hide if object is placed
            if (obstacle.GrabType == GrabType.PLACED)
            {
                obstacle.IsVisible = false;
            }
        
            CheckQueue();
            return true;
        }

        void CheckQueue()
        {
            if (_nextObstacle || ObstacleList.Count == 0 || IsPlaying)
                return;

            _nextObstacle = ObstacleList.First.Value;
            _nextGrabType = _nextObstacle.GrabType;
            // create a copy
            _obstacleCopy = Instantiate(_nextObstacle);
            _obstacleCopy.transform.parent = _handBone.transform;
            _obstacleCopy.transform.position = new Vector3(0, -3.33f, 33f);
            _obstacleCopy.IsVisible = false;
        
            ObstacleList.RemoveFirst();
            StartCoroutine(PerformAnimations());
        }

        private bool _timeToDoObjectThing = false;
        private bool _hasAnimationEnded = false;
        private IEnumerator PerformAnimations()
        {
            IsPlaying = true;
            transform.position = _hiddenPosition;
            _mesh.enabled = true;

            yield return new WaitUntil(() => MoveToPositionFromPosition(_hiddenPosition, _idlePosition));
            if (!_nextObstacle || 
                !_isObstacleValidStrategy.IsValid(_nextObstacle.transform.position.z, default))
            {
                Debug.LogError("The obstacle is not at a valid position for grabbing (ignorable error)");
                transform.position = _idlePosition;
                yield return new WaitUntil(() => MoveToPositionFromPosition(_idlePosition, _hiddenPosition));
                IsPlaying = false;
                _mesh.enabled = false;
                DropObstacle();
                yield break;
            }

            transform.position = _idlePosition;
            yield return new WaitUntil(() => _isReadyForGrabStrategy
                .IsValid(_nextObstacle.transform.position.z, _nextObstacle.GrabType));
        
        
            transform.position = _animationPosition;
            if(_nextGrabType == GrabType.PLACED)
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

            if (_nextGrabType == GrabType.GRABBED)
            {
                _nextObstacle.IsVisible = false;
                _obstacleCopy.IsVisible = true;
                _obstacleCopy.transform.localPosition = Vector3.zero;
                OnPlaySound?.Raise(this, _grabSound); 
            }
            else if(_nextGrabType == GrabType.PLACED)
            {
                _nextObstacle.IsVisible = true;
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
            _nextObstacle = null;
            _nextGrabType = GrabType.NONE;
        
        }
    

        private bool MoveToPositionFromPosition(Vector3 fromPosition, Vector3 toPosition)
        {
            float diff = toPosition.x - fromPosition.x;
            Vector3 dir = diff > 0 ? Vector3.right : Vector3.left; 
            Vector3 position = transform.position;
            position += dir * (_currentHiddenToIdleSpeed * Time.deltaTime);
            transform.position = position;
        
            if(dir == Vector3.left && transform.position.x < toPosition.x ||
               dir == Vector3.right && transform.position.x > toPosition.x)
            {
                return true;
            }

            return false;
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
            _nextObstacle.IsVisible = false;
            _obstacleCopy.IsVisible = false;
            _mesh.enabled = false;
        }
    }
}
