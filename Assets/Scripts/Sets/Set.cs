using System.Collections.Generic;
using Controllers;
using UnityEngine;
using UnityEngine.Serialization;

namespace Sets
{
    public class Set : MonoBehaviour
    {
        [Header("Gizmos")]
        [SerializeField] private bool _drawGizmos = true;

        [Header("Set settings")] 
        [Range(0f, 1f)] public float SpawnChance = 1f;
        public float Length = 4;
        private void OnValidate() => Length = Mathf.Clamp(Length, 0, Mathf.Infinity);
        [FormerlySerializedAs("_needsHands")] [Tooltip("Will not spawn if hands cannot grab/place")] public  bool NeedsHands = false;
    
    
    
        private Bounds _myBounds; // only for drawing

    
    
    

        private bool AlignWithConveyor()
        {
            ConveyorBelt convBelt = GameObject.FindGameObjectWithTag("ConveyorBelt").GetComponent<ConveyorBelt>();
            if (!convBelt)
                return false;
        
            transform.position = convBelt.AlignWithConveyor(transform.position.z);
            return true;
        }


        private ConveyorBelt TryGetConveyorBelt()
        {
            ConveyorBelt convBelt = GameObject.FindGameObjectWithTag("ConveyorBelt").GetComponent<ConveyorBelt>();
            if (!convBelt)
            {
                Debug.LogError("No conveyor belt component has been found");
                return null;
            }
            return convBelt;
        }

        private void GenerateMyBounds()
        {
            ConveyorBelt convBelt = TryGetConveyorBelt();
            if (!convBelt)
                return;
            Vector3 size = new Vector3(convBelt.ConveyorWidth, convBelt.ConveyorHeight, Length);
            Vector3 center = new Vector3(size.x / 2f, size.y / 2f, size.z / 2f);
            _myBounds = new Bounds(center + transform.position, size);
        }
    
        // Draws the gizmos of the set's aabb and conveyor lanes
        private void OnDrawGizmos()
        {
            if (!AlignWithConveyor())
                return;
            GenerateMyBounds();
        
            if (_drawGizmos && ContainsAllChildren())
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(_myBounds.center, _myBounds.size);
            
                int laneCount = TryGetConveyorBelt().NumberOfBelts;
                Vector3 size = new Vector3(_myBounds.size.x / laneCount, 0, _myBounds.size.z);
                Vector3 center = new Vector3(_myBounds.center.x - _myBounds.extents.x + size.x/2, _myBounds.center.y - _myBounds.extents.y, _myBounds.center.z);
                for (int i = 0; i < laneCount; i++)
                {
                    Vector3 newCenter = center + Vector3.right * i * size.x;
                    Gizmos.DrawWireCube(newCenter, size);
                }
            }
        }


        private Obstacle[] _children;
        private bool _isInitialized = false;
        public void Initialize()
        {
            if (_isInitialized)
                return;
            _isInitialized = true;
            _children = GetComponentsInChildren<Obstacle>();
        }

        private List<Obstacle> _placedObstacles;
        private bool _containsHandActions = false;
        public bool ContainsHandActions => _containsHandActions;
        public List<Obstacle> PlacedObstacles => _placedObstacles;
        public void CheckChildren()
        {
            foreach (Obstacle obstacle in _children)
            {
                if (!obstacle.IsInitialized)
                {
                    obstacle.OwningSet = this;
                    obstacle.OriginalPosition = obstacle.transform.localPosition; 
                    obstacle.OriginalRotation = obstacle.transform.localRotation;
                    obstacle.OriginalScale = obstacle.transform.localScale;
                    obstacle.IsInitialized = true;
                }
                obstacle.IsVisible = true;
                switch (obstacle.GrabType)
                {
                    case GrabType.NONE:
                        break;
                    case GrabType.PLACED:
                        _containsHandActions = true;
                        if(_placedObstacles == null)
                            _placedObstacles = new List<Obstacle>();
                        _placedObstacles.Add(obstacle);
                        break;
                    case GrabType.GRABBED:
                        _containsHandActions = true;
                        if(_placedObstacles == null)
                            _placedObstacles = new List<Obstacle>();
                        _placedObstacles.Add(obstacle);
                        break;
                } 
            }
        }

        private bool ContainsAllChildren()
        {
            return true;
            bool isFalse = false;
            Obstacle[] children = GetComponentsInChildren<Obstacle>();
            bool leftHandHasObstacle = false;
            bool rightHandHasObstacle = false;
            foreach (Obstacle child in children)
            {
                if (!_myBounds.Intersects(child.transform.GetComponentInChildren<Collider>().bounds))
                {
                    //Debug.LogError("Please make sure all children are contained in the set");
                    isFalse = true;
                }

                if (child.GrabType != GrabType.NONE)
                {
                    if (!leftHandHasObstacle && child.HandSide == HandSide.LEFT)
                    {
                        leftHandHasObstacle = true;
                    }
                    else if (!rightHandHasObstacle && child.HandSide == HandSide.RIGHT)
                    {
                        rightHandHasObstacle = true;
                    }
                    else
                    {
                        Debug.LogError("Only one obstacle per hand is allowed");
                    }
                } 
            }

            return !isFalse;
        }
    }
}



