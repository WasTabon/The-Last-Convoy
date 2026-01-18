using System;
using UnityEngine;

public class EnemyHelicopterModel
{
    public event Action<Vector3> OnPositionChanged;
    public event Action<Quaternion> OnRotationChanged;

    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    public float CurrentSpeed { get; private set; }
    public int CurrentWaypointIndex { get; private set; }
    public float SpeedRatio => _config.CruiseSpeed > 0 ? CurrentSpeed / _config.CruiseSpeed : 0f;
    public float AngularVelocity => _angularVelocity;
    public float TurnIntensity => Mathf.Abs(_angularVelocity) / 30f;

    private readonly EnemyHelicopterConfig _config;
    private Vector3 _currentVelocity;
    private float _currentYaw;
    private float _currentPitch;
    private float _currentRoll;
    private float _targetRoll;
    private float _angularVelocity;
    private float _oscillationTime;

    public EnemyHelicopterModel(EnemyHelicopterConfig config)
    {
        _config = config;
    }

    public void Initialize(Vector3 startPosition, float startYaw)
    {
        Position = startPosition;
        _currentYaw = startYaw;
        Rotation = Quaternion.Euler(0, startYaw, 0);
    }

    public void Update(Vector3 targetWaypointPosition, float deltaTime)
    {
        if (deltaTime <= 0.0001f) return;

        UpdateSpeed(deltaTime);
        UpdateYawTowardsTarget(targetWaypointPosition, deltaTime);
        UpdatePitchAndRoll(deltaTime);
        UpdatePosition(deltaTime);
        UpdateOscillation(deltaTime);

        Rotation = Quaternion.Euler(_currentPitch, _currentYaw, _currentRoll);
        OnRotationChanged?.Invoke(Rotation);
    }

    public bool HasReachedWaypoint(Vector3 waypointPosition)
    {
        var toTarget = waypointPosition - Position;
        return toTarget.magnitude < _config.WaypointReachDistance;
    }

    public void MoveToNextWaypoint(int totalWaypoints)
    {
        CurrentWaypointIndex = (CurrentWaypointIndex + 1) % totalWaypoints;
    }

    private void UpdateSpeed(float deltaTime)
    {
        CurrentSpeed = Mathf.Lerp(CurrentSpeed, _config.CruiseSpeed, deltaTime * _config.Acceleration);
    }

    private void UpdateYawTowardsTarget(Vector3 targetPosition, float deltaTime)
    {
        var toTarget = targetPosition - Position;
        toTarget.y = 0;

        if (toTarget.sqrMagnitude < 0.01f) return;

        float targetYaw = Mathf.Atan2(toTarget.x, toTarget.z) * Mathf.Rad2Deg;
        float yawDifference = Mathf.DeltaAngle(_currentYaw, targetYaw);
        float maxYawChange = _config.YawSpeed * 60f * deltaTime;
        float yawChange = Mathf.Clamp(yawDifference * _config.YawSpeed * deltaTime * 2f, -maxYawChange, maxYawChange);

        float previousYaw = _currentYaw;
        _currentYaw += yawChange;

        _angularVelocity = Mathf.Lerp(_angularVelocity, (_currentYaw - previousYaw) / deltaTime, deltaTime * 3f);
    }

    private void UpdatePitchAndRoll(float deltaTime)
    {
        _targetRoll = Mathf.Clamp(_angularVelocity * 0.4f, -_config.MaxRollAngle, _config.MaxRollAngle);

        float rollSpeed = Mathf.Abs(_targetRoll) > Mathf.Abs(_currentRoll)
            ? _config.BankingSpeed
            : _config.BankReturnSpeed;

        _currentRoll = Mathf.Lerp(_currentRoll, _targetRoll, deltaTime * rollSpeed);

        float targetPitch = SpeedRatio * _config.MaxPitchAngle;
        _currentPitch = Mathf.Lerp(_currentPitch, targetPitch, deltaTime * _config.PitchSpeed);
    }

    private void UpdatePosition(float deltaTime)
    {
        Vector3 forward = Quaternion.Euler(0, _currentYaw, 0) * Vector3.forward;
        Vector3 targetVelocity = forward * CurrentSpeed;

        _currentVelocity = Vector3.Lerp(_currentVelocity, targetVelocity, deltaTime * _config.Acceleration);

        Position += _currentVelocity * deltaTime;
        OnPositionChanged?.Invoke(Position);
    }

    private void UpdateOscillation(float deltaTime)
    {
        _oscillationTime += deltaTime;

        float heightOffset = Mathf.Sin(_oscillationTime * _config.HeightOscillationSpeed) * _config.HeightOscillationAmount;
        heightOffset += Mathf.Sin(_oscillationTime * _config.HeightOscillationSpeed * 2.3f) * _config.HeightOscillationAmount * 0.3f;

        float swayX = Mathf.Sin(_oscillationTime * _config.SwaySpeed) * _config.SwayAmount;
        float swayZ = Mathf.Sin(_oscillationTime * _config.SwaySpeed * 0.7f + 1.5f) * _config.SwayAmount * 0.5f;

        Vector3 localOscillation = new Vector3(swayX, heightOffset, swayZ);
        Vector3 worldOscillation = Rotation * localOscillation;

        Position += worldOscillation * deltaTime;
    }
}
