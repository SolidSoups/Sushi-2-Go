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
    public class HandController : MonoBehaviour, IControllable
    {
        [Header("References")]
        [SerializeField] private Animator _animator;
        [SerializeField] private SkinnedMeshRenderer _mesh;
        [FormerlySerializedAs("_bone")] [SerializeField] private GameObject _handBone;
        [SerializeField] private string _handName;

        [Header("Speeds")] 
        [SerializeField] private float _hiddenToIdleSpeed = 2f;
        private float _currentHiddenToIdleSpeed;
    
        [Header("Timing")] 
        [SerializeField] private float _animationTiming = 0.6f;
        [SerializeField] private float _placeTiming = 0.5f;

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
                        return _placeTiming;// / _diffC.DifficultyScale;
                    case GrabType.GRABBED:
                        return _animationTiming;// / _diffC.DifficultyScale;
                }

                return Mathf.Max(_animationTiming, _placeTiming);// / _diffC.DifficultyScale;
            }
        }
        private float _currentAnimationTiming;

        public bool IsPlaying { get; private set; }
    
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
    
        //private DifficultyController _diffC;

        public void Initialize()
        {
            _mesh.enabled = false;
            _currentHiddenToIdleSpeed = _hiddenToIdleSpeed;
            _currentAnimationTiming = _animationTiming;
            //_diffC = 1f;// DifficultyController.Instance;
        }

        public void DoUpdate()
        {
            _currentHiddenToIdleSpeed = _hiddenToIdleSpeed;// * _diffC.DifficultyScale;
            //_animator.speed = _diffC.DifficultyScale;
        
        
        }

        public void DoFixedUpdate()
        {
        
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
            if (!HasEnoughTime(distance, obstacle.GrabType))
            {
                return false;
            }

            // third case, the time between this obstacle and the preceeding is not enough
            if (ObstacleList.Count != 0)
            {
                Obstacle lastObstacle = ObstacleList.Last.Value; 
                float lastObstacleZPosition = lastObstacle.transform.position.z;
                float lDist = obstacleZPosition - lastObstacleZPosition;
                if(!HasEnoughTime(lDist, obstacle.GrabType))
                {
                    return false;
                }
            }
            else if(_nextObstacle)
            {
                float nextObstacleZPosition = _nextObstacle.transform.position.z; 
                float nDist = obstacleZPosition - nextObstacleZPosition;
                if(!HasEnoughTime(nDist, obstacle.GrabType))
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

        private bool HasEnoughTime(float distance, GrabType grabType)
        {
            // calculate time for hand to appear
            float handDistanceToCover = Mathf.Abs(_idlePosition.x - _hiddenPosition.x);
            float secondsToCover = handDistanceToCover / _currentHiddenToIdleSpeed;
        
            // calculate time until impact
            float d = Mathf.Abs(distance);
            float a = Singelton.Instance.SpeedController.Acceleration;
            float v = Singelton.Instance.SpeedController.Speed;

            float p = v / a;
            float q = -2 * d / a;

            float timeTillImpact = -p + Mathf.Sqrt(p * p - q);

            float animationSpeed = grabType == GrabType.GRABBED ? _animationTiming : _placeTiming;
            float timingNeeded = animationSpeed;// / _diffC.DifficultyScale + secondsToCover*2;

            if (timeTillImpact <= timingNeeded)
            {
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

        private bool IsReadyForGrab(Obstacle obstacle)
        {
            float d = Singelton.Instance.SpeedController.Speed * _currentAnimationTiming +
                      (Singelton.Instance.SpeedController.Acceleration * _currentAnimationTiming * _currentAnimationTiming) / 2f;
            float zPos = transform.position.z;
            float oZPos = obstacle.transform.position.z;
            if (oZPos - zPos <= d)
                return true;
            return false;
        }

        private bool IsObstacleStillValid(Obstacle obstacle)
        {
            if (!obstacle)
            {
                Debug.Log("Obstacle doesn't exist");
                return false;
            }
            float myZPosition = transform.position.z;
            float obstacleZPosition = obstacle.transform.position.z;
            return (obstacleZPosition - myZPosition) >= 0;
        }

        private bool _timeToDoObjectThing = false;
        private bool _hasAnimationEnded = false;
        private IEnumerator PerformAnimations()
        {
            IsPlaying = true;
            transform.position = _hiddenPosition;
            _mesh.enabled = true;

            yield return new WaitUntil(() => MoveToPositionFromPosition(_hiddenPosition, _idlePosition));
            if (!IsObstacleStillValid(_nextObstacle))
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
            yield return new WaitUntil(() => IsReadyForGrab(_nextObstacle));
        
        
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
