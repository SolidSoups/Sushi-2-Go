using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using Controllers.Controller;
using MySingelton;
using Sets;
using UnityEngine;

namespace Controllers
{
  public class SetMover : Controllable
  {
    private TurnAnimator _turnAnimator;
    public Transform turnStartAPosition;
    public Transform turnEndAPosition;
    
    #region Sets
    private readonly List<Set> _spawnedSets = new();
    public void AddSet(Set set)
    {
      _spawnedSets.Add(set);
      _turnAnimator.AddSet(set);
      _turnAnimator.AnimateAll();
    }

    private GameObject _setParent;
    #endregion
    
    #region Gizmos
    [Header("Gizmo Settings")] 
    [SerializeField] private bool _drawGizmos = true;
    [SerializeField] private float _despawnZPosition = 5f;
    public Transform turnStartBPosition;
    public Transform turnEndBPosition;

    private void OnDrawGizmos()
    {
      if (_drawGizmos)
      {
        //GizmosDrawSharpTurn();
        DrawTurnBounds();
        GizmosDrawArcTurn();
        GizmosDrawDespawn();
      }      
    }

    private void DrawTurnBounds()
    {
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
    }

    private void GizmosDrawArcTurn()
    {
      Vector3 startA = turnStartAPosition.position;
      Vector3 startB = turnStartBPosition.position;
      Vector3 endA = turnEndAPosition.position;
      Vector3 endB = turnEndBPosition.position;

      // precalculate offsets
      Vector3 displacement = endB - endA;
      Vector3 dispD3 = displacement / 3f;

      float radius(float x0, float x1) => x0 - x1;
      const int segments = 20;
      for (int i = 0; i < 3; i++)
      {
        float xi = endA.x + dispD3.x / 2f + dispD3.x * i;
        float rad = radius(startA.x, xi);
        Vector3[] points = new Vector3[segments + 1];
        for (int n = 0; n <= segments; n++)
        {
          float angle = ((float)n / segments) * Mathf.PI * 0.5f;
          float newX = rad * (float)Math.Cos(angle);
          float newY = rad * (float)Math.Sin(angle);

          points[n] = new Vector3(startA.x - newX, endA.y, endA.z - newY);
        }

        Gizmos.color = Color.yellow;
        for (int n = 1; n < points.Length; n++)
        {
          Gizmos.DrawLine(points[n - 1], points[n]);
        }
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

    #endregion
    
    public override void InitializeController()
    {
      base.InitializeController();
    
      _setParent = new GameObject("SetParent");   
      _setParent.transform.parent = transform;
      _turnAnimator = new TurnAnimator(this);
    }

    public override void FixedUpdateController()
    {
      base.FixedUpdateController();
      MoveSets();
      _turnAnimator.AnimateAll();
    }

    private void MoveSets()
    {
      float currentSpeed = Singelton.Instance.SpeedController.Speed;
      for (int i = _spawnedSets.Count - 1; i >= 0; i--)
      {
        Set set = _spawnedSets[i];
        if (!set)
          continue;
        Vector3 pos = set.transform.position;
        pos.z -= currentSpeed * Time.deltaTime;
        set.transform.position = pos;
      
        // check despawn
        float z = pos.z + set.Length;
        if (z <= _despawnZPosition)
        {
          Despawn(set);
        }
      } 
    }

    private void Despawn(Set set)
    {
      _spawnedSets.Remove(set);
      // should be adding back to pool here 
      Singelton.Instance.ObjectPool.AddBackToPool(set);
    }
  }
}
