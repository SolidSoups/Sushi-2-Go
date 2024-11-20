using System;
using System.Collections.Generic;
using Controllers.Controller;
using MySingelton;
using UnityEngine;

namespace Controllers
{
  public class ConveyorBelt : Controllable
  {
    [Header("Conveyor Settings")] [SerializeField]
    private bool _drawGizmos = true;
    [SerializeField] private GameObject _conveyorPrefab;
    [SerializeField] private int _conveyorLength = 10;
    [SerializeField] private int _numberOfBelts = 3;
    [SerializeField] private float _moveZPosition;

    private void OnValidate()
    {
      Bounds currentBounds = GenerateConveyorBounds();
      _moveZPosition = (Mathf.RoundToInt(_moveZPosition / currentBounds.size.z) * currentBounds.size.z);
    }

    private Bounds _conveyorBounds;
    private float[] _xPositions;
    private float _conveyorRespawnZ;

    private List<GameObject> _conveyors = new List<GameObject>();
    private GameObject _conveyorParent;

    private void OnDrawGizmos()
    {
      if (_drawGizmos)
      {
        Vector3 pos = new Vector3(0, 0, _moveZPosition);
        Vector3 size = new Vector3(10, 10, 0);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(pos, size);
        
        Gizmos.DrawLine(new Vector3(-size.x/2, -size.y/2, _moveZPosition), new Vector3(size.x/2, size.y/2, _moveZPosition));
        Gizmos.DrawLine(new Vector3(-size.x/2, size.y/2, _moveZPosition), new Vector3(size.x/2, -size.y/2, _moveZPosition));
      }      
    }

    public override void Initialize()
    {
      base.Initialize();
      
      // spawn conveyor belts
      _conveyorBounds = GenerateConveyorBounds();
      _conveyorParent = new GameObject("ConveyorParent");
      _conveyorParent.transform.parent = transform;
      SpawnConveyors();
      GenerateXPositions();
    }

    public override void DoFixedUpdate()
    {
      base.DoFixedUpdate();

      MoveConveyors();
    }

    private void MoveConveyors()
    {
      float currentSpeed = Singelton.Instance.SpeedController.Speed;
      foreach (GameObject conveyor in _conveyors)
      {
        Vector3 pos = conveyor.transform.position;
        
        pos.z -= currentSpeed * Time.deltaTime;
        float zFront = pos.z + _conveyorBounds.size.z * 0.5f;
        if (zFront <= _moveZPosition)
        {
          float diff = zFront - _moveZPosition; // find small diff
          pos.z = _conveyorRespawnZ + diff;
        }
         conveyor.transform.position = pos;
      }
    }

    private void SpawnConveyors()
    {
      if (!_conveyorPrefab)
      {
        Debug.LogError("No conveyor prefab assigned");
        return;
      }

      for (int i = -1; i < _conveyorLength; i++)
      {
        float newZ = i * _conveyorBounds.size.x;
        if (i == _conveyorLength - 1)
        {
          _conveyorRespawnZ = newZ;          
        }

        GameObject newConveyor = Instantiate(
          _conveyorPrefab,
          Vector3.zero + Vector3.forward * newZ,
          Quaternion.identity
        );
        _conveyors.Add(newConveyor);        
        newConveyor.transform.parent = _conveyorParent.transform;
      }
    }

    private void GenerateXPositions()
    {
      _xPositions = new float[_numberOfBelts];
      for (int i = 0; i < _numberOfBelts; i++)
      {
        _xPositions[i] = i * _conveyorBounds.size.x; 
      }      
    }

    private Bounds GenerateConveyorBounds()
    {
      if (!_conveyorPrefab)
      {
        return new Bounds(Vector3.zero, Vector3.one);
      }
      
      Transform t = _conveyorPrefab.transform;
      Vector3 center = Vector3.zero;
      foreach (Transform child in t)
      {
        center += child.position;
      }
      center /= transform.childCount;
      
      Bounds bounds = new Bounds(center, Vector3.zero);
      foreach (Transform child in t)
      {
        bounds.Encapsulate(child.gameObject.GetComponent<Renderer>().bounds);
      }
      return bounds;
    }
  }
}
