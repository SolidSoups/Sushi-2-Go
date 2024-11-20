using UnityEngine;

namespace Controllers
{
    public class DifficultyController : MonoBehaviour, IControllable
    {
        public static DifficultyController Instance;
    
        [Header("Settings")] 
        [SerializeField] private float _startingSpeed = 25f;
        [SerializeField] private float _targetSpeed = 50f;
        [SerializeField] private float _timeToTargetInSeconds = 60*10;
    
        private float _currentSpeed;
        public float WorldSpeed => _currentSpeed;
        public float Acceleration => _scalingPerSecond;
        public float DifficultyScale => _currentSpeed / 25f;
        public float Change => ( _currentSpeed -25f) / ( _targetSpeed -25f);

        private float _scalingPerSecond;

        private void Awake()
        {
            Instance = this;
        }

        public void Initialize()
        {
            _currentSpeed = _startingSpeed;
            WorldMover.Instance.SetWorldSpeed(_startingSpeed);
            _scalingPerSecond = (_targetSpeed - _currentSpeed) / _timeToTargetInSeconds;
        }
    
        public void DoUpdate()
        {
            _currentSpeed += Time.deltaTime * _scalingPerSecond; 
            _currentSpeed = Mathf.Clamp(_currentSpeed, _startingSpeed, _targetSpeed);
        }

        public void DoFixedUpdate()
        {
        
        }
    }
}
