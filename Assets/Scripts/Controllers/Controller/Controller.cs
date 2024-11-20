using System;
using UnityEngine;

namespace Controllers.Controller
{
  public class Controller : MonoBehaviour
  {
    [SerializeField] private Controllable[] _controllables;
    protected virtual void Awake()
    {
      foreach (Controllable c in _controllables)
      {
        c.Initialize();
      }  
    }
    protected virtual void Update()
    {
      foreach (Controllable c in _controllables)
      {
        c.DoUpdate();      
      }
    }
    protected virtual void FixedUpdate()
    {
      foreach (Controllable c in _controllables)
      {
        c.DoFixedUpdate();      
      }
    }
    protected virtual void LateUpdate()
    {
      foreach (Controllable c in _controllables)
      {
        c.DoLateUpdate();      
      }
    }
  }
}