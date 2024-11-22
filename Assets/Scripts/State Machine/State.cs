using Controllers.Controller;
using UnityEngine;
using UnityEngine.Serialization;

namespace State_Machine
{
    public class State : MonoBehaviour
    {
        [Header("Game loop")]
        [SerializeField] private Controllable[] _initializers;
        [SerializeField] private Controllable[] _updaters;
        [FormerlySerializedAs("_FixedUpdaters")] [SerializeField] private Controllable[] _fixedUpdaters;
        
       
        public virtual void EnterState()
        {
            foreach (Controllable c in _initializers)
            {
                c.Initialize();
            }
        }

        public virtual void ExitState()
        {

        }

        public virtual void UpdateState()
        {
            foreach (Controllable c in _updaters)
            {
                c.DoUpdate();
            }
        }

        public virtual void FixedUpdateState()
        {
            foreach (Controllable c in _fixedUpdaters)
            {
                c.DoFixedUpdate();
            }
        }
    }
}
