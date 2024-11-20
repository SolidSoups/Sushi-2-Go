using System.Collections.Generic;
using Hand;
using Sets;
using UnityEngine;


namespace Controllers
{
    /// <summary>
    /// List of sets
    /// SpawnSet(Prefab)
    /// Spawnpoint
    /// Can Spawn?()
    /// lastest spawned set
    /// Refrence mover
    /// </summary>
    public class SetSpawner : MonoBehaviour, IControllable
    {

    
        [Header("Settings")]
        [SerializeField] private float _lengthToSpawnPoint = 5f;

        private void OnValidate() => _lengthToSpawnPoint = Mathf.Clamp(_lengthToSpawnPoint, 0f, Mathf.Infinity);

        [Header("References")] 
        [SerializeField] private WorldMover _worldMover;
        [SerializeField] private MyObjectPool _setPool;

        [SerializeField] private string _handDelegatorTag = "Handdelegator";
        private HandDelegator _handDelegator;
    
    
    
    
        private Set _latestSpawnedSet;
        public Set LatestSpawnedSet => _latestSpawnedSet;
        private GameObject _setParent;

        public void Initialize()
        {
            _handDelegator = GameObject.FindGameObjectWithTag(_handDelegatorTag).GetComponent<HandDelegator>();
            _setParent = new GameObject("Set Parent");
            TryFindWorldMover();

            // load all sets
        }

        public void DoUpdate()
        {
            LoopSpawning();
        }
    
        public void DoFixedUpdate(){}

        // Test method
        private void LoopSpawning()
        {
            if (!_latestSpawnedSet)
            {
                SpawnRandomSet();
                return;
            }
        
            // check distance to spawnPoint
            float setZ = _latestSpawnedSet.transform.position.z + _latestSpawnedSet.Length;
            if (setZ <= _lengthToSpawnPoint)
                SpawnRandomSet();
        }

        private void SpawnRandomSet()
        {
            Set setToSpawn = _setPool.GetRandomSet();
            if(setToSpawn!=null)
                SpawnSet(setToSpawn);
        }

        private void SpawnSet(Set set)
        {
            float diff = 0f;
            if(_latestSpawnedSet != null)
                diff = (_latestSpawnedSet.transform.position.z + _latestSpawnedSet.Length) - _lengthToSpawnPoint;

            set.transform.position = _worldMover.AlignWithConveyor(_lengthToSpawnPoint) + Vector3.forward * diff;
            set.Initialize();
            set.CheckChildren();
        
            // delegate to hand if hand actions are available
            bool handsCanDelegate = false;
            if (set.ContainsHandActions)
            {
                List<Obstacle> placedObstacles = set.PlacedObstacles;
                if(placedObstacles.Count != 0)
                    handsCanDelegate = _handDelegator.DelegateToHands(placedObstacles);
            }

            if (!handsCanDelegate && set.ContainsHandActions && set.NeedsHands)
            {
                _setPool.AddBackToPool(set); 
                return;
            }
        
            set.transform.parent = _setParent.transform;
            _latestSpawnedSet = set;
            _worldMover.AddSet(_latestSpawnedSet);
        }

        private bool TryFindWorldMover()
        {
            WorldMover mover = WorldMover.Instance;
            if(!mover)
                mover = GameObject.FindGameObjectWithTag("WorldMover").GetComponent<WorldMover>();
            if(mover)
                _worldMover = mover;
            else
            {
                Debug.LogError("WorldMover could not be found");
                return false;
            }

            return true;
        }
    
        private void OnDrawGizmos()
        {
            if (!TryFindWorldMover())
                return;
        
            Gizmos.color = Color.green;
            float length = _worldMover.GetWidth();
            Vector3 startPosition = _worldMover.AlignWithConveyor(_lengthToSpawnPoint); 
            Vector3 endPosition = _worldMover.AlignWithConveyor(_lengthToSpawnPoint) + Vector3.right * length;
            Gizmos.DrawSphere(startPosition, 0.5f);
            Gizmos.DrawLine(startPosition, endPosition);
            Gizmos.DrawSphere(endPosition, 0.5f);
        }
    }
}
