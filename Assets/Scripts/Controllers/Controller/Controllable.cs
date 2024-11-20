using UnityEngine;

namespace Controllers.Controller
{
  public class Controllable : MonoBehaviour
  {
    public virtual void Initialize(){}

    public virtual void DoUpdate(){}

    public virtual void DoFixedUpdate(){}
    
    public virtual void DoLateUpdate(){}
  }
}