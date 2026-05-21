using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyCarController : MonoBehaviour, IDamageable
{
    [Header("References")]
    [SerializeField] private Transform _playerCar;
    [SerializeField] private Transform _waypointsParent;
    [SerializeField] private Transform[] _wheels;

    [Header("Movement")]
    [SerializeField] private float _speed = 18f;
    [SerializeField] private float _rotationSpeed = 5f;
    [SerializeField] private float _waypointReachDistance = 5f;

    [Header("Stay Ahead")]
    [SerializeField] private float _minDistanceFromPlayer = 15f;
    [SerializeField] private float _maxDistanceFromPlayer = 40f;
    [SerializeField] private float _speedAdjustmentRate = 5f;

    [Header("Health")]
    [SerializeField] private float _maxHealth = 100f;

    [Header("Combat")]
    [SerializeField] private float _attackRange = 30f;
    [SerializeField] private float _fireRate = 0.5f;
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private AudioClip _fireSound;
    [SerializeField] private float _fireVolume = 0.8f;

    [Header("Death")]
    [SerializeField] private GameObject _explosionPrefab;
    [SerializeField] private AudioClip _explosionSound;
    [SerializeField] private float _explosionVolume = 1f;
    [SerializeField] private float _destroyDelay = 0.1f;

    [Header("Wheels")]
    [SerializeField] private float _wheelRotationSpeed = 360f;

    [Header("Hover Settings")]
    [SerializeField] private float _hoverHeight = 0.5f;
    [SerializeField] private float _hoverForce = 50f;
    [SerializeField] private float _hoverDamping = 5f;

    [Header("Ground Detection")]
    [SerializeField] private float _groundCheckDistance = 3f;
    [SerializeField] private LayerMask _groundMask = ~0;
    [SerializeField] private float _alignToGroundSpeed = 8f;

    [Header("Physics")]
    [SerializeField] private float _extraGravity = 20f;

    public float BaseSpeed => _speed;
    public float FireRate => _fireRate;
    public EnemyCarDrivingState DrivingState { get; private set; }
    public EnemyCarAttackingState AttackingState { get; private set; }
    public EnemyCarDeadState DeadState { get; private set; }

    private Rigidbody _rigidbody;
    private AudioSource _audioSource;
    private Transform[] _waypoints;
    private int _currentWaypointIndex;
    private float _currentSpeed;
    private float _targetSpeed;
    private Vector3 _targetDirection;
    private EnemyCarState _currentState;
    private bool _isGrounded;
    private Vector3 _groundNormal = Vector3.up;
    private float _groundDistance;
    private float _currentHealth;
    private bool _isDead;
    private bool _isInitialized;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        _currentSpeed = _speed;
        _targetSpeed = _speed;
        _targetDirection = transform.forward;
        _currentHealth = _maxHealth;

        SetupAudio();
        SetupStates();
    }

    private void Start()
    {
        if (!_isInitialized)
        {
            SetupWaypoints();
        }
        ChangeState(DrivingState);
    }

    public void Initialize(Transform playerCar, Transform waypointsParent, int startWaypointIndex)
    {
        _playerCar = playerCar;
        _waypointsParent = waypointsParent;
        _currentWaypointIndex = startWaypointIndex;

        SetupWaypoints();
        _isInitialized = true;
    }

    private void SetupAudio()
    {
        GameObject audioObj = new GameObject("EnemyCarAudio");
        audioObj.transform.SetParent(transform);
        audioObj.transform.localPosition = Vector3.zero;

        _audioSource = audioObj.AddComponent<AudioSource>();
        _audioSource.playOnAwake = false;
        _audioSource.spatialBlend = 1f;
        _audioSource.minDistance = 5f;
        _audioSource.maxDistance = 50f;
    }

    private void SetupStates()
    {
        DrivingState = new EnemyCarDrivingState();
        DrivingState.SetController(this);

        AttackingState = new EnemyCarAttackingState();
        AttackingState.SetController(this);

        DeadState = new EnemyCarDeadState();
        DeadState.SetController(this);
    }

    private void SetupWaypoints()
    {
        if (_waypointsParent == null)
        {
            Debug.LogError("[EnemyCarController] Waypoints Parent is not assigned!");
            return;
        }

        _waypoints = new Transform[_waypointsParent.childCount];
        for (int i = 0; i < _waypointsParent.childCount; i++)
        {
            _waypoints[i] = _waypointsParent.GetChild(i);
        }

        if (_waypoints.Length == 0)
        {
            Debug.LogError("[EnemyCarController] No waypoints found!");
        }
    }

    private void Update()
    {
        if (_isDead) return;
        _currentState?.Update();
        UpdateWheels();
    }

    private void FixedUpdate()
    {
        if (_isDead) return;
        if (_waypoints == null || _waypoints.Length < 2) return;

        CheckGround();
        ApplyHover();

        _currentState?.FixedUpdate();

        if (!_isGrounded)
        {
            ApplyExtraGravityInternal();
        }
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        _currentHealth -= damage;

        if (_currentHealth <= 0)
        {
            _currentHealth = 0;
            ChangeState(DeadState);
        }
    }

    public void OnDeath()
    {
        _isDead = true;

        if (_explosionPrefab != null)
        {
            Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
        }

        if (_explosionSound != null)
        {
            AudioSource.PlayClipAtPoint(_explosionSound, transform.position, _explosionVolume);
        }

        Destroy(gameObject, _destroyDelay);
    }

    private void CheckGround()
    {
        Ray ray = new Ray(transform.position + Vector3.up * 0.1f, Vector3.down);

        if (Physics.Raycast(ray, out RaycastHit hit, _groundCheckDistance, _groundMask))
        {
            _isGrounded = true;
            _groundNormal = hit.normal;
            _groundDistance = hit.distance - 0.1f;
        }
        else
        {
            _isGrounded = false;
            _groundNormal = Vector3.up;
            _groundDistance = _groundCheckDistance;
        }
    }

    private void ApplyHover()
    {
        if (!_isGrounded) return;

        float heightError = _hoverHeight - _groundDistance;
        float verticalVelocity = _rigidbody.velocity.y;

        float hoverAcceleration = (heightError * _hoverForce) - (verticalVelocity * _hoverDamping);

        _rigidbody.AddForce(Vector3.up * hoverAcceleration, ForceMode.Acceleration);
    }

    public void ChangeState(EnemyCarState newState)
    {
        _currentState?.Exit();
        _currentState = newState;
        _currentState?.Enter();
    }

    public void SetTargetSpeed(float speed)
    {
        _targetSpeed = speed;
    }

    public bool IsGrounded()
    {
        return _isGrounded;
    }

    public bool IsPlayerInAttackRange()
    {
        if (_playerCar == null) return false;
        float distance = Vector3.Distance(transform.position, _playerCar.position);
        return distance <= _attackRange;
    }

    public void UpdateWaypointProgress()
    {
        if (_waypoints == null || _waypoints.Length == 0) return;

        Transform currentWaypoint = _waypoints[_currentWaypointIndex];
        Vector3 toWaypoint = currentWaypoint.position - transform.position;
        toWaypoint.y = 0;

        if (toWaypoint.magnitude < _waypointReachDistance)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
        }
    }

    public void AdjustSpeedBasedOnPlayer()
    {
        if (_playerCar == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _playerCar.position);
        float speedMultiplier = 1f;

        if (distanceToPlayer < _minDistanceFromPlayer)
        {
            speedMultiplier = 1.3f;
        }
        else if (distanceToPlayer > _maxDistanceFromPlayer)
        {
            speedMultiplier = 0.7f;
        }

        float adjustedTarget = _targetSpeed * speedMultiplier;
        _currentSpeed = Mathf.Lerp(_currentSpeed, adjustedTarget, Time.fixedDeltaTime * _speedAdjustmentRate);
    }

    public void UpdateTargetDirection()
    {
        if (_waypoints == null || _waypoints.Length == 0) return;

        Transform targetWaypoint = _waypoints[_currentWaypointIndex];
        Vector3 direction = targetWaypoint.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            _targetDirection = direction.normalized;
        }
    }

    public void ApplyMovement()
    {
        if (!_isGrounded) return;

        Vector3 moveDirection = Vector3.ProjectOnPlane(transform.forward, _groundNormal).normalized;

        Vector3 targetVelocity = moveDirection * _currentSpeed;
        Vector3 currentHorizontalVelocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        Vector3 velocityDifference = new Vector3(targetVelocity.x, 0f, targetVelocity.z) - currentHorizontalVelocity;

        _rigidbody.AddForce(velocityDifference, ForceMode.VelocityChange);
    }

    public void ApplyRotation()
    {
        if (!_isGrounded) return;
        if (_targetDirection.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
        Quaternion newRotation = Quaternion.Slerp(
            _rigidbody.rotation,
            targetRotation,
            Time.fixedDeltaTime * _rotationSpeed
        );
        _rigidbody.MoveRotation(newRotation);

        AlignToGround();
    }

    private void AlignToGround()
    {
        Vector3 forward = Vector3.ProjectOnPlane(transform.forward, _groundNormal).normalized;

        if (forward.sqrMagnitude < 0.01f)
        {
            forward = transform.forward;
        }

        Quaternion targetRotation = Quaternion.LookRotation(forward, _groundNormal);

        float currentYaw = _rigidbody.rotation.eulerAngles.y;
        Quaternion yawOnly = Quaternion.Euler(targetRotation.eulerAngles.x, currentYaw, targetRotation.eulerAngles.z);

        Quaternion newRotation = Quaternion.Slerp(_rigidbody.rotation, yawOnly, Time.fixedDeltaTime * _alignToGroundSpeed);
        _rigidbody.MoveRotation(newRotation);
    }

    public void ApplyExtraGravity()
    {
    }

    private void ApplyExtraGravityInternal()
    {
        _rigidbody.AddForce(Vector3.down * _extraGravity, ForceMode.Acceleration);
    }

    public void Shoot()
    {
        if (_muzzleFlash != null)
        {
            _muzzleFlash.Play();
        }

        if (_audioSource != null && _fireSound != null)
        {
            _audioSource.pitch = Random.Range(0.95f, 1.05f);
            _audioSource.PlayOneShot(_fireSound, _fireVolume);
        }
    }

    private void UpdateWheels()
    {
        if (_wheels == null || _wheels.Length == 0) return;

        float rotationAmount = _currentSpeed * _wheelRotationSpeed * Time.deltaTime;

        foreach (Transform wheel in _wheels)
        {
            if (wheel != null)
            {
                wheel.Rotate(rotationAmount, 0f, 0f);
            }
        }
    }
}
