using Controllers;
using MySingelton;
using Sets;
using UnityEngine;

namespace Hand
{
  public interface IHandValidator
  {
    public bool IsValid(float distance, GrabType grabType);
  }
  
  public class IsReadyForGrabStrategy : IHandValidator
  {
    readonly SpeedController _speedController = Singelton.Instance.SpeedController;
    public static float MyZPosition;
    public static float GrabAnimationTiming;
    public static float PlaceAnimationTiming;

    public bool IsValid(float distance, GrabType grabType)
    {
      float timing = grabType == GrabType.GRABBED ? GrabAnimationTiming : PlaceAnimationTiming;
      float distNeece = _speedController.Speed * timing +
                (_speedController.Acceleration * timing * timing) / 2f;
      float dist = distance - MyZPosition;
      return dist <= distNeece;
    }
  }
  
  public class HasEnoughTimeStrategy : IHandValidator
  {
    readonly SpeedController _speedController = Singelton.Instance.SpeedController;
    readonly DifficultyController _difficultyController = Singelton.Instance.DifficultyController;
    public static float HiddenToIdleDistance;
    public static float HiddenToIdleSpeed;
    public static float GrabAnimationTiming;
    public static float PlaceAnimationTiming;

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