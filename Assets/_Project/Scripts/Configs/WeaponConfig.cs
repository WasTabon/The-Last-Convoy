using UnityEngine;

namespace LastConvoy.Configs
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "LastConvoy/Configs/Weapon")]
    public class WeaponConfig : ScriptableObject
    {
        [Header("Firing")]
        [SerializeField] private float spinUpTime = 1.5f;
        [SerializeField] private float fireRate = 0.1f;
        [SerializeField] private float maxBarrelRotationSpeed = 1800f;

        [Header("Raycast")]
        [SerializeField] private float raycastRange = 500f;
        [SerializeField] private LayerMask hitLayers = -1;

        [Header("Camera Shake")]
        [SerializeField] private float shakeIntensity = 0.04f;
        [SerializeField] private float shakeFrequency = 30f;

        [Header("Recoil")]
        [SerializeField] private float recoilRotationAmount = 2f;
        [SerializeField] private float recoilFrequency = 30f;
        [SerializeField] private float recoilRecoverySpeed = 8f;

        [Header("Audio")]
        [SerializeField] private AudioClip spinClip;
        [SerializeField] private AudioClip fireLoopClip;
        [SerializeField] private float baseFireVolume = 0.8f;
        [SerializeField] private float baseSpinVolume = 0.5f;
        [SerializeField] private float firePitchMin = 0.95f;
        [SerializeField] private float firePitchMax = 1.05f;
        [SerializeField] private float spinPitchMin = 0.5f;
        [SerializeField] private float spinPitchMax = 1.2f;
        [SerializeField] private float audioMaxDistance = 80f;
        [SerializeField] private float audioMinDistance = 3f;

        public float SpinUpTime => spinUpTime;
        public float FireRate => fireRate;
        public float MaxBarrelRotationSpeed => maxBarrelRotationSpeed;
        public float RaycastRange => raycastRange;
        public LayerMask HitLayers => hitLayers;
        public float ShakeIntensity => shakeIntensity;
        public float ShakeFrequency => shakeFrequency;
        public float RecoilRotationAmount => recoilRotationAmount;
        public float RecoilFrequency => recoilFrequency;
        public float RecoilRecoverySpeed => recoilRecoverySpeed;
        public AudioClip SpinClip => spinClip;
        public AudioClip FireLoopClip => fireLoopClip;
        public float BaseFireVolume => baseFireVolume;
        public float BaseSpinVolume => baseSpinVolume;
        public float FirePitchMin => firePitchMin;
        public float FirePitchMax => firePitchMax;
        public float SpinPitchMin => spinPitchMin;
        public float SpinPitchMax => spinPitchMax;
        public float AudioMaxDistance => audioMaxDistance;
        public float AudioMinDistance => audioMinDistance;
    }
}