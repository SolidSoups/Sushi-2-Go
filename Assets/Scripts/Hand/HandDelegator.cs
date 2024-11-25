using System.Collections.Generic;
using Controllers;
using Controllers.Controller;
using Sets;
using UnityEngine;

namespace Hand
{
    public class HandDelegator : MonoBehaviour
    {
        [Header("Hands")]
        [SerializeField] private HandController _leftHand;
        [SerializeField] private HandController _rightHand;
    
        public bool DelegateToHands(List<Obstacle> obstacles)
        {
            bool delegatedLeft = false;
            bool delegatedRight = false;
        
            foreach (Obstacle obstacle in obstacles)
            {
                if (delegatedLeft && delegatedRight)
                    return true;
                if (!delegatedLeft && obstacle.HandSide == HandSide.LEFT)
                {
                    delegatedLeft = _leftHand.TryAttachObstacle(obstacle);
                }
                else if (!delegatedRight && obstacle.HandSide == HandSide.RIGHT)
                {
                    delegatedRight = _rightHand.TryAttachObstacle(obstacle);
                }
            }

            return delegatedLeft || delegatedRight;
        } 
    }
}
