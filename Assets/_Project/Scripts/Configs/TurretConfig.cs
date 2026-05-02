using UnityEngine;

[CreateAssetMenu(fileName = "TurretConfig", menuName = "LastConvoy/Configs/Turret")]
public class TurretConfig : ScriptableObject
{
    [Header("Rotation Speed")]
    [SerializeField] private float horizontalSpeed = 100f;
    [SerializeField] private float verticalSpeed = 80f;

    [Header("Horizontal Limits")]
    [SerializeField] private float minHorizontalAngle = -90f;
    [SerializeField] private float maxHorizontalAngle = 90f;

    [Header("Vertical Limits")]
    [SerializeField] private float minVerticalAngle = -15f;
    [SerializeField] private float maxVerticalAngle = 30f;

    [Header("Smoothing")]
    [SerializeField] private float rotationSmoothing = 10f;

    public float HorizontalSpeed => horizontalSpeed;
    public float VerticalSpeed => verticalSpeed;
    public float MinHorizontalAngle => minHorizontalAngle;
    public float MaxHorizontalAngle => maxHorizontalAngle;
    public float MinVerticalAngle => minVerticalAngle;
    public float MaxVerticalAngle => maxVerticalAngle;
    public float RotationSmoothing => rotationSmoothing;
}
