using System;
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
        
    public TurnAnimator(SetMover setMover)
    {
      _setMover = setMover;
            
    }

    public void Animate()
    {
      for(int i=_sets.Count - 1; i >= 0; i--)
      {
        Set set = _sets[i];
        
        // reset obstacles and return set
        if (set.transform.position.z + set.Length <= _setMover.turnEndAPosition.position.z)
        {
          _sets.Remove(set);
          Array.ForEach(set.MyChildren, child => child.ResetTransform());
          continue;
        }

        foreach (Obstacle obstacle in set.MyChildren)
        {
          const float halfPI = Mathf.PI * 0.5f;
          
          Vector3 obPos = obstacle.GetRealPosition();
          float rad = obPos.x - _setMover.turnStartAPosition.position.x;
          float circD4 = rad * halfPI;

          var endAPos = _setMover.turnEndAPosition.position;
          var startAPos = _setMover.turnStartAPosition.position; 
          float dist = obPos.z - endAPos.z;
          float distRatio = (dist / circD4);
          // Debug.Log($"Dist: {dist}, circD4 {circD4}");
          float angle = distRatio * halfPI;

          if (angle > halfPI)
          {
            // draw linearly
            float newZ = startAPos.z + obPos.x - endAPos.x;
            float newX = startAPos.x - (dist - circD4); 
            
            obstacle.transform.position = new Vector3(newX, obPos.y, newZ);
            continue;
          }

          if (angle <= 0)
          {
            // reset 
            obstacle.ResetTransform();
            continue;
          }

          // draw on arc
          float arcX = startAPos.x + rad * (float)Mathf.Cos(angle);
          float arcZ = endAPos.z + rad * (float)Mathf.Sin(angle);
          obstacle.transform.position = new Vector3(arcX, obPos.y, arcZ); 
        }

      }
    }
    public void AddSet(Set set) => _sets.Add(set);
    private float CalculateRadius(float x1) => _setMover.turnStartAPosition.position.x - x1;

       
  }
}