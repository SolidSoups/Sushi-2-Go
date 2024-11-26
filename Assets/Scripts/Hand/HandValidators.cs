using Controllers;
using Sets;
using UnityEngine;

namespace Hand
{
  public interface IHandValidator
  {
    public bool IsValid(float distance, GrabType grabType);
  }

  public class HandIsObstacleStillValid : IHandValidator
  {
    public static float MyZPosition;
    
    public bool IsValid(float distance, GrabType _)
    {
      return (distance - MyZPosition) >= 0;
    }
  }
  
  
  public class HandIsReadyForGrab : IHandValidator
  {
    readonly SpeedController _speedController;
    public static float MyZPosition;
    public static float GrabAnimationTiming;
    public static float PlaceAnimationTiming;

    public HandIsReadyForGrab(SpeedController speedController)
    {
      _speedController = speedController;
    }
    
    public bool IsValid(float distance, GrabType grabType)
    {
      float timing = grabType == GrabType.GRABBED ? GrabAnimationTiming : PlaceAnimationTiming;
      float distNeece = _speedController.Speed * timing +
                (_speedController.Acceleration * timing * timing) / 2f;
      float dist = distance - MyZPosition;
      return dist <= distNeece;
    }
  }
  
  public class HandHasEnoughTime : IHandValidator
  {
    readonly SpeedController _speedController;
    readonly DifficultyController _difficultyController;
    public static float HiddenToIdleDistance;
    public static float HiddenToIdleSpeed;
    public static float GrabAnimationTiming;
    public static float PlaceAnimationTiming;

    public HandHasEnoughTime(SpeedController speedController, DifficultyController difficultyController)
    {
      _speedController = speedController;
      _difficultyController = difficultyController;
    }
        
    public bool IsValid(float distance, GrabType grabType)
    {
      float secondsToShow = HiddenToIdleDistance / (HiddenToIdleSpeed * _difficultyController.SpeedScale);
      
      float dist = Mathf.Abs(distance);
      float accel = _speedController.Acceleration;
      float vel = _speedController.Speed;
      float timeToReachVel = vel / accel;
      float squareSeconds = -2 * dist / accel;
      float timeTillImpact = -timeToReachVel + Mathf.Sqrt(timeToReachVel * timeToReachVel - squareSeconds);
        
      float animationTimingSeconds = 
        grabType == GrabType.GRABBED ? GrabAnimationTiming : PlaceAnimationTiming;
      animationTimingSeconds /= _difficultyController.SpeedScale;
      float timingNeeded = animationTimingSeconds + secondsToShow*2;
        
      if (timeTillImpact <= timingNeeded)
        return false;
        
      return true;
    }
  }
}