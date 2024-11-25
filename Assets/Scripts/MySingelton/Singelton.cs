using Controllers;
using UnityEngine;
using UnityEngine.Pool;

namespace MySingelton
{
  public class Singelton : MonoBehaviour
  {
    public static Singelton Instance { get; private set; }
    
    // References
    public SpeedController SpeedController { get; private set; }
    public MyObjectPool ObjectPool { get; private set; }
    public DifficultyController DifficultyController { get; private set; }

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
      ObjectPool = GetComponentInChildren<MyObjectPool>();
      DifficultyController = GetComponentInChildren<DifficultyController>();
    }
  }
  
}