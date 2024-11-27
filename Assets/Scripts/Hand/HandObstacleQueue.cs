using System.Collections.Generic;
using Sets;
using UnityEngine;

namespace Hand
{
  public class HandObstacleQueue
  {
    private readonly Transform _transform;
    private readonly IHandValidator _hasEnoughTimeStrategy;
    private readonly LinkedList<Obstacle> _obstacleList = new();
    
    
    public HandObstacleQueue(Transform transform, IHandValidator hasEnoughTimeStrategy)
    {
      _transform = transform;
      _hasEnoughTimeStrategy = hasEnoughTimeStrategy;
    }

    public bool TryDequeueNextObstacle(out Obstacle nextObstacle)
    {
      nextObstacle = null;
      if(_obstacleList.Count == 0)
        return false;
     
      nextObstacle = _obstacleList.First.Value;
      _obstacleList.RemoveFirst();
      return nextObstacle || TryDequeueNextObstacle(out nextObstacle);
    }


    public bool TryAddObstacleLast(Obstacle obstacle, Obstacle nextObstacle)
    {
      if (!CanAttachObstacle(obstacle, nextObstacle))
        return false;
      
      _obstacleList.AddLast(obstacle);

      return true;
    }
        
    public bool CanAttachObstacle(Obstacle obstacle, Obstacle nextObstacle)
    {
      float distance = obstacle.transform.position.z - _transform.position.z;

      if (distance <= 0)
        return false;
        
      if (!_hasEnoughTimeStrategy.IsValid(distance, obstacle.GrabType))
        return false;

      if (_obstacleList.Count != 0)
      {
        float lDist = obstacle.transform.position.z - _obstacleList.Last.Value.transform.position.z;
        if (!_hasEnoughTimeStrategy.IsValid(lDist, obstacle.GrabType))
          return false;
      }
      else if(nextObstacle)
      {
        float nDist = obstacle.transform.position.z - nextObstacle.transform.position.z;
        if (!_hasEnoughTimeStrategy.IsValid(nDist, obstacle.GrabType))
          return false;
      }

      return true;
    }
  }
}