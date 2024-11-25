using Controllers.Controller;
using UnityEngine;
using UnityEngine.Serialization;

namespace State_Machine
{
    public class State : MonoBehaviour
    {
        public virtual void EnterState()
        {
        }

        public virtual void ExitState()
        {
        }

        public virtual void UpdateState()
        {
        }

        public virtual void FixedUpdateState()
        {
        }
    }
}
