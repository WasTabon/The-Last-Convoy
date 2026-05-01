using UnityEngine;

[CreateAssetMenu(fileName = "EnemyHelicopterConfig", menuName = "LastConvoy/Configs/EnemyHelicopter")]
public class EnemyHelicopterConfig : ScriptableObject
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 100f;

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

    [Header("Audio - Rotor")]
    [SerializeField] private AudioClip rotorLoopClip;
    [SerializeField] private float rotorBaseVolume = 0.7f;
    [SerializeField] private float rotorMinPitch = 0.9f;
    [SerializeField] private float rotorMaxPitch = 1.1f;

    [Header("Audio - 3D Settings")]
    [SerializeField] private float audioMinDistance = 5f;
    [SerializeField] private float audioMaxDistance = 150f;
    [SerializeField] private float audioSpatialBlend = 1f;
    [SerializeField] private float audioDopplerLevel = 0.3f;

    [Header("Audio - Filters")]
    [SerializeField] private float lowPassMinFrequency = 2000f;
    [SerializeField] private float lowPassMaxFrequency = 6000f;

    public float MaxHealth => maxHealth;

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

    public AudioClip RotorLoopClip => rotorLoopClip;
    public float RotorBaseVolume => rotorBaseVolume;
    public float RotorMinPitch => rotorMinPitch;
    public float RotorMaxPitch => rotorMaxPitch;
    public float AudioMinDistance => audioMinDistance;
    public float AudioMaxDistance => audioMaxDistance;
    public float AudioSpatialBlend => audioSpatialBlend;
    public float AudioDopplerLevel => audioDopplerLevel;
    public float LowPassMinFrequency => lowPassMinFrequency;
    public float LowPassMaxFrequency => lowPassMaxFrequency;
}
