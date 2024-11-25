using UnityEngine;

namespace Controllers.Controller
{
  public class Controllable : MonoBehaviour
  {
    public virtual void InitializeController(){}

    public virtual void UpdateController(){}

    public virtual void FixedUpdateController(){}
    
  }
}