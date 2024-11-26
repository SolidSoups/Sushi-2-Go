using Controllers;
using Sets;
using UnityEngine;

namespace Hand
{
  public abstract class HandValidatorArguments{}
  public interface IHandValidator
  {
    public bool IsValid(HandValidatorArguments _args);
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
    
    public bool IsValid(HandValidatorArguments _args)
    {
      if (_args is not DistanceAndGrabTypeArguments args)
      {
        Debug.LogError("HandValidatorArguments is not a DistanceArguments");
        return false;
      } 
      
      float timing = args.GrabType == GrabType.GRABBED ? GrabAnimationTiming : PlaceAnimationTiming;
      float distNeece = _speedController.Speed * timing +
                (_speedController.Acceleration * timing * timing) / 2f;
      float dist = args.Distance - MyZPosition;
      return dist <= distNeece;
    }
  }
  
  
  

  
  public class DistanceAndGrabTypeArguments : HandValidatorArguments
  {
    public float Distance;
    public GrabType GrabType;
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
        
    public bool IsValid(HandValidatorArguments _args)
    {
      if (_args is not DistanceAndGrabTypeArguments args)
      {
        Debug.LogError("HandValidatorArguments is not HandHasEnoughTimeArguments");
        return false;
      }

      float secondsToShow = HiddenToIdleDistance / (HiddenToIdleSpeed * _difficultyController.SpeedScale);
      
      float dist = Mathf.Abs(args.Distance);
      float accel = _speedController.Acceleration;
      float vel = _speedController.Speed;
      float timeToReachVel = vel / accel;
      float squareSeconds = -2 * dist / accel;
      float timeTillImpact = -timeToReachVel + Mathf.Sqrt(timeToReachVel * timeToReachVel - squareSeconds);
        
      float animationTimingSeconds = args.GrabType == GrabType.GRABBED ? GrabAnimationTiming : PlaceAnimationTiming;
      animationTimingSeconds /= _difficultyController.SpeedScale;
      float timingNeeded = animationTimingSeconds + secondsToShow*2;
        
      if (timeTillImpact <= timingNeeded)
        return false;
        
      return true;
    }
  }
}