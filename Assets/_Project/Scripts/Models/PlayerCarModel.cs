using System;
using UnityEngine;

public class PlayerCarModel
{
    public event Action<float> OnSpeedChanged;

    public float CurrentSpeed { get; private set; }
    public float CurrentTurnInput { get; private set; }
    public float SpeedRatio => Mathf.Abs(CurrentSpeed) / _config.MaxForwardSpeed;
    public bool IsMovingForward => CurrentSpeed > 0.1f;
    public bool IsMovingBackward => CurrentSpeed < -0.1f;
    public bool IsStationary => Mathf.Abs(CurrentSpeed) <= 0.1f;

    private readonly PlayerCarConfig _config;

    public PlayerCarModel(PlayerCarConfig config)
    {
        _config = config;
    }

    public void Initialize()
    {
        CurrentSpeed = 0f;
        CurrentTurnInput = 0f;
    }

    public void Update(float verticalInput, float horizontalInput, float deltaTime)
    {
        if (deltaTime <= 0.0001f) return;

        CurrentTurnInput = horizontalInput;
        UpdateSpeed(verticalInput, deltaTime);
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

    public float GetTurnAmount(float deltaTime)
    {
        if (Mathf.Abs(CurrentSpeed) < _config.MinSpeedToTurn) return 0f;
        if (Mathf.Abs(CurrentTurnInput) < 0.01f) return 0f;

        float speedFactor = 1f - (SpeedRatio * _config.TurnSpeedReduction);
        float turnAmount = CurrentTurnInput * _config.TurnSpeed * speedFactor * deltaTime;

        if (CurrentSpeed < 0)
        {
            turnAmount = -turnAmount;
        }

        return turnAmount;
    }
}
