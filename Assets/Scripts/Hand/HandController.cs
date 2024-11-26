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
        private bool useOld;
        
        [Header("Hand Data")]
        [SerializeField] private HandData _handData;
        
        [Header("References")]
        [SerializeField] private Animator _animator;
        [SerializeField] private SkinnedMeshRenderer _mesh;
        [FormerlySerializedAs("_bone")] [SerializeField] private GameObject _handBone;

        [Header("Events")] 
        public GameEvent OnPlaySound;

        [Header("Sounds")] 
        public string _placeSound;
        public string _grabSound;
        public string _scarySound;


        public bool IsPlaying { get; private set; }
        
        // VALIDATION STRATEGIES
        private IHandValidator _hasEnoughTimeStrategy;
        private IHandValidator _isReadyForGrabStrategy;
        private IHandValidator _isObstacleValidStrategy;
        // HAND MOVEMENT STRATEGIES
        private IHandMovement _movementStrategy;

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
            _diffC = Singelton.Instance.DifficultyController;
        }

        private void Start()
        {
            // Initialize validation strategies
            _hasEnoughTimeStrategy = new HasEnoughTimeStrategy(
                Singelton.Instance.SpeedController,
                Singelton.Instance.DifficultyController
            );
            HasEnoughTimeStrategy.HiddenToIdleDistance = Mathf.Abs(_handData.IdlePosition.x - _handData.HiddenPosition.x);
            HasEnoughTimeStrategy.HiddenToIdleSpeed = _handData.MovementSpeed;
            HasEnoughTimeStrategy.GrabAnimationTiming = _handData.GrabAnimationTiming;
            HasEnoughTimeStrategy.PlaceAnimationTiming = _handData.PlaceAnimationTiming;

            _isReadyForGrabStrategy = new IsReadyForGrabStrategy(
                Singelton.Instance.SpeedController
            );
            IsReadyForGrabStrategy.MyZPosition = transform.position.z;
            IsReadyForGrabStrategy.GrabAnimationTiming = _handData.GrabAnimationTiming;
            IsReadyForGrabStrategy.PlaceAnimationTiming = _handData.PlaceAnimationTiming;

            _isObstacleValidStrategy = new IsObstacleValidStrategy();
            IsObstacleValidStrategy.MyZPosition = transform.position.z;
            // Initialize movement strategy
            _movementStrategy = new MoveHandStrategy(Singelton.Instance.DifficultyController)
            {
                HiddenToIdleSpeed = _handData.MovementSpeed,
                HiddenPosition = _handData.HiddenPosition,
                IdlePosition = _handData.IdlePosition,
                HandTransform = transform
            };
        }

        private void Update()
        {
            _animator.speed = _diffC.SpeedScale;
        }

        public bool TryAttachObstacle(Obstacle obstacle)
        {
            if (!CanAttachObstacle(obstacle))
                return false;
            ObstacleList.AddLast(obstacle);
        
            if (obstacle.GrabType == GrabType.PLACED)
            {
                obstacle.IsVisible = false;
            }
        
            CheckQueue();
            return true;
        }
        public bool CanAttachObstacle(Obstacle obstacle)
        {
            float distance = obstacle.transform.position.z - transform.position.z;

            if (distance <= 0)
                return false;
        
            if (!_hasEnoughTimeStrategy.IsValid(distance, obstacle.GrabType))
                return false;

            if (ObstacleList.Count != 0)
            {
                Obstacle lastObstacle = ObstacleList.Last.Value;
                float lDist = obstacle.transform.position.z - lastObstacle.transform.position.z;
                if (!_hasEnoughTimeStrategy.IsValid(lDist, obstacle.GrabType))
                    return false;
            }
            else if(_nextObstacle)
            {
                float nDist = obstacle.transform.position.z - _nextObstacle.transform.position.z;
                if (!_hasEnoughTimeStrategy.IsValid(nDist, obstacle.GrabType))
                    return false;
            }

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
            transform.position = _handData.HiddenPosition;
            _mesh.enabled = true;

            yield return new WaitUntil(() => _movementStrategy.MoveFromHiddenToIdle());
            if (!_nextObstacle || 
                !_isObstacleValidStrategy.IsValid(_nextObstacle.transform.position.z, default))
            {
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
                .IsValid(_nextObstacle.transform.position.z, _nextObstacle.GrabType));
        
        
            transform.position = _handData.AnimationPosition;
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
