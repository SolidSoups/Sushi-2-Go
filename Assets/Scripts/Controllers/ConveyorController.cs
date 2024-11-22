using System;
using Controllers;
using Controllers.Controller;
using State_Machine;
using State_Machine.GameStates;
using UnityEditor.VersionControl;
using UnityEngine;

public class ConveyorController : Controller
{
  public override void Initialize()
  {
    base.Initialize();
    EnableControllables();
  }
}
