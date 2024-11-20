using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Attributes;

namespace Controllers
{
  public class SpeedController : MonoBehaviour
  {
    [SerializeField, Attributes.ReadOnly, Tooltip("The current speed of the game [m/s]")] private float _speed;
    [SerializeField, Attributes.ReadOnly, Tooltip("The current speed of the game [m/s]")] private float _acceleration;
    
    public float Speed
    {
      get => _speed;
      private set => _speed = Mathf.Clamp(value, 0, _maxSpeed);
    }
    public float Acceleration
    {
      get => _acceleration;
      private set => _acceleration = value;
    }

    [Header("Speed Settings")]
    [SerializeField, Tooltip("The starting speed of the game [m/s]")] private float _startSpeed = 30f;
    [SerializeField, Tooltip("The max speed of the game [m/s]")] private float _maxSpeed = 70f;
    
    [Header("Acceleration Settings")]
    [SerializeField, Tooltip("The min acceleration of the game [m/s]")] private float _minAcceleration = 0;
    [SerializeField, Tooltip("The max acceleration of the game [m/s]")] private float _maxAcceleration = 10;

    private bool _isAccelerating = false;
    
    private void Awake()
    {
      Speed = _startSpeed;  
      Acceleration = 0f;
    }

    public void SetTargetSpeed(float targetSpeed, float time) => StartCoroutine(AccelerateToTargetSpeed(targetSpeed, time));

    private IEnumerator AccelerateToTargetSpeed(float targetSpeed, float time)
    {
      Acceleration = (targetSpeed - Speed) / time;
      _isAccelerating = true;
      while (true)
      {
        yield return null;
        Speed += Acceleration * Time.deltaTime;
        
        if (float.IsNegative(Acceleration) && Speed <= targetSpeed)
        {
          Speed = targetSpeed;
          break;
        }

        if (!float.IsNegative(Acceleration) && Speed >= targetSpeed)
        {
          Speed = targetSpeed;
          break;
        }
      }
      _isAccelerating = false;
    }
  }
}

