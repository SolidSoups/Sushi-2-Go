using System;
using System.Collections.Generic;
using System.Xml.Schema;
using Prototype.Scripts;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Serialization;

public class WorldMover : MonoBehaviour, IControllable
{
    public static WorldMover Instance;

    [Header("References")] [SerializeField]
    private MyObjectPool _setPool;
    
    [Header("Gizmos")] 
    [SerializeField] private bool _enableConveyorGizmos = true;
    [SerializeField] private bool _enableDespawnGizmos = true;

    [Header("Prefabs")] 
    [SerializeField] private GameObject _conveyorPrefab;
    [SerializeField] private int _conveyorLength = 10;
    [SerializeField] private int _conveyorCount = 3;
    
    [Header("Settings")]
    [SerializeField] private float _despawnZPosition = 5f;
    [SerializeField] private float _setHeight = 5f;
    
    // completely private fields
    private float _worldSpeed = 25f;
    public void SetWorldSpeed(float speed) => _worldSpeed = speed;
    private List<Set> _spawnedSets = new List<Set>();
    private List<GameObject> _conveyors = new List<GameObject>();
    private Bounds _conveyorBounds;
    private float _conveyorRespawnZ;
    private float[] _conveyorXPositions;
    private ObjectPool<Set> _pool;

    private GameObject _conveyorParent;
    
    // GETTERS
    public float[] GetConveyorXPositions() => _conveyorXPositions;
    public float GetWidth() => _conveyorBounds.size.x * _conveyorCount;
    public float GetHeight() => _setHeight;
    public int GetLaneCount() => _conveyorCount;

    public float GetConveyorHeight()
    {
        GenerateBounds();
        return _conveyorBounds.extents.y + _conveyorBounds.center.y;
    }

    public void Initialize()
    {
        Instance = this;
        _conveyorParent = new GameObject("Conveyor Parent"); 
        if (_conveyorPrefab)
        {
            _conveyorBounds = GenerateBounds();
            SpawnConveyors();
        }
        else
        {
            Debug.LogError("No conveyor prefab assigned to WorldMover");
        }
    }

    public void DoUpdate(){}
    public void DoFixedUpdate()
    {
        // TODO could be made into one big for loop for efficiency
        UpdateConveyor();
    }
    private void OnDrawGizmos()
    {
        _conveyorBounds = GenerateBounds();
        // draw despawn
        if (_enableDespawnGizmos)
        {
            Bounds boundsMin = GenerateBounds();
            Bounds boundsMax = GenerateBounds();
            boundsMin.center = boundsMin.center + new Vector3(0f, 0f, -1f * boundsMin.size.z);
            boundsMax.center = new Vector3(boundsMax.size.x * ( _conveyorCount -1), boundsMax.center.y, boundsMax.size.z * ( _conveyorLength -1)); 
            boundsMin.Encapsulate(boundsMax);

            Gizmos.color = Color.white;
            Gizmos.DrawWireCube(boundsMin.center, boundsMin.size);
            
            
            const float height = 20f;
            const int divisions = 20;
            float slitSize = height / divisions;
            Gizmos.color = Color.red;

            float centerX = _conveyorBounds.extents.x * (_conveyorCount - 1);

            Vector3 pos = new Vector3(centerX, (-height + slitSize)/2f, _despawnZPosition);
            Vector3 size = new Vector3(_conveyorBounds.size.x*_conveyorCount, slitSize, 0);
            for (int n = 0; n < divisions; n++)
            {
                Vector3 newPos = pos + Vector3.up * slitSize * n;
                Gizmos.DrawWireCube(newPos, size);
            }
        }
        
        if (_conveyorPrefab && _enableConveyorGizmos)
        {
            
        }
    }

    private void SpawnConveyors()
    {
        if (!_conveyorPrefab)
        {
            Debug.LogError("No conveyor prefab assigned");
            return;
        }
        
        _conveyorXPositions = new float[_conveyorCount];

        for (int n = 0; n < _conveyorCount; n++)
        {
            float newX = n * _conveyorBounds.size.x;
            _conveyorXPositions[n] = newX;
            for (int i = -1; i < _conveyorLength; i++)
            {
                float newZ = i * _conveyorBounds.size.z; 
                if(i == _conveyorLength-1)
                    _conveyorRespawnZ = newZ;
                GameObject newConveyor = Instantiate(
                    _conveyorPrefab, 
                    Vector3.zero + Vector3.forward * newZ + Vector3.right * newX, 
                    Quaternion.identity
                );
                _conveyors.Add(newConveyor);
                newConveyor.transform.parent = _conveyorParent.transform;
            }
        } 
    }
    public void AddSet(Set set) => _spawnedSets.Add(set);
    private void UpdateConveyor()
    {
        for(int i=_spawnedSets.Count-1; i >= 0; i--)
        {
            Set set = _spawnedSets[i];
            if (!set)
                continue;
            Vector3 position = set.transform.position;
            position.z -= _worldSpeed * GetCurrentTimeScale();
            set.transform.position = position;
            CheckDespawn(set);
        }

        foreach (GameObject conveyor in _conveyors)
        {
            Vector3 pos = conveyor.transform.position;

            pos.z -= _worldSpeed * GetCurrentTimeScale();
            float zFront = pos.z + _conveyorBounds.size.z * 0.5f;
            if (zFront <= _despawnZPosition)
            {
                // find difference
                float diff = zFront - _despawnZPosition;
                
                pos.z = _conveyorRespawnZ + diff;
            }
           
            conveyor.transform.position = pos;
        }
    }
    private void CheckDespawn(Set set)
    {
        float z = set.transform.position.z + set.Length;
        if (z <= _despawnZPosition)
        {
            Despawn(set);
        }
    }
    private void Despawn(Set set)
    {
        //Destroy(set.gameObject);
        
        _spawnedSets.Remove(set);
        _setPool.AddBackToPool(set); 
    }


    Bounds GenerateBounds()
    {
        
        Transform transform = _conveyorPrefab.transform;
        Vector3 center = Vector3.zero;
        foreach (Transform child in transform)
        {
            center += child.gameObject.GetComponent<Renderer>().bounds.center;
        }
        center /= transform.childCount;
        
        Bounds bounds = new Bounds(center, Vector3.zero);
        foreach (Transform child in transform)
        {
            bounds.Encapsulate(child.gameObject.GetComponent<Renderer>().bounds);
        }

        return bounds;
    }

    public Vector3 AlignWithConveyor(float z)
    {
        return new Vector3(-_conveyorBounds.extents.x, _conveyorBounds.extents.y, z); 
    }

    public float GetCurrentTimeScale() => Time.deltaTime * DifficultyController.Instance.DifficultyScale;

    public void SetPool(ObjectPool<Set> pool)
    {
        _pool = pool;
    }
}
    