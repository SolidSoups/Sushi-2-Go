using UnityEngine;

namespace Hand
{
    public class HandEndAnimation : MonoBehaviour
    {
        [SerializeField] private HandController _handController;
    
//...
        public void EndHandAnimation() => _handController.OnHandAnimationEnd();
        public void DoObstacleThing() => _handController.DoObstacleThing();
        public void HideObstacle() => _handController.HideObstacle();
    }
}
