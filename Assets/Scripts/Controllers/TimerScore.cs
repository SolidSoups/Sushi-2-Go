using System;
using Prototype.Scripts;
using UnityEngine;

public class TimerScore : MonoBehaviour, IControllable
{
    [SerializeField] private float _pointsPerSecond;

    private float _timerScore;
    public int Score => ((int)(_timerScore / 10f)) * 10;


    public void Initialize() => _timerScore = 0;

    public void DoFixedUpdate()
    {
        
    }
    
    public void DoUpdate()
    {
        _timerScore += (_pointsPerSecond * DifficultyController.Instance.DifficultyScale) * Time.deltaTime;
    }
}
