using System;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class Player : MonoBehaviour
{
    private CharacterController _player;
    private WindowHandler _windowHandler;
    private InputSystem_Actions _playerInput;
    private CameraLook _camera;

    [Space]
    [SerializeField] private float _walkSpeed = 2f;
    [SerializeField] private float _runSpeed = 4f;
    [SerializeField] private float _crouchSpeed = 3f;
    [SerializeField] private float _jumpForce = 9f;
    [SerializeField] private float _crouchTransitionSpeed = 5f;
    [SerializeField] private float _gravity = -7f;

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

    private void Start()
    {
        _windowHandler = GetComponent<WindowHandler>();
    }
    private void Awake()
    {
        _player = GetComponent<CharacterController>();
        _camera = GetComponentInChildren<CameraLook>();
        _audioSource = GetComponent<AudioSource>();
        _playerInput = new InputSystem_Actions();
    }

    private void OnDestroy()
    {
        if (_playerInput != null)
        {
            _playerInput.Dispose();
            _playerInput = null;
        }
    }

    private void OnEnable()
    {
        _playerInput.Player.Move.Enable();
        _playerInput.Player.Run.Enable();
        _playerInput.Player.Crouch.Enable();
        _playerInput.Player.Jump.Enable();

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
        _playerInput.Player.Move.Disable();
        _playerInput.Player.Run.Disable();
        _playerInput.Player.Crouch.Disable();
        _playerInput.Player.Jump.Disable();

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
        Debug.Log(GetComponent<PlayerStats>().health);
        if (GetComponent<PlayerStats>().health <= 0)
        {
            if (!GetComponent<PlayerStats>().isDead)
            {
                Die();
            }
            return;
        }
        if (crouching)
        {
            if (currentCrouchLenght < crouchStepLength)
                currentCrouchLenght += Time.deltaTime;
            else
            {
                currentCrouchLenght = 0;
                _audioSource.PlayOneShot(GetFootstepSound());
            }
        }
        else if (walking)
        {
            if (currentWalkLenght < walkStepLength)
                currentWalkLenght += Time.deltaTime;
            else
            {
                currentWalkLenght = 0;
                _audioSource.PlayOneShot(GetFootstepSound());
            }
        }
        else if (running)
        {
            if (currentRunLenght < runStepLength)
                currentRunLenght += Time.deltaTime;
            else
            {
                currentRunLenght = 0;
                _audioSource.PlayOneShot(GetFootstepSound());
            }
        }
    }
    private void Die()
    {
        for (int i = 0; i < _windowHandler.inventory.inventorySlots.Length; i++)
        {
            _windowHandler.inventory.inventorySlots[i].Drop();
        }
        GetComponent<PlayerStats>().isDead = true;
        GetComponent<PlayerStats>().health = 100;
        GetComponent<PlayerStats>().thirst = 100;
        GetComponent<PlayerStats>().hunger = 100;

    }
    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 moveDirection = new Vector3(_movementInput.x, 0, _movementInput.y);
        bool isMoving = _movementInput.sqrMagnitude > 0;

        running = crouching = walking = false;

        float speed = _walkSpeed;

        if (_isRunning && isMoving && !_isCrouching)
        {
            speed = _runSpeed;
            running = true;
        }
        else if (_isCrouching && isMoving)
        {
            speed = _crouchSpeed;
            crouching = true;
        }
        else if (isMoving)
        {
            walking = true;
        }

        moveDirection = transform.TransformDirection(moveDirection) * speed;

        HandleCrouch();

        // Gravedad y salto correctamente aplicados por frame  
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
            _yVelocity += _gravity * Time.fixedDeltaTime;
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
        if (Physics.Raycast(_player.transform.position, Vector3.down, out hit, _player.bounds.extents.y + 0.3f) ||
            Physics.SphereCast(_player.transform.position, 0.2f, Vector3.down, out hit, _player.bounds.extents.y + 0.3f))
        {
            if (hit.transform.TryGetComponent(out Surface surface) &&
                surface.surface.footstepSounds != null &&
                surface.surface.footstepSounds.Length > 0)
            {
                int i = UnityEngine.Random.Range(0, surface.surface.footstepSounds.Length);
                return surface.surface.footstepSounds[i];
            }
        }

        return null;
    }


}
