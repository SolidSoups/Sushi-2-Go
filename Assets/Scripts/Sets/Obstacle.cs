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
        public bool IsInitialized;
    }
}