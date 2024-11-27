using System;
using System.Collections;
using Sets;
using UnityEngine;

namespace Hand
{
  public class HandAnimationController : MonoBehaviour
  {
    private HandObstacleQueue _handObstacleQueue;
    
    #region References
    private  Transform _transform;
    private  Renderer _meshRenderer;
    private  Animator _animator;
    private  HandData _handData;
    private GameObject _handBone;
    #endregion
    
    #region Strategies
    private  IHandMovement _movementStrategy;
    private  IHandValidator _isReadyForGrabStrategy;
    #endregion
    
    #region Cached Data
    private Obstacle _nextObstacle;
    public Obstacle NextObstacle => _nextObstacle;
    private Obstacle _obstacleCopy;
    #endregion

    #region Flags
    public bool IsPlaying { get; private set; }
    private bool _hasAnimationEnded;
    private bool _timeToDoObjectThing;
    #endregion
    
    #region Actions

    public Action OnPlayScarySound;
    public Action OnPlayGrabSound;
    public Action OnPlayPlaceSound;
    #endregion

    public void Initialize(HandData handData, GameObject handBone)
    {
      _transform = transform;
      _meshRenderer = GetComponentInChildren<Renderer>();
      _meshRenderer.enabled = false;
      _animator = GetComponentInChildren<Animator>();
            
      _handData = handData;
      _handBone = handBone;
      
      InitializeStrategies(handData);
      InitializeQueue(handData);
    }

    public void InitializeStrategies(HandData handData)
    {
      _isReadyForGrabStrategy = new IsReadyForGrabStrategy();
      IsReadyForGrabStrategy.MyZPosition = transform.position.z;
      IsReadyForGrabStrategy.GrabAnimationTiming = handData.GrabAnimationTiming;
      IsReadyForGrabStrategy.PlaceAnimationTiming = handData.PlaceAnimationTiming;

      // Initialize movement strategy
      _movementStrategy = new MoveHandStrategy
      {
        HiddenToIdleSpeed = handData.MovementSpeed,
        HiddenPosition = handData.HiddenPosition,
        IdlePosition = handData.IdlePosition,
        HandTransform = transform
      };
    }
    
    public void InitializeQueue(HandData handData)
    {
      _handObstacleQueue = new HandObstacleQueue(transform, new HasEnoughTimeStrategy());
      HasEnoughTimeStrategy.HiddenToIdleDistance = Mathf.Abs(handData.IdlePosition.x - handData.HiddenPosition.x);
      HasEnoughTimeStrategy.HiddenToIdleSpeed = handData.MovementSpeed;
      HasEnoughTimeStrategy.GrabAnimationTiming = handData.GrabAnimationTiming;
      HasEnoughTimeStrategy.PlaceAnimationTiming = handData.PlaceAnimationTiming;
    }

    public bool TryEnqueueObstacle(Obstacle obstacle)
    {
      if (!_handObstacleQueue.TryAddObstacleLast(obstacle, _nextObstacle))
        return false;

      if (obstacle.GrabType == GrabType.PLACED)
        obstacle.IsVisible = false;

      CheckQueue();
      return true;
    }
    
    public void CheckQueue()
    {
      if (IsPlaying)
        return;
      
      if (_nextObstacle || !_handObstacleQueue.TryDequeueNextObstacle(out _nextObstacle) )
        return;

      _obstacleCopy = Instantiate(_nextObstacle);
      _obstacleCopy.transform.parent = _handBone.transform;
      _obstacleCopy.transform.position = new Vector3(0, -3.33f, 33f); 
      _obstacleCopy.IsVisible = false;
    
      StartCoroutine(PerformAnimations());
    }
        
    private IEnumerator PerformAnimations()
    {
      IsPlaying = true;
      _transform.position = _handData.HiddenPosition;
      _meshRenderer.enabled = true;

      yield return new WaitUntil(() => _movementStrategy.MoveFromHiddenToIdle());
      if (!_nextObstacle || _nextObstacle.transform.position.z - _transform.position.z <= 0 )
      {
        Debug.LogError("The obstacle is not at a valid position for grabbing (ignorable error)");
        _transform.position = _handData.IdlePosition;
        yield return new WaitUntil(() => _movementStrategy.MoveFromIdleToHidden());
        IsPlaying = false;
        _meshRenderer.enabled = false;
        DropObstacle();
        yield break;
      }

      _transform.position = _handData.IdlePosition;
      yield return new WaitUntil(() => _isReadyForGrabStrategy
        .IsValid(_nextObstacle.transform.position.z, _nextObstacle.GrabType));
        
        
      _transform.position = _handData.AnimationPosition;
      if(_nextObstacle.GrabType == GrabType.PLACED)
      {
        _obstacleCopy.IsVisible = true;
        _animator.SetTrigger("DoPlace");
        _obstacleCopy.transform.localPosition = Vector3.zero;
      }
      else
        _animator.SetTrigger("DoGrab");

      OnPlayScarySound();
      _timeToDoObjectThing = false;
      yield return new WaitUntil(() => _timeToDoObjectThing);

      if (_nextObstacle.GrabType == GrabType.GRABBED)
      {
        _nextObstacle.IsVisible = false;
        _obstacleCopy.IsVisible = true;
        _obstacleCopy.transform.localPosition = Vector3.zero;
        OnPlayGrabSound();
      }
      else if(_nextObstacle.GrabType == GrabType.PLACED)
      {
        _nextObstacle.IsVisible = true;
        _obstacleCopy.IsVisible = false;
        OnPlayPlaceSound();
      }

      _hasAnimationEnded = false;
      yield return new WaitUntil(() => _hasAnimationEnded);
        
      IsPlaying = false;
      _meshRenderer.enabled = false;        
      _animator.ResetTrigger("DoGrab");
        
      DropObstacle();
        
      CheckQueue();
    }

    private void DropObstacle()
    {
      Destroy(_obstacleCopy.gameObject);
      _nextObstacle = null;
    }
    public void OnHandAnimationEnd()
    {
      _hasAnimationEnded = true;
    }
    public void DoObjectThing()
    {
      _timeToDoObjectThing = true;
    }
    public void HideObstacle()
    {
      _nextObstacle.IsVisible = false;
      _obstacleCopy.IsVisible = false;
      _meshRenderer.enabled = false;
    }
  }
}