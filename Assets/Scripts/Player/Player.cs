using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    private CharacterController _player;
    private InputSystem_Actions _playerInput;
    private CameraLook _camera;

    [Space]
    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField] private float _runSpeed = 4f;
    [SerializeField] private float _crouchSpeed = 3f;
    [SerializeField] private float _jumpForce = 9f;
    [SerializeField] private float _crouchTransitionSpeed = 5f;
    [SerializeField] private float _gravity = -7f;

    private float _gravityAcceleration;

    private Vector2 _movementInput;
    private bool _isRunning;
    private bool _isCrouching;
    private bool _jumpPressed;

    private float _yVelocity;
    [HideInInspector] public bool running;
    [HideInInspector] public bool crouching;
    [HideInInspector] public bool walking;

    [Header("FootSteps")]
    private AudioSource _audioSource;
    public float runStepLength;
    public float walkStepLength;
    public float crouchStepLength;

    private float currentCrouchLenght;
    private float currentRunLenght;
    private float currentWalkLenght;

    private void Awake()
    {
        _player = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<CameraLook>();
        _audioSource = GetComponent<AudioSource>();

        _gravityAcceleration = _gravity * _gravity;
        _gravityAcceleration *= Time.deltaTime;
        
        _playerInput = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        // Enable Input Actions
        _playerInput.Player.Move.Enable();
        _playerInput.Player.Run.Enable();
        _playerInput.Player.Crouch.Enable();
        _playerInput.Player.Jump.Enable();

        // Bind Inputs
        _playerInput.Player.Move.performed += Move;
        _playerInput.Player.Move.canceled += Move;
        _playerInput.Player.Run.performed += ctx => _isRunning = true;
        _playerInput.Player.Run.canceled += ctx => _isRunning = false;
        _playerInput.Player.Crouch.performed += ctx => _isCrouching = true;
        _playerInput.Player.Crouch.canceled += ctx => _isCrouching = false;
        _playerInput.Player.Jump.performed += ctx => _jumpPressed = true;
    }

    private void OnDisable()
    {
        // Disable Input Actions
        _playerInput.Player.Move.Disable();
        _playerInput.Player.Run.Disable();
        _playerInput.Player.Crouch.Disable();
        _playerInput.Player.Jump.Disable();

        // Unbind Inputs
        _playerInput.Player.Move.performed -= Move;
        _playerInput.Player.Move.canceled -= Move;
        _playerInput.Player.Run.performed -= ctx => _isRunning = true;
        _playerInput.Player.Run.canceled -= ctx => _isRunning = false;
        _playerInput.Player.Crouch.performed -= ctx => _isCrouching = true;
        _playerInput.Player.Crouch.canceled -= ctx => _isCrouching = false;
        _playerInput.Player.Jump.performed -= ctx => _jumpPressed = true;
    }

    private void Move(InputAction.CallbackContext context)
    {
        _movementInput = context.ReadValue<Vector2>();
    }

    private void FixedUpdate()
    {
        HandleMovement();
        if (crouching)
        {
            if (currentCrouchLenght < crouchStepLength)
            {
                currentCrouchLenght += Time.deltaTime;
            }
            else
            {
                currentCrouchLenght = 0;
                AudioClip footstepSound = GetFootstepSound();
                if (footstepSound != null)
                {
                    _audioSource.PlayOneShot(footstepSound);
                }
            }
        }
        else if (running)
        {
            if (currentRunLenght < runStepLength)
            {
                currentRunLenght += Time.deltaTime;
            }
            else
            {
                currentRunLenght = 0;
                AudioClip footstepSound = GetFootstepSound();
                if (footstepSound != null)
                {
                    _audioSource.PlayOneShot(footstepSound);
                }
            }
        }
        else if (walking)
        {
            if (currentWalkLenght < walkStepLength)
            {
                currentWalkLenght += Time.deltaTime;
            }
            else
            {
                currentWalkLenght = 0;
                AudioClip footstepSound = GetFootstepSound();
                if (footstepSound != null)
                {
                    _audioSource.PlayOneShot(footstepSound);
                }
            }
        }
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = new Vector3(_movementInput.x, 0, _movementInput.y);

        // Determinar si el jugador se está moviendo
        bool isMoving = _movementInput.sqrMagnitude > 0;
        if (!isMoving)
        {
            running = false;
            crouching = false;
            walking = false;
        }
        else {
            running = false;
            crouching = false;
            walking = true;
        }
        float speed = _walkSpeed;
        if (_isRunning && isMoving && !_isCrouching) // Solo correr si se mueve
        {
            speed = _runSpeed;
            running = true;
            walking = false;
            crouching = false;
        }
        else if (_isCrouching)
        {
            running = false;
            crouching = true;
            walking = false;
            speed = _crouchSpeed;
        }

        moveDirection = transform.TransformDirection(moveDirection) * speed;

        // Aplicar transición de agachado
        HandleCrouch();

        // Salto y gravedad
        if (_player.isGrounded)
        {
            _yVelocity = 0;
            if (_jumpPressed)
            {
                _yVelocity = _jumpForce;
                _jumpPressed = false;
            }
        }
        else
        {
            _yVelocity -= _gravityAcceleration;
        }
        moveDirection.y = _yVelocity;

        _player.Move(moveDirection * Time.deltaTime);
    }

    private void HandleCrouch()
    {
        if (_isCrouching)
        {
            _camera.transform.localPosition = Vector3.Lerp(_camera.transform.localPosition, new Vector3(0, 1, 0), _crouchTransitionSpeed * Time.deltaTime);
            _player.height = Mathf.Lerp(_player.height, 1.2f, _crouchTransitionSpeed * Time.deltaTime);
            _player.center = Vector3.Lerp(_player.center, new Vector3(0, 0.6f, 0), _crouchTransitionSpeed * Time.deltaTime);
        }
        else
        {
            _camera.transform.localPosition = Vector3.Lerp(_camera.transform.localPosition, new Vector3(0, 2, 0), _crouchTransitionSpeed * Time.deltaTime);
            _player.height = Mathf.Lerp(_player.height, 2, _crouchTransitionSpeed * Time.deltaTime);
            _player.center = Vector3.Lerp(_player.center, new Vector3(0, 1, 0), _crouchTransitionSpeed * Time.deltaTime);
        }
    }

    public AudioClip GetFootstepSound()
    {
        RaycastHit hit;

        if (Physics.SphereCast(_player.center, 0.1f, Vector3.down, out hit, _player.bounds.extents.y))
        {
            Surface surface = hit.collider.GetComponent<Surface>();
            if (surface != null && surface.surface.footstepSounds.Length > 0)
            {
                return surface.surface.footstepSounds[Random.Range(0, surface.surface.footstepSounds.Length)];
            }
        }
        return null;
    }
}
