using UnityEngine;

[CreateAssetMenu(fileName = "EnemyHelicopterConfig", menuName = "LastConvoy/Configs/EnemyHelicopter")]
public class EnemyHelicopterConfig : ScriptableObject
{
    [Header("Movement")]
    [SerializeField] private float cruiseSpeed = 15f;
    [SerializeField] private float acceleration = 2f;
    [SerializeField] private float waypointReachDistance = 10f;

    [Header("Turning")]
    [SerializeField] private float yawSpeed = 0.6f;
    [SerializeField] private float bankingSpeed = 1.2f;
    [SerializeField] private float bankReturnSpeed = 0.8f;

    [Header("Helicopter Feel")]
    [SerializeField] private float maxPitchAngle = 10f;
    [SerializeField] private float maxRollAngle = 18f;
    [SerializeField] private float pitchSpeed = 1.2f;

    [Header("Oscillation")]
    [SerializeField] private float heightOscillationAmount = 0.25f;
    [SerializeField] private float heightOscillationSpeed = 1f;
    [SerializeField] private float swayAmount = 0.12f;
    [SerializeField] private float swaySpeed = 0.7f;

    [Header("Blades")]
    [SerializeField] private float bladeRotationSpeed = 1800f;

    public float CruiseSpeed => cruiseSpeed;
    public float Acceleration => acceleration;
    public float WaypointReachDistance => waypointReachDistance;
    public float YawSpeed => yawSpeed;
    public float BankingSpeed => bankingSpeed;
    public float BankReturnSpeed => bankReturnSpeed;
    public float MaxPitchAngle => maxPitchAngle;
    public float MaxRollAngle => maxRollAngle;
    public float PitchSpeed => pitchSpeed;
    public float HeightOscillationAmount => heightOscillationAmount;
    public float HeightOscillationSpeed => heightOscillationSpeed;
    public float SwayAmount => swayAmount;
    public float SwaySpeed => swaySpeed;
    public float BladeRotationSpeed => bladeRotationSpeed;
}
