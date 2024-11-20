using Controllers;
using Events;
using UnityEngine;

namespace Player
{
    public class PlayerMovementController : MonoBehaviour, IControllable
    {
        [Header("References")] 
        [SerializeField] private Animator _animator;
    
        [Header("Movement settings")]
        [SerializeField] private float _playerSpeed = 3.5f;
        [SerializeField] float _jumpHeight = 4.5f;
        [SerializeField] float _gravityScale = 6f;
        [SerializeField] float _difficultyDivider = 20f;
        private float _currentGravityScale;
        private float _currentPlayerSpeed;



        // private fields
        //private List<InputList> inputBuffer = new List<InputList>(); //Input buffer
        private float[] _lanePositions;
        private bool _isMoving = false;
        private bool _isJumping = false;
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
    
        public void Initialize()
        {
            ConveyorBelt convBelt = GameObject.FindGameObjectWithTag("ConveyorBelt").GetComponent<ConveyorBelt>(); 
            _lanePositions = convBelt.XPositions;
            _currentGravityScale = _gravityScale;
            _currentPlayerSpeed = _playerSpeed;
        }

        public void DoFixedUpdate()
        {
        }
    
        public void DoUpdate()
        {
            _currentGravityScale = _gravityScale; // * DifficultyController.Instance.DifficultyScale;
            _currentPlayerSpeed = _playerSpeed; // * DifficultyController.Instance.DifficultyScale;
            //_animator.speed = DifficultyController.Instance.DifficultyScale;
        
            GetHorizontalInput();
            GetVerticalInput();
        
            DoJump();
            MoveToLane();

            CheckHasJumped();
            CheckXPositionToLane();
        }

        //TODO will execute the buffer order in an que style.
        private void InputBufferCheck()
        {

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
            if(_isJumping) return;

            int vertical = (int)Input.GetAxisRaw("Vertical");
            if ( vertical == 0) return;
            _isJumping = false;

            if (vertical == 1 && _isGrounded)
            {
                _isJumping = true;
                _isGrounded = false;
                _animator.SetTrigger("DoJump");
                _animator.SetBool("HasLanded", false);
                int randomIndex = UnityEngine.Random.Range(0, _jumpSounds.Length);
                OnPlayerJump?.Raise(this, null);
                OnPlaySound?.Raise(this, _jumpSounds[randomIndex]);
                _maxHeight = float.MinValue;
            }

            
        }

    
        float _maxHeight = float.MinValue;
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
                _animator.SetBool("IsGoingDown", false);
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
