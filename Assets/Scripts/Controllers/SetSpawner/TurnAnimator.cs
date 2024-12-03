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

    private struct KXMLine
    {
      public float k;
      public float m;
    }
    private KXMLine line;

    public (Vector3, Vector3) GetLinePoints(float x0, float x1)
    {
      var vX0 = CalculateIntersectionOnLine(x0);
      var vX1 = CalculateIntersectionOnLine(x1);
      Vector3 newVX0 = new Vector3(vX0.x, 0, vX0.y);
      Vector3 newVX1 = new Vector3(vX1.x, 0, vX1.y);
      return (
        newVX0, newVX1
      );
    }
        
    public TurnAnimator(SetMover setMover)
    {
      _setMover = setMover;
            
      // initialize intersection line
      Vector2 start = GetLaneIntersection(0);
      Vector2 end = GetLaneIntersection(2);
      line = new KXMLine();
      line.k = (end.y - start.y) / (end.x - start.x);
      line.m = start.y - line.k * start.x;
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
          foreach (Obstacle obstacle in set.MyChildren)
          {
            obstacle.transform.localPosition = obstacle.OriginalPosition;
          }
          continue;
        }

        foreach (Obstacle obstacle in set.MyChildren)
        {
          Vector3 obstaclePosition = set.transform.position + obstacle.OriginalPosition;
          Vector2 intersectionPoint = CalculateIntersectionOnLine(obstaclePosition.x);
          float dist = obstaclePosition.z - intersectionPoint.y;

          if (dist <= 0f)
          {
            obstacle.transform.localPosition = obstacle.OriginalPosition;
            continue;
          }
          
          // translate position 
          Vector3 newPosition = new Vector3(intersectionPoint.x - dist, obstaclePosition.y, intersectionPoint.y);
          obstacle.transform.position = newPosition;
        }        
               
      }
    }
    public void AddSet(Set set) => _sets.Add(set);

    private Vector2 CalculateIntersectionOnLine(float x)
    {
      return new Vector3(x, line.k * x + line.m);
    }

    private Vector2 GetLaneIntersection(int lane)
    {
      lane = Mathf.Clamp(lane, 0, 3);
            
      // start positions
      Vector3 startAPosition = _setMover.turnStartAPosition.position;
      Vector3 startBPosition = _setMover.turnStartBPosition.position;
      Vector3 startABd3 = (startBPosition - startAPosition) / 3f;
      Vector3 startPos = startABd3 / 2f + startABd3 * lane;
            
      // end positions
      Vector3 endAPosition = _setMover.turnEndAPosition.position;
      Vector3 endBPosition = _setMover.turnEndBPosition.position;
      Vector3 endABd3 = (endBPosition - endAPosition) / 3f;
      Vector3 endPos = endABd3 / 2f + endABd3 * lane;
            
      // intersection
      return new Vector2(endAPosition.x + endPos.x, startAPosition.z + startPos.z);
    }
       
  }
}