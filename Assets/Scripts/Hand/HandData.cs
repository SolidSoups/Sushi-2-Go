using UnityEngine;

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
}