using System.Collections.Generic;
using Sets;
using UnityEngine;

namespace Hand
{
  public class HandObstacleQueue
  {
    private readonly Transform _transform;
    private readonly IHandValidator _hasEnoughTimeStrategy;
        
    private LinkedList<Obstacle> _obstacleList = new LinkedList<Obstacle>();

    public Obstacle NextObstacle { get; private set; } = null;

    
    // refactoring methods
    public void AddLast(Obstacle obstacle) => _obstacleList.AddLast(obstacle);
    public int Count => _obstacleList.Count;
    public Obstacle Last => _obstacleList.Last.Value;
    public Obstacle First => _obstacleList.First.Value;
    public void RemoveFirst() => _obstacleList.RemoveFirst();
    
    
    
    
    
    public bool TryDequeueNextObstacle()
    {
      NextObstacle = null;
      if (_obstacleList.Count == 0 )
        return false;
     
      NextObstacle = _obstacleList.First.Value;
      _obstacleList.RemoveFirst();
      return NextObstacle || TryDequeueNextObstacle();
    }

    public HandObstacleQueue(Transform transform, IHandValidator hasEnoughTimeStrategy)
    {
      _transform = transform;
      _hasEnoughTimeStrategy = hasEnoughTimeStrategy;
    }

    public bool TryAddObstacleLast(Obstacle obstacle)
    {
      if (!CanAttachObstacle(obstacle))
        return false;
      _obstacleList.AddLast(obstacle);

      // TODO, where should this go???
      if (obstacle.GrabType == GrabType.PLACED)
        obstacle.IsVisible = false;

      return true;
    }
        
    public bool CanAttachObstacle(Obstacle obstacle)
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
      else if(NextObstacle)
      {
        float nDist = obstacle.transform.position.z - NextObstacle.transform.position.z;
        if (!_hasEnoughTimeStrategy.IsValid(nDist, obstacle.GrabType))
          return false;
      }

      return true;
    }
  }
}