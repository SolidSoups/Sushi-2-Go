using System;
using Controllers;
using Controllers.Controller;
using Events;
using MySingelton;
using State_Machine.GameStates;
using UnityEngine;

namespace Player
{
    public class PlayerMovementController : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Animator _animator;
    
        [Header("Movement settings")]
        [SerializeField] private float _playerSpeed = 3.5f;
        [SerializeField] float _jumpHeight = 4.5f;
        [SerializeField] float _gravityScale = 6f;
        private float _currentGravityScale;

        private DifficultyController _difficultyController;


        // private fields
        //private List<InputList> inputBuffer = new List<InputList>(); //Input buffer
        private float[] _lanePositions;
        private bool _isMoving;
        private bool _isJumping;
        private bool _isGrounded = true;



        float jumpVelocity;

        [Header("Events")]
        public GameEvent OnPlayerJump;

        public GameEvent OnPlaySound;

        [Header("Sounds")]
        [SerializeField] private string[] _jumpSounds;

        // private properties
        private int _targetLane = 1;
        private int TargetLane
        {
            get => _targetLane;
            set => _targetLane = Mathf.Clamp(value, 0, _lanePositions.Length - 1);
        }

        private MoveType _movementDirection;

        private enum MoveType
        {
            MoveLeft,
            MoveRight
        }

        private void Awake()
        {
           _currentGravityScale = _gravityScale;
        }

        private void Start()
        {
            _difficultyController = Singelton.Instance.DifficultyController;
        }

        public bool IsInitialized  { get; private set; }
        public void Initialize(Component sender, object data)
        {
            if (sender is not ConveyorBelt || data is not float[])
                return;
            
            _lanePositions = (float[])data;
            
            IsInitialized = true;
        } 

        private void Update()
        {
            if (!IsInitialized || !GameManager.Instance.IsState<PlayingState>())
                return;

            _currentGravityScale = _gravityScale * _difficultyController.SpeedScale;
            _animator.speed = _difficultyController.SpeedScale;
        
            GetHorizontalInput();
            GetVerticalInput();
        
            DoJump();
            MoveToLane();

            CheckHasJumped();
            CheckXPositionToLane();
        }

      


        private void CheckXPositionToLane()
        {
            float x = transform.position.x; 
            float targetX = _lanePositions[TargetLane];

            if (_movementDirection == MoveType.MoveLeft && x < targetX ||
                _movementDirection == MoveType.MoveRight && x > targetX)
            {
                Vector3 pos = transform.position;
                pos.x = _lanePositions[TargetLane];
                transform.position = pos;
                _isMoving = false;
            }
        }

        private void MoveToLane()
        {
            //TODO will snap to the xposition when close enough
            if (!_isMoving)
                return;
               
            float targetX = _lanePositions[TargetLane];
            Vector3 position = transform.position;
            float normVector = Mathf.Sign(targetX - position.x);
            position.x += normVector * _playerSpeed *10f* Time.deltaTime;
            transform.position = position;
        }

        private void GetHorizontalInput()
        {
            //if (_isMoving)
            //return;

            if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
            {
                //TODO Will save the key to the input buffer 
                if (TargetLane - 1 >= 0)
                {
                    _isMoving = true;
                    TargetLane--;
                    _movementDirection = MoveType.MoveLeft;
                } 
            }
            else if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
            {
                //TODO Will save the key to the input buffer
                if (TargetLane + 1 < _lanePositions.Length)
                {
                    _isMoving = true;
                    TargetLane++;
                    _movementDirection = MoveType.MoveRight;
                }
            }
        }
    
        private void GetVerticalInput()
        {
            if(_isJumping || !_isGrounded) return;

            int vertical = (int)Input.GetAxisRaw("Vertical");
            if (Input.GetKey(KeyCode.Space))
                vertical = 1;
            if ( vertical == 0) return;
            _isJumping = false;

            if (vertical == 1 && _isGrounded)
            {
                _isJumping = true;
                _isGrounded = false;
                _animator.SetTrigger("DoJump");
                _animator.SetBool("IsJumping", true);
                _animator.SetBool("IsGoingDown", false);
                _animator.SetBool("HasLanded", false);
                int randomIndex = UnityEngine.Random.Range(0, _jumpSounds.Length);
                OnPlayerJump?.Raise(this, null);
                OnPlaySound?.Raise(this, _jumpSounds[randomIndex]);
            }

            
        }


        private void DoJump()
        {
        
            if(!_isJumping) return;
            // if(!_isGrounded) return;



            if (jumpVelocity == 0)
            {
                jumpVelocity = Mathf.Sqrt(_jumpHeight * -2 * (Physics.gravity.y * _currentGravityScale));
            }

            transform.Translate(new Vector3(0, jumpVelocity, 0) * (Time.deltaTime));
            jumpVelocity += Physics.gravity.y * _currentGravityScale * Time.deltaTime;
        
            if(jumpVelocity >= 0)
            {
                _isJumping = false;
            }

        }

        private void CheckHasJumped()
        {

            if (transform.position.y <= 0) 
            {
                transform.position = new Vector3(transform.position.x, 0, transform.position.z);
                jumpVelocity = 0;
                _isGrounded = true;
                _animator.SetBool("HasLanded", true);
                _animator.SetBool("IsJumping", false);
            }
            else
            {
                jumpVelocity += Physics.gravity.y * _currentGravityScale * Time.deltaTime;
                transform.Translate(new Vector3(0, jumpVelocity, 0) * Time.deltaTime);
                _animator.SetBool("IsGoingDown", true);
            }
        
        }
    
    }
}
