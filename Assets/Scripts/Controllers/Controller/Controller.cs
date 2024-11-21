using System;
using UnityEngine;

namespace Controllers.Controller
{
  public class Controller : MonoBehaviour
  {
    [SerializeField] private Controllable[] _controllables;

    public void DisableControllables()
    {
      foreach (Controllable c in _controllables)
      {
        c.enabled = false;
      }
    }
    public void EnableControllables()
    {
      foreach (Controllable c in _controllables)
      {
        c.enabled = false;
      }
    }

    public virtual void Initialize()
    {
      foreach (Controllable c in _controllables)
      {
        c.Initialize();
      }  
    }
    public virtual void DoUpdate()
    {
      foreach (Controllable c in _controllables)
      {
        c.DoUpdate();      
      }
    }
    public virtual void DoFixedUpdate()
    {
      foreach (Controllable c in _controllables)
      {
        c.DoFixedUpdate();      
      }
    }
    public virtual void DoLateUpdate()
    {
      foreach (Controllable c in _controllables)
      {
        c.DoLateUpdate();      
      }
    }
  }
}