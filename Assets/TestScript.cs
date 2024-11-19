using UnityEngine;
using WIP;

public class TestScript : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
       Debug.Log($"Speed: {Singelton.Instance.SpeedController.Speed}, Acceleration: {Singelton.Instance.SpeedController.Acceleration}");

       if (Input.GetMouseButtonDown(0))
       {
         float randomSpeed = Random.Range(30, 70);
         float time = 10f;
         Debug.Log($"Setting random speed to {randomSpeed} m/s");
         Singelton.Instance.SpeedController.SetTargetSpeed(randomSpeed, time);
       }
    }
}
