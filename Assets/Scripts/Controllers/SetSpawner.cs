using System;
using System.Collections.Generic;
using Controllers.Controller;
using Hand;
using MySingelton;
using Sets;
using State_Machine.GameStates;
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
    public class SetSpawner : MonoBehaviour 
    {
        private SetMover _setMover;
        private HandDelegator _handDelegator;
        private ConveyorBelt _conveyorBelt;
        
        [Header("Settings")]
        [SerializeField] private float _lengthToSpawnPoint = 5f;
        private void OnValidate() => _lengthToSpawnPoint = Mathf.Clamp(_lengthToSpawnPoint, 0f, Mathf.Infinity);

        // cached variables 
        public Set LatestSpawnedSet { get; private set; }
        private GameObject _setParent;

        private void Awake()
        {
            _handDelegator = GameObject.FindGameObjectWithTag("Handdelegator").GetComponent<HandDelegator>();
            _setMover = GameObject.FindGameObjectWithTag("SetMover").GetComponent<SetMover>();
            _setParent = new GameObject("Set Parent");
            _conveyorBelt = GameObject.FindGameObjectWithTag("ConveyorBelt").GetComponent<ConveyorBelt>();
        }

        private void Update()
        {
            if (!GameManager.Instance.IsState<PlayingState>())
                return;
            
            LoopSpawning();
        }

        // Test method
        private void LoopSpawning()
        {
            if (!LatestSpawnedSet)
            {
                SpawnRandomSet();
                return;
            }
        
            // check distance to spawnPoint
            float setZ = LatestSpawnedSet.transform.position.z + LatestSpawnedSet.Length;
            if (setZ <= _lengthToSpawnPoint)
                SpawnRandomSet();
        }

        private void SpawnRandomSet()
        {
            Set setToSpawn = Singelton.Instance.ObjectPool.GetRandomSet();
            if(setToSpawn!=null)
                SpawnSet(setToSpawn);
        }

        private void SpawnSet(Set set)
        {
            float diff = 0f;
            if(LatestSpawnedSet != null)
                diff = (LatestSpawnedSet.transform.position.z + LatestSpawnedSet.Length) - _lengthToSpawnPoint;

            set.transform.position = _conveyorBelt.AlignWithConveyor(_lengthToSpawnPoint) + Vector3.forward * diff;
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
                Singelton.Instance.ObjectPool.AddBackToPool(set); 
                return;
            }
        
            set.transform.parent = _setParent.transform;
            LatestSpawnedSet = set;
            _setMover.AddSet(LatestSpawnedSet.gameObject);
        }

        private ConveyorBelt TryFindConveyorBelt()
        {
            ConveyorBelt belt = GameObject.FindGameObjectWithTag("ConveyorBelt").GetComponent<ConveyorBelt>();
            if(!belt)
            {
                Debug.LogError("Conveyor Belt could not be found");
                return null;
            }

            return belt;
        }
    
        private void OnDrawGizmos()
        {
            ConveyorBelt belt = TryFindConveyorBelt();
            if (!belt)
                return;
        
            Gizmos.color = Color.green;
            float length = belt.ConveyorWidth;
            Vector3 startPosition = belt.AlignWithConveyor(_lengthToSpawnPoint); 
            Vector3 endPosition = belt.AlignWithConveyor(_lengthToSpawnPoint) + Vector3.right * length;
            Gizmos.DrawSphere(startPosition, 0.5f);
            Gizmos.DrawLine(startPosition, endPosition);
            Gizmos.DrawSphere(endPosition, 0.5f);
        }
    }
}
