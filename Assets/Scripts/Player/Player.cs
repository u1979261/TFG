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

    private void Update()
    {
        if (crouching)
        {
            if (currentCrouchLenght < crouchStepLength)
            {
                currentCrouchLenght += Time.deltaTime;
            }
            else
            {
                currentCrouchLenght = 0;
                _audioSource.PlayOneShot(GetFootstepSound());
                
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
                _audioSource.PlayOneShot(GetFootstepSound());

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
                _audioSource.PlayOneShot(GetFootstepSound());

            }
        }
    }
    private void FixedUpdate()
    {
        HandleMovement();
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
        else if (_isCrouching && isMoving)
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
        if (_player == null) return null;

        RaycastHit hit;

        // Primero, un Raycast para máxima precisión
        if (Physics.Raycast(_player.transform.position, Vector3.down, out hit, _player.bounds.extents.y + 0.3f) ||
            Physics.SphereCast(_player.transform.position, 0.2f, Vector3.down, out hit, _player.bounds.extents.y + 0.3f))
        {
            if (hit.transform.TryGetComponent(out Surface surface) &&
                surface.surface.footstepSounds != null &&
                surface.surface.footstepSounds.Length > 0)
            {
                int i = Random.Range(0, surface.surface.footstepSounds.Length);
                return surface.surface.footstepSounds[i];
            }
        }
        return null;
    }

}
