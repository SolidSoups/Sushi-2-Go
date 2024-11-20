using Controllers;
using UnityEngine;

namespace MySingelton
{
  public class Singelton : MonoBehaviour
  {
    public static Singelton Instance { get; private set; }
    
    // References
    public SpeedController SpeedController { get; private set; }

    private void Awake()
    {
      if (Instance != null && Instance != this)
      {
        Destroy(this);
        return;
      }

      Instance = this;
      InitializeReferences();
    }
    
    private void InitializeReferences()
    {
      SpeedController = GetComponentInChildren<SpeedController>(); 
    }
  }
  
}