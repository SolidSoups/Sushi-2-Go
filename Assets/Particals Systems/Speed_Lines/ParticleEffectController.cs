using Controllers;
using UnityEngine;

public class ParticleEffectController : MonoBehaviour, IControllable
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

    public void Initialize()
    {
        _particleSystem = GetComponent<ParticleSystem>();
        _particleSystemMainModule = _particleSystem.main;
        _particleSystemEmissionModule = _particleSystem.emission;
        _CurrentSpeed = _startSpeed;
        _CurrentRate = _startRate; 
    }

    public void DoUpdate()
    {
        _CurrentSpeed = _startSpeed + (_endSpeed - _startSpeed) * DifficultyController.Instance.Change;
        _CurrentRate = _startRate + (_endRate - _startRate) * DifficultyController.Instance.Change;
    }

    public void DoFixedUpdate()
    {
        
    }
}
