using Controllers;
using Controllers.Controller;
using MySingelton;
using UnityEngine;

public class ParticleEffectController : MonoBehaviour
{
    [Header("References")]
    private ParticleSystem _particleSystem;
    private ParticleSystem.MainModule _particleSystemMainModule;
    private ParticleSystem.EmissionModule _particleSystemEmissionModule;
    
    [Header("Settings")]
    [SerializeField] private float _startSpeed = 4;
    [SerializeField] private float _endSpeed = 45;
    [SerializeField] private float _startRate = 2f;
    [SerializeField] private float _endRate = 20f;

    private float _CurrentSpeed
    {
        set
        {
            _particleSystemMainModule.startSpeed = value;
        }
    }

    private float _CurrentRate
    {
        set
        {
            _particleSystemEmissionModule.rateOverTime = value;
        } 
    }

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystemMainModule = _particleSystem.main;
        _particleSystemEmissionModule = _particleSystem.emission;
        _CurrentSpeed = _startSpeed;
        _CurrentRate = _startRate; 
    }

    private void Update()
    {
        _CurrentSpeed = _startSpeed + (_endSpeed - _startSpeed) * Singelton.Instance.DifficultyController.Change;
        _CurrentRate = _startRate + (_endRate - _startRate) * Singelton.Instance.DifficultyController.Change;
    }
}
