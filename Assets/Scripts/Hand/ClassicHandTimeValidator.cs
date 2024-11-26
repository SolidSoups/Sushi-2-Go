using Controllers;
using Sets;
using UnityEngine;

namespace Hand
{
  public interface IHandTimeValidator
  {
    public bool HasEnoughTime(float distanceToObstacle, GrabType grabType);
  }
  
  public class ClassicHandTimeValidator : IHandTimeValidator
  {
    public readonly SpeedController SpeedController;
    public readonly DifficultyController DifficultyController;
    public static float HiddenToIdleDistance;
    public static float HiddenToIdleSpeed;
    public static float AnimationTiming;
    public static float PlaceTiming;

    public ClassicHandTimeValidator(SpeedController speedController, DifficultyController difficultyController)
    {
      SpeedController = speedController;
      DifficultyController = difficultyController;
    }
        
    public bool HasEnoughTime(float distanceToObstacle, GrabType grabType)
    {
      float secondsToCover = HiddenToIdleDistance / (HiddenToIdleSpeed * DifficultyController.SpeedScale);
                
      float dist = Mathf.Abs(distanceToObstacle);
      float accel = SpeedController.Acceleration;
      float vel = SpeedController.Speed;
      float timeToReachVel = vel / accel;
      float squareSeconds = -2 * dist / accel;
      float timeTillImpact = -timeToReachVel + Mathf.Sqrt(timeToReachVel * timeToReachVel - squareSeconds);
        
      float animationSpeed = grabType == GrabType.GRABBED ? AnimationTiming : PlaceTiming;
      float timingNeeded = animationSpeed / DifficultyController.SpeedScale + secondsToCover*2;
        
      if (timeTillImpact <= timingNeeded)
        return false;
        
      return true;
    }
  }
}