using System;
using System.Collections;
using System.Collections.Generic;
using Prototype.Scripts;
using UnityEngine;

public class HandDelegator : MonoBehaviour, IControllable
{
    [Header("Hands")]
    [SerializeField] private HandController _leftHand;
    [SerializeField] private HandController _rightHand;



    public void Initialize()
    {
        _leftHand.Initialize();
        _rightHand.Initialize();
    }

    public void DoUpdate()
    {
       _leftHand.DoUpdate();
       _rightHand.DoUpdate();
    }

    public void DoFixedUpdate()
    {
        
    }
    
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
