using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using State_Machine.GameStates;

namespace Controllers
{
  public class SpeedController : MonoBehaviour
  {
    [SerializeField, Tooltip("The current speed of the game [m/s]")] private float _speed;
    [SerializeField, Tooltip("The current speed of the game [m/s]")] private float _acceleration;
    
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
    public float TargetSpeed  { get; private set; }

    [Header("Speed Settings")]
    [SerializeField, Tooltip("The starting speed of the game [m/s]")] private float _startSpeed = 30f;
    [SerializeField, Tooltip("The max speed of the game [m/s]")] private float _maxSpeed = 70f;
    [Header("Time")]
    [SerializeField, Tooltip("The time to reach the max speed [s]")] private float _time = 300;

    private void Awake()
    {
      Speed = _startSpeed;
      TargetSpeed = Speed;
      Acceleration = 0f;
    }

    private void Start()
    {
      SetTargetSpeed(_maxSpeed, _time);   
    }

    public void SetTargetSpeed(float targetSpeed, float time)
    {
      TargetSpeed = targetSpeed;
      StartCoroutine(AccelerateToTargetSpeed(targetSpeed, time));
    }

    private IEnumerator AccelerateToTargetSpeed(float targetSpeed, float time)
    {
      Acceleration = (targetSpeed - Speed) / time;
      while (true)
      {
        if (GameManager.Instance.IsState<GameOverState>())
          yield break;
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
    }
  }
}

