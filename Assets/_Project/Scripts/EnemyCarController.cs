using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class EnemyCarController : MonoBehaviour
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

    [Header("Wheels")]
    [SerializeField] private float _wheelRotationSpeed = 360f;

    [Header("Physics")]
    [SerializeField] private float _extraGravity = 30f;
    [SerializeField] private float _groundCheckDistance = 1.5f;
    [SerializeField] private LayerMask _groundMask = ~0;

    private Rigidbody _rigidbody;
    private Transform[] _waypoints;
    private int _currentWaypointIndex;
    private float _currentSpeed;
    private Vector3 _targetDirection;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
        _rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        _rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        _currentSpeed = _speed;
        _targetDirection = transform.forward;
    }

    private void Start()
    {
        SetupWaypoints();
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
        UpdateWheels();
    }

    private void FixedUpdate()
    {
        if (_waypoints == null || _waypoints.Length < 2) return;

        UpdateWaypointProgress();
        UpdateSpeed();
        UpdateTargetDirection();

        bool isGrounded = IsGrounded();

        if (isGrounded)
        {
            ApplyMovement();
            ApplyRotation();
        }

        ApplyExtraGravity();
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, _groundCheckDistance, _groundMask);
    }

    private void UpdateWaypointProgress()
    {
        Transform currentWaypoint = _waypoints[_currentWaypointIndex];
        Vector3 toWaypoint = currentWaypoint.position - transform.position;
        toWaypoint.y = 0;

        if (toWaypoint.magnitude < _waypointReachDistance)
        {
            _currentWaypointIndex = (_currentWaypointIndex + 1) % _waypoints.Length;
        }
    }

    private void UpdateSpeed()
    {
        if (_playerCar == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, _playerCar.position);
        float targetSpeed = _speed;

        if (distanceToPlayer < _minDistanceFromPlayer)
        {
            targetSpeed = _speed * 1.3f;
        }
        else if (distanceToPlayer > _maxDistanceFromPlayer)
        {
            targetSpeed = _speed * 0.7f;
        }

        _currentSpeed = Mathf.Lerp(_currentSpeed, targetSpeed, Time.fixedDeltaTime * _speedAdjustmentRate);
    }

    private void UpdateTargetDirection()
    {
        Transform targetWaypoint = _waypoints[_currentWaypointIndex];
        Vector3 direction = targetWaypoint.position - transform.position;
        direction.y = 0;

        if (direction.sqrMagnitude > 0.01f)
        {
            _targetDirection = direction.normalized;
        }
    }

    private void ApplyMovement()
    {
        Vector3 horizontalForward = transform.forward;
        horizontalForward.y = 0f;
        horizontalForward.Normalize();

        Vector3 targetHorizontalVelocity = horizontalForward * _currentSpeed;
        Vector3 currentHorizontalVelocity = new Vector3(_rigidbody.velocity.x, 0f, _rigidbody.velocity.z);
        Vector3 velocityDifference = targetHorizontalVelocity - currentHorizontalVelocity;

        _rigidbody.AddForce(velocityDifference, ForceMode.VelocityChange);
    }

    private void ApplyRotation()
    {
        if (_targetDirection.sqrMagnitude < 0.01f) return;

        Quaternion targetRotation = Quaternion.LookRotation(_targetDirection);
        Quaternion newRotation = Quaternion.Slerp(
            _rigidbody.rotation,
            targetRotation,
            Time.fixedDeltaTime * _rotationSpeed
        );
        _rigidbody.MoveRotation(newRotation);
    }

    private void ApplyExtraGravity()
    {
        _rigidbody.AddForce(Vector3.down * _extraGravity, ForceMode.Acceleration);
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