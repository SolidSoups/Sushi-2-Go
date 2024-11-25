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

    public override void InitializeController()
    {
      foreach (Controllable c in _controllables)
      {
        c.InitializeController();
      }  
    }
    public override void UpdateController()
    {
      foreach (Controllable c in _controllables)
      {
        c.UpdateController();      
      }
    }
    public override void FixedUpdateController()
    {
      foreach (Controllable c in _controllables)
      {
        c.FixedUpdateController();      
      }
    }
  }
}