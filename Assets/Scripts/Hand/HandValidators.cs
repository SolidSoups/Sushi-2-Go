using Controllers;
using Sets;
using UnityEngine;

namespace Hand
{
  public abstract class HandValidatorArguments{}

  public class HandHasEnoughTimeArguments : HandValidatorArguments
  {
    public float DistanceToObstacle;
    public GrabType GrabType;
  }
  
  
  
  public interface IHandValidator
  {
    public bool IsValid(HandValidatorArguments _args);
  }
  
  public class HandHasEnoughTime : IHandValidator
  {
    public readonly SpeedController SpeedController;
    public readonly DifficultyController DifficultyController;
    public static float HiddenToIdleDistance;
    public static float HiddenToIdleSpeed;
    public static float GrabAnimationTiming;
    public static float PlaceAnimationTiming;

    public HandHasEnoughTime(SpeedController speedController, DifficultyController difficultyController)
    {
      SpeedController = speedController;
      DifficultyController = difficultyController;
    }
        
    public bool IsValid(HandValidatorArguments _args)
    {
      if (_args is not HandHasEnoughTimeArguments)
      {
        Debug.LogError("HandValidatorArguments is not HandHasEnoughTimeArguments");
        return false;
      }
      HandHasEnoughTimeArguments args = (HandHasEnoughTimeArguments)_args;
      
      float secondsToShow = HiddenToIdleDistance / (HiddenToIdleSpeed * DifficultyController.SpeedScale);
      
      float dist = Mathf.Abs(args.DistanceToObstacle);
      float accel = SpeedController.Acceleration;
      float vel = SpeedController.Speed;
      float timeToReachVel = vel / accel;
      float squareSeconds = -2 * dist / accel;
      float timeTillImpact = -timeToReachVel + Mathf.Sqrt(timeToReachVel * timeToReachVel - squareSeconds);
        
      float animationTimingSeconds = args.GrabType == GrabType.GRABBED ? GrabAnimationTiming : PlaceAnimationTiming;
      animationTimingSeconds /= DifficultyController.SpeedScale;
      float timingNeeded = animationTimingSeconds + secondsToShow*2;
        
      if (timeTillImpact <= timingNeeded)
        return false;
        
      return true;
    }
  }
}