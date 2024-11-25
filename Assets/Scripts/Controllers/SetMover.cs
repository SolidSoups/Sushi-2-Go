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
  
    private List<GameObject> _spawnedSets = new List<GameObject>();
    public void AddSet(GameObject set) => _spawnedSets.Add(set);
  
    private GameObject _setParent;

    private void OnDrawGizmos()
    {
      if (_drawGizmos)
      {
        Vector3 pos = new Vector3(0, 0, _despawnZPosition);
        Vector3 size = new Vector3(10, 10, 0);

        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(pos, size);
        
        Gizmos.DrawLine(new Vector3(-size.x/2, -size.y/2, _despawnZPosition), new Vector3(size.x/2, size.y/2, _despawnZPosition));
        Gizmos.DrawLine(new Vector3(-size.x/2, size.y/2, _despawnZPosition), new Vector3(size.x/2, -size.y/2, _despawnZPosition));
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
