using System.Collections.Generic;
using Controllers.Controller;
using MySingelton;
using Sets;
using UnityEngine;

namespace Controllers
{
  public class SetMover : Controllable
  {
    [Header("Settings")] 
    [SerializeField] private bool _drawGizmos = true;
    [SerializeField] private float _despawnZPosition = 5f;
    public Transform turnStartAPosition;
    public Transform turnStartBPosition;
    public Transform turnEndAPosition;
    public Transform turnEndBPosition;
  
    private List<GameObject> _spawnedSets = new List<GameObject>();
    public void AddSet(GameObject set) => _spawnedSets.Add(set);
  
    private GameObject _setParent;

    private void OnDrawGizmos()
    {
      if (_drawGizmos)
      {
        GizmosDrawTurn();
        GizmosDrawDespawn();
      }      
    }

    private void GizmosDrawDespawn()
    {
      // Draw Despawn Zone
      Vector3 pos = new Vector3(0, 0, _despawnZPosition);
      Vector3 size = new Vector3(10, 10, 0);

      Gizmos.color = Color.red;
      Gizmos.DrawWireCube(pos, size);
        
      Gizmos.DrawLine(new Vector3(-size.x/2, -size.y/2, _despawnZPosition), new Vector3(size.x/2, size.y/2, _despawnZPosition));
      Gizmos.DrawLine(new Vector3(-size.x/2, size.y/2, _despawnZPosition), new Vector3(size.x/2, -size.y/2, _despawnZPosition));
    }

    private void GizmosDrawTurn()
    {
      // Draw start and end of turn
      Vector3 startA = turnStartAPosition.position;
      Vector3 startB = turnStartBPosition.position;
      Vector3 endA = turnEndAPosition.position;
      Vector3 endB = turnEndBPosition.position;

      Gizmos.color = new Color(229f/255f, 154f/255f, 35f/255f, 1);
      Gizmos.DrawLine(startA, startB);
      Gizmos.DrawLine(endA, endB);
      Gizmos.DrawSphere(startA, 0.5f); 
      Gizmos.DrawSphere(startB, 0.5f); 
      Gizmos.DrawSphere(endA, 0.5f); 
      Gizmos.DrawSphere(endB, 0.5f);
      
      Vector3 startABd3 = ( startB - startA )/3f;
      Vector3 endABd3 = (endB - endA)/3f;
      Vector3 startOffset = startABd3 / 2f + startA;
      Vector3 endOffset = endABd3 / 2f + endA;
      Gizmos.color = Color.yellow;
      for (int i = 0; i < 3; i++)
      {
        Vector3 newEndPosition = endOffset + i * endABd3;
        Vector3 newStartPosition = startOffset + i * startABd3;
        Gizmos.DrawSphere(newEndPosition, 0.4f);
        Gizmos.DrawSphere(newStartPosition, 0.4f);
        
        // draw line to intersection
        Vector3 intersectionPoint = new Vector3(newEndPosition.x, newEndPosition.y, newStartPosition.z);
        Gizmos.DrawLine(newEndPosition, intersectionPoint);
        Gizmos.DrawLine(newStartPosition, intersectionPoint);
      }
    }

    public override void InitializeController()
    {
      base.InitializeController();
    
      _setParent = new GameObject("SetParent");   
      _setParent.transform.parent = transform; 
    }

    public override void FixedUpdateController()
    {
      base.FixedUpdateController();
      MoveSets();
    }

    private void MoveSets()
    {
      float currentSpeed = Singelton.Instance.SpeedController.Speed;
      for (int i = _spawnedSets.Count - 1; i >= 0; i--)
      {
        GameObject set = _spawnedSets[i];
        if (!set)
          continue;
        Vector3 pos = set.transform.position;
        pos.z -= currentSpeed * Time.deltaTime;
        set.transform.position = pos;
      
        // check despawn
        Set setComponent = set.GetComponent<Set>();
        float z = pos.z + setComponent.Length;
        if (z <= _despawnZPosition)
        {
          Despawn(setComponent);
        }
      } 
    }

    private void Despawn(Set set)
    {
      _spawnedSets.Remove(set.gameObject);
      // should be adding back to pool here 
      Singelton.Instance.ObjectPool.AddBackToPool(set);
    }
  }
}
