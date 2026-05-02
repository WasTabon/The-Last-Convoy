using System;
using UnityEngine;

public class TurretModel
{
    public event Action<float, float> OnAnglesChanged;

    public float CurrentHorizontalAngle { get; private set; }
    public float CurrentVerticalAngle { get; private set; }
    public float TargetHorizontalAngle { get; private set; }
    public float TargetVerticalAngle { get; private set; }

    private readonly TurretConfig _config;

    public TurretModel(TurretConfig config)
    {
        _config = config;
    }

    public void Initialize()
    {
        CurrentHorizontalAngle = 0f;
        CurrentVerticalAngle = 0f;
        TargetHorizontalAngle = 0f;
        TargetVerticalAngle = 0f;
    }

    public void UpdateInput(float mouseX, float mouseY, float deltaTime)
    {
        TargetHorizontalAngle += mouseX * _config.HorizontalSpeed * deltaTime;
        TargetHorizontalAngle = Mathf.Clamp(TargetHorizontalAngle, _config.MinHorizontalAngle, _config.MaxHorizontalAngle);

        TargetVerticalAngle -= mouseY * _config.VerticalSpeed * deltaTime;
        TargetVerticalAngle = Mathf.Clamp(TargetVerticalAngle, _config.MinVerticalAngle, _config.MaxVerticalAngle);
    }

    public void UpdateRotation(float deltaTime)
    {
        float previousHorizontal = CurrentHorizontalAngle;
        float previousVertical = CurrentVerticalAngle;

        CurrentHorizontalAngle = Mathf.Lerp(CurrentHorizontalAngle, TargetHorizontalAngle, deltaTime * _config.RotationSmoothing);
        CurrentVerticalAngle = Mathf.Lerp(CurrentVerticalAngle, TargetVerticalAngle, deltaTime * _config.RotationSmoothing);

        if (Mathf.Abs(previousHorizontal - CurrentHorizontalAngle) > 0.01f ||
            Mathf.Abs(previousVertical - CurrentVerticalAngle) > 0.01f)
        {
            OnAnglesChanged?.Invoke(CurrentHorizontalAngle, CurrentVerticalAngle);
        }
    }

    public Vector3 GetAimDirection(Transform turretTransform)
    {
        Quaternion rotation = turretTransform.parent.rotation * Quaternion.Euler(CurrentVerticalAngle, CurrentHorizontalAngle, 0f);
        return rotation * Vector3.forward;
    }
}
