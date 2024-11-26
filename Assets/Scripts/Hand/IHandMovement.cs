using Controllers;
using MySingelton;
using UnityEngine;

namespace Hand
{
  public interface IHandMovement
  {
    public bool MoveFromIdleToHidden();
    public bool MoveFromHiddenToIdle();
  }

  public class MoveHandStrategy : IHandMovement
  {
    readonly DifficultyController _difficultyController = Singelton.Instance.DifficultyController;
    public float HiddenToIdleSpeed;
    public Vector3 HiddenPosition;
    public Vector3 IdlePosition;
    public Transform HandTransform;

    private bool MoveFromPositionToPosition(Vector3 fromPosition, Vector3 toPosition)
    {
      float diff = toPosition.x - fromPosition.x;
      Vector3 dir = diff > 0 ? Vector3.right : Vector3.left; 
      Vector3 position = HandTransform.position;
      position += dir * (HiddenToIdleSpeed * _difficultyController.SpeedScale * Time.deltaTime);
      HandTransform.position = position;
        
      return dir == Vector3.left && position.x < toPosition.x ||
             dir == Vector3.right && position.x > toPosition.x;
    }
    
    public bool MoveFromIdleToHidden() => MoveFromPositionToPosition(IdlePosition, HiddenPosition);

    public bool MoveFromHiddenToIdle() => MoveFromPositionToPosition(HiddenPosition, IdlePosition);
  }
  
}