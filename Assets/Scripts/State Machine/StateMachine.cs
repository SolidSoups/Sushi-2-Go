using System.Collections.Generic;
using UnityEngine;

namespace State_Machine
{
    public class StateMachine : MonoBehaviour
    {
        public List<State> states = new List<State>();
        public State CurrentState = null;
    

        //Switching State
        public void SwitchState<aState>()
        {
            //going through all states, comparing if we have the state we want to change to
            foreach (State s in states)
            {
                if (s.GetType() == typeof(aState))
                {
                    //Switching the state, and running the exit and enter
                    CurrentState?.ExitState();
                    CurrentState = s;
                    CurrentState.EnterState();
                    return;
                }
            }

            //I return in the for loop once we find a state to change to, if we havent changed it means the state doen not exist
            Debug.LogWarning("State does not exist");
        }

        public virtual void UpdateStateMachine()
        {
            //Updating the current state
            CurrentState?.UpdateState();
        }

        public virtual void FixedUpdateStateMachine()
        {
            CurrentState?.FixedUpdateState();
        }

        public bool IsState<aState>()
        {
            if (!CurrentState) return false;
            return CurrentState.GetType() == typeof(aState);
        }
    }
}
