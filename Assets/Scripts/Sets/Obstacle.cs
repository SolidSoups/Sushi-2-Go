using System;
using System.Linq;
using UnityEngine;

namespace Sets
{
    public enum GrabType
    {
        NONE,
        PLACED,
        GRABBED
    }

    public enum HandSide
    {
        LEFT,
        RIGHT
    }

    public class Obstacle : MonoBehaviour
    {
        private Collider[] _childColliders;
        private Collider myCollider;

        private void Awake()
        {
            _childColliders = GetComponentsInChildren<Collider>();  
            myCollider = GetComponent<Collider>();
        }
        
#if UNITY_EDITOR
        bool hasDebugged = false;
        private void OnDrawGizmos()
        {
            Obstacle[] children = GetComponentsInChildren<Obstacle>();
            if(children != null && children.Length > 1 && hasDebugged != true)
            {
                hasDebugged = true;
                Debug.LogError("Obstacles cannot contain children obstacle components. Please remove the component and try again.\n" +
                               "Objects causing problems:\n" + 
                               string.Join(",\n", children.Select(x => x.gameObject.name).ToArray()));
            }
        }
#endif

        public bool IsVisible
        {
            set
            {
                if(myCollider)
                    myCollider.enabled = value;
                foreach (Collider col in _childColliders)
                {
                    col.enabled = value;
                }
                foreach (Transform child in transform)
                {
                    child.gameObject.SetActive(value);
                }
            }
        }
        // nothing special here...    ¯\_(ツ)_/¯
        public GrabType GrabType = GrabType.NONE;
        public HandSide HandSide = HandSide.LEFT;
        public Set OwningSet;
        public Vector3 OriginalPosition;
        public Quaternion OriginalRotation;
        public Vector3 OriginalScale;

        public void ResetTransform()
        {
            this.transform.localPosition = OriginalPosition;
            this.transform.localRotation = OriginalRotation;
            this.transform.localScale = OriginalScale;
        }

        public Vector3 GetRealPosition() => OwningSet.transform.position + OriginalPosition;
        public bool IsInitialized;
    }
}