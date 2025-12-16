using UnityEngine;

namespace LastConvoy.Configs
{
    [CreateAssetMenu(fileName = "HelicopterConfig", menuName = "LastConvoy/Configs/Helicopter")]
    public class HelicopterConfig : ScriptableObject
    {
        [Header("Movement")]
        [SerializeField] private float cruiseSpeed = 20f;
        [SerializeField] private float acceleration = 3f;
        [SerializeField] private float waypointReachDistance = 8f;

        [Header("Turning")]
        [SerializeField] private float yawSpeed = 0.8f;
        [SerializeField] private float bankingSpeed = 1.5f;
        [SerializeField] private float bankReturnSpeed = 1f;

        [Header("Helicopter Feel")]
        [SerializeField] private float maxPitchAngle = 12f;
        [SerializeField] private float maxRollAngle = 20f;
        [SerializeField] private float pitchSpeed = 1.5f;

        [Header("Oscillation")]
        [SerializeField] private float heightOscillationAmount = 0.3f;
        [SerializeField] private float heightOscillationSpeed = 1.2f;
        [SerializeField] private float swayAmount = 0.15f;
        [SerializeField] private float swaySpeed = 0.8f;

        [Header("Blades")]
        [SerializeField] private float bladeRotationSpeed = 2000f;

        [Header("Audio")]
        [SerializeField] private AudioClip mainRotorLoop;
        [SerializeField] private AudioClip windLoop;
        [SerializeField] private float baseRotorVolume = 0.7f;
        [SerializeField] private float baseWindVolume = 0.3f;
        [SerializeField] private float windVolumeBySpeed = 0.5f;
        [SerializeField] private float rotorPitchMin = 0.85f;
        [SerializeField] private float rotorPitchMax = 1.15f;

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
        public AudioClip MainRotorLoop => mainRotorLoop;
        public AudioClip WindLoop => windLoop;
        public float BaseRotorVolume => baseRotorVolume;
        public float BaseWindVolume => baseWindVolume;
        public float WindVolumeBySpeed => windVolumeBySpeed;
        public float RotorPitchMin => rotorPitchMin;
        public float RotorPitchMax => rotorPitchMax;
    }
}