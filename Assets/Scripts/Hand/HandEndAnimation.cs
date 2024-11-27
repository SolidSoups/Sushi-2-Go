using System;
using UnityEngine;

namespace Hand
{
    public class HandEndAnimation : MonoBehaviour
    {
        private HandAnimationController _handAnimationController;
        private void Start()
        {
            _handAnimationController = GetComponentInParent<HandAnimationController>();
        }

        //...
        public void EndHandAnimation() => _handAnimationController.OnHandAnimationEnd();
        public void DoObstacleThing() => _handAnimationController.DoObjectThing();
        public void HideObstacle() => _handAnimationController.HideObstacle();
    }
}
