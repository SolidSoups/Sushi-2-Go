﻿using System;
using System.Collections.Generic;
using Gaskellgames;
using Sets;
using UnityEngine;

namespace Controllers
{
  public class TurnAnimator
  {
    private readonly SetMover _setMover;
    private readonly List<Set> _sets = new();
    
    private readonly Vector3 _endPosition;
    private readonly Vector3 _startPosition;
        
    public TurnAnimator(SetMover setMover)
    {
      _setMover = setMover;
      _endPosition = _setMover.turnEndAPosition.position;
      _startPosition = _setMover.turnStartAPosition.position; 
    }
    public void AddSet(Set set) => _sets.Add(set);
    public void AnimateAll()
    {
      for(int i=_sets.Count - 1; i >= 0; i--)
      {
        Set set = _sets[i];
        
        // reset obstacles and return set
        if (set.transform.position.z + set.Length <= _endPosition.z)
        {
          _sets.Remove(set);
          Array.ForEach(set.MyChildren, child => child.ResetTransform());
          continue;
        }

        foreach (Obstacle obstacle in set.MyChildren)
        {
          const float halfPI = Mathf.PI * 0.5f;
          
          Vector3 obstaclePos = obstacle.GetRealPosition();
          Quaternion obstacleOrgRotation = obstacle.OriginalRotation;
          float rad = obstaclePos.x - _startPosition.x;
          
          float circD4 = rad * halfPI;
          float dist = obstaclePos.z - _endPosition.z;
          float angle = dist / circD4 * halfPI;
          
          if (angle <= 0) // reset
          {
            obstacle.ResetTransform();
            continue;
          }

          float newX, newZ;
          Quaternion rotation, result;
          if (angle > halfPI) // draw linearly
          {
            newX = _startPosition.x - (dist - circD4);
            newZ = _startPosition.z + obstaclePos.x - _endPosition.x;

            rotation = Quaternion.Euler(0, -90, 0);
            result = obstacleOrgRotation * rotation;
          }
          else // draw on arc path
          {
            rotation = Quaternion.Euler(0, (angle / halfPI) * -90, 0);
            result = obstacleOrgRotation * rotation;
            newX = _startPosition.x + rad * Mathf.Cos(angle);
            newZ = _endPosition.z + rad * Mathf.Sin(angle);
          }
          obstacle.transform.position = new Vector3(newX, obstaclePos.y, newZ); 
          obstacle.transform.rotation = result;
        }

      }
    }
  }
}