using System;
using UnityEngine;

public class PlayerCarModel
{
    public event Action<Vector3> OnPositionChanged;
    public event Action<Quaternion> OnRotationChanged;
    public event Action<float> OnSpeedChanged;

    public Vector3 Position { get; private set; }
    public Quaternion Rotation { get; private set; }
    public float CurrentSpeed { get; private set; }
    public float SpeedRatio => Mathf.Abs(CurrentSpeed) / _config.MaxForwardSpeed;
    public bool IsMovingForward => CurrentSpeed > 0.1f;
    public bool IsMovingBackward => CurrentSpeed < -0.1f;
    public bool IsStationary => Mathf.Abs(CurrentSpeed) <= 0.1f;

    private readonly PlayerCarConfig _config;
    private float _currentYaw;

    public PlayerCarModel(PlayerCarConfig config)
    {
        _config = config;
    }

    public void Initialize(Vector3 startPosition, Quaternion startRotation)
    {
        Position = startPosition;
        Rotation = startRotation;
        _currentYaw = startRotation.eulerAngles.y;
        CurrentSpeed = 0f;
    }

    public void Update(float verticalInput, float horizontalInput, float deltaTime)
    {
        if (deltaTime <= 0.0001f) return;

        UpdateSpeed(verticalInput, deltaTime);
        UpdateRotation(horizontalInput, deltaTime);
        UpdatePosition(deltaTime);
    }

    private void UpdateSpeed(float verticalInput, float deltaTime)
    {
        float previousSpeed = CurrentSpeed;

        if (verticalInput > 0.01f)
        {
            if (CurrentSpeed < 0)
            {
                CurrentSpeed += _config.BrakeForce * deltaTime;
            }
            else
            {
                CurrentSpeed += _config.Acceleration * verticalInput * deltaTime;
                CurrentSpeed = Mathf.Min(CurrentSpeed, _config.MaxForwardSpeed);
            }
        }
        else if (verticalInput < -0.01f)
        {
            if (CurrentSpeed > 0)
            {
                CurrentSpeed -= _config.BrakeForce * deltaTime;
            }
            else
            {
                CurrentSpeed += _config.Acceleration * verticalInput * deltaTime;
                CurrentSpeed = Mathf.Max(CurrentSpeed, -_config.MaxReverseSpeed);
            }
        }
        else
        {
            if (CurrentSpeed > 0)
            {
                CurrentSpeed -= _config.Deceleration * deltaTime;
                CurrentSpeed = Mathf.Max(CurrentSpeed, 0f);
            }
            else if (CurrentSpeed < 0)
            {
                CurrentSpeed += _config.Deceleration * deltaTime;
                CurrentSpeed = Mathf.Min(CurrentSpeed, 0f);
            }
        }

        if (Mathf.Abs(previousSpeed - CurrentSpeed) > 0.01f)
        {
            OnSpeedChanged?.Invoke(CurrentSpeed);
        }
    }

    private void UpdateRotation(float horizontalInput, float deltaTime)
    {
        if (Mathf.Abs(CurrentSpeed) < _config.MinSpeedToTurn) return;
        if (Mathf.Abs(horizontalInput) < 0.01f) return;

        float speedFactor = 1f - (SpeedRatio * _config.TurnSpeedReduction);
        float turnAmount = horizontalInput * _config.TurnSpeed * speedFactor * deltaTime;

        if (CurrentSpeed < 0)
        {
            turnAmount = -turnAmount;
        }

        _currentYaw += turnAmount;
        Rotation = Quaternion.Euler(0f, _currentYaw, 0f);
        OnRotationChanged?.Invoke(Rotation);
    }

    private void UpdatePosition(float deltaTime)
    {
        if (Mathf.Abs(CurrentSpeed) < 0.01f) return;

        Vector3 forward = Rotation * Vector3.forward;
        Vector3 movement = forward * CurrentSpeed * deltaTime;

        Position += movement;
        OnPositionChanged?.Invoke(Position);
    }
}
