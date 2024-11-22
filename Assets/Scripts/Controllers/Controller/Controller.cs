using System;
using UnityEngine;

namespace Controllers.Controller
{
  public class Controller : Controllable
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

    public override void Initialize()
    {
      foreach (Controllable c in _controllables)
      {
        c.Initialize();
      }  
    }
    public override void DoUpdate()
    {
      foreach (Controllable c in _controllables)
      {
        c.DoUpdate();      
      }
    }
    public override void DoFixedUpdate()
    {
      foreach (Controllable c in _controllables)
      {
        c.DoFixedUpdate();      
      }
    }
  }
}