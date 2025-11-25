using System.Collections.Generic;
using UnityEngine;

public class MinigunController : MonoBehaviour
{
    [SerializeField] private ParticleSystem _gunShots;
    [SerializeField] private ParticleSystem _bulletShells;
    
    [Header("Minigun Settings")]
    [SerializeField] private float spinUpTime = 1.5f;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float maxBarrelRotationSpeed = 1800f;
    
    [Header("Barrel Reference")]
    [SerializeField] private Transform barrelTransform;
    
    [Header("Minigun Transform")]
    [SerializeField] private Transform minigunTransform;
    
    [Header("Camera Reference")]
    [SerializeField] private Transform cameraTransform;
    
    [Header("Camera Shake")]
    [SerializeField] private float shakeIntensity = 0.04f;
    [SerializeField] private float shakeFrequency = 30f;
    
    [Header("Recoil")]
    [SerializeField] private float recoilRotationAmount = 2f;
    [SerializeField] private float recoilFrequency = 30f;
    [SerializeField] private float recoilRecoverySpeed = 8f;

    [Header("Audio - Minigun")]
    [SerializeField] private AudioClip minigunSpinClip;
    [SerializeField] private AudioClip minigunFireLoopClip;
    
    [Header("Audio Settings")]
    [SerializeField] private float baseFireVolume = 0.8f;
    [SerializeField] private float baseSpinVolume = 0.5f;
    [SerializeField] private float firePitchMin = 0.95f;
    [SerializeField] private float firePitchMax = 1.05f;
    [SerializeField] private float spinPitchMin = 0.5f;
    [SerializeField] private float spinPitchMax = 1.2f;
    [SerializeField] private float maxDistance = 80f;
    [SerializeField] private float minDistance = 3f;
    
    [Header("Audio Positioning")]
    [SerializeField] private Transform muzzlePosition;

    [Header("Raycast Settings")]
    [SerializeField] private float raycastRange = 500f;
    [SerializeField] private Transform raycastOrigin;
    [SerializeField] private LayerMask hitLayers = -1;
    
    [Header("Impact Effects")]
    [SerializeField] private BulletImpactPool impactPool;

    private float currentSpinProgress = 0f;
    private bool isSpinning = false;
    private bool isFiring = false;
    private float nextFireTime = 0f;
    private float currentBarrelSpeed = 0f;

    private Vector3 originalCameraPosition;
    private Vector3 cameraShakeOffset = Vector3.zero;
    private float shakePhase = 0f;
    
    private Vector3 minigunRotationRecoil = Vector3.zero;
    private float recoilPhase = 0f;

    // Audio sources
    private AudioSource spinSource;
    private AudioSource fireSource;
    
    // Audio filters
    private AudioLowPassFilter fireLowPass;
    private AudioHighPassFilter fireHighPass;
    private AudioDistortionFilter fireDistortion;
    private AudioReverbFilter fireReverb;
    private AudioEchoFilter fireEcho;
    
    // Ignored transforms for raycast
    private Transform rootTransform;
    private HashSet<Transform> ignoredTransforms = new HashSet<Transform>();

    void Start()
    {
        if (cameraTransform != null)
        {
            originalCameraPosition = cameraTransform.localPosition;
        }

        if (barrelTransform == null)
        {
            Debug.LogWarning("Barrel Transform not assigned!");
        }
        
        if (minigunTransform == null)
        {
            Debug.LogWarning("Minigun Transform not assigned!");
        }

        SetupAudio();
        SetupIgnoredTransforms();
    }

    void Update()
    {
        HandleInput();
        UpdateSpinState();
        RotateBarrel();
        HandleFiring();
        UpdateCameraShake();
        UpdateRecoil();
        UpdateMinigunRotation();
        UpdateAudio();
    }

    void SetupAudio()
    {
        // Spin sound (motor spinning up/down)
        spinSource = CreateAudioSource("Spin", muzzlePosition);
        spinSource.spatialBlend = 0.8f;
        spinSource.volume = 0f;
        spinSource.spread = 90f;
        spinSource.minDistance = minDistance;
        spinSource.maxDistance = maxDistance;
        spinSource.clip = minigunSpinClip;
        spinSource.loop = true;

        // Fire loop sound
        fireSource = CreateAudioSource("Fire", muzzlePosition);
        ConfigureFireAudio();
    }

    AudioSource CreateAudioSource(string name, Transform position)
    {
        GameObject audioObj = new GameObject($"Audio_{name}");
        
        if (position != null)
        {
            audioObj.transform.SetParent(position);
            audioObj.transform.localPosition = Vector3.zero;
        }
        else
        {
            audioObj.transform.SetParent(transform);
            audioObj.transform.localPosition = Vector3.zero;
        }
        
        AudioSource source = audioObj.AddComponent<AudioSource>();
        source.playOnAwake = false;
        source.loop = true;
        source.spatialBlend = 1f;
        source.dopplerLevel = 0.5f;
        source.rolloffMode = AudioRolloffMode.Custom;
        source.priority = 64;
        
        return source;
    }

    void ConfigureFireAudio()
    {
        if (fireSource == null) return;
        
        fireSource.clip = minigunFireLoopClip;
        fireSource.volume = baseFireVolume;
        fireSource.spatialBlend = 0.9f;
        fireSource.spread = 80f;
        fireSource.minDistance = minDistance;
        fireSource.maxDistance = maxDistance;
        fireSource.priority = 32;
        
        // Low-pass filter for distance muffling
        fireLowPass = fireSource.gameObject.AddComponent<AudioLowPassFilter>();
        fireLowPass.cutoffFrequency = 8000f;
        fireLowPass.lowpassResonanceQ = 1.0f;
        
        // High-pass filter for punch
        fireHighPass = fireSource.gameObject.AddComponent<AudioHighPassFilter>();
        fireHighPass.cutoffFrequency = 150f;
        fireHighPass.highpassResonanceQ = 1.0f;
        
        // Distortion for aggressive sound
        fireDistortion = fireSource.gameObject.AddComponent<AudioDistortionFilter>();
        fireDistortion.distortionLevel = 0.15f;
        
        // Reverb for environmental feel
        fireReverb = fireSource.gameObject.AddComponent<AudioReverbFilter>();
        fireReverb.reverbPreset = AudioReverbPreset.Plain;
        fireReverb.dryLevel = 200f;
        fireReverb.room = -500f;
        fireReverb.roomHF = -200f;
        fireReverb.decayTime = 0.4f;
        fireReverb.decayHFRatio = 0.5f;
        fireReverb.reflectionsLevel = -800;
        fireReverb.reflectionsDelay = 0.002f;
        fireReverb.reverbLevel = -400;
        fireReverb.reverbDelay = 0.008f;
        fireReverb.diffusion = 100f;
        fireReverb.density = 100f;
        
        // Echo for mechanical feedback
        fireEcho = fireSource.gameObject.AddComponent<AudioEchoFilter>();
        fireEcho.delay = 30f;
        fireEcho.decayRatio = 0.15f;
        fireEcho.wetMix = 0.08f;
        fireEcho.dryMix = 1.0f;
    }

    void UpdateAudio()
    {
        // Spin sound (motor) - plays only when barrel is spinning but NOT firing
        if (spinSource != null && minigunSpinClip != null)
        {
            bool shouldPlaySpin = currentSpinProgress > 0.01f && !isFiring;
            
            // Start playing spin sound if barrel is rotating and not firing
            if (shouldPlaySpin && !spinSource.isPlaying)
            {
                spinSource.Play();
            }
            // Stop spin sound when firing starts or barrel stops
            else if (!shouldPlaySpin && spinSource.isPlaying)
            {
                spinSource.Stop();
            }
            
            // Only adjust pitch/volume when spin sound should be playing
            if (shouldPlaySpin)
            {
                // Smoothly adjust pitch based on spin progress
                float targetPitch = Mathf.Lerp(spinPitchMin, spinPitchMax, currentSpinProgress);
                spinSource.pitch = Mathf.Lerp(spinSource.pitch, targetPitch, Time.deltaTime * 5f);
                
                // Smoothly adjust volume based on spin progress
                float targetVolume = baseSpinVolume * Mathf.Clamp01(currentSpinProgress);
                spinSource.volume = Mathf.Lerp(spinSource.volume, targetVolume, Time.deltaTime * 3f);
            }
        }

        // Fire sound - plays only when actually firing (100% spin)
        if (fireSource != null && minigunFireLoopClip != null)
        {
            if (isFiring && !fireSource.isPlaying)
            {
                fireSource.Play();
            }
            else if (!isFiring && fireSource.isPlaying)
            {
                fireSource.Stop();
            }

            if (isFiring)
            {
                // Varying pitch for realism
                fireSource.pitch = Random.Range(firePitchMin, firePitchMax);
                fireSource.volume = baseFireVolume + Random.Range(-0.05f, 0.05f);
                
                // Dynamic filter adjustment
                if (fireLowPass != null)
                {
                    fireLowPass.cutoffFrequency = Mathf.Lerp(
                        fireLowPass.cutoffFrequency,
                        7000f + Random.Range(-500f, 500f),
                        Time.deltaTime * 10f
                    );
                }
                
                if (fireDistortion != null)
                {
                    fireDistortion.distortionLevel = 0.15f + Random.Range(0f, 0.05f);
                }
            }
        }
    }

    void HandleInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            isSpinning = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            isSpinning = false;
            isFiring = false;
        }
    }

    void UpdateSpinState()
    {
        if (isSpinning)
        {
            currentSpinProgress += Time.deltaTime / spinUpTime;
            currentSpinProgress = Mathf.Clamp01(currentSpinProgress);

            if (currentSpinProgress >= 1f && !isFiring)
            {
                isFiring = true;
            }
        }
        else
        {
            currentSpinProgress -= Time.deltaTime / (spinUpTime * 0.5f);
            currentSpinProgress = Mathf.Clamp01(currentSpinProgress);
        }
    }

    void RotateBarrel()
    {
        if (barrelTransform == null) return;

        currentBarrelSpeed = Mathf.Lerp(
            currentBarrelSpeed,
            currentSpinProgress * maxBarrelRotationSpeed,
            Time.deltaTime * 5f
        );

        barrelTransform.Rotate(0f, 0f, -currentBarrelSpeed * Time.deltaTime);
    }

    void HandleFiring()
    {
        if (isFiring && Time.time >= nextFireTime)
        {
            Fire();
            nextFireTime = Time.time + fireRate;
        }

        if (isFiring && !_gunShots.isPlaying)
        {
            _gunShots.Play();
            _bulletShells.Play();
        }
        else if (!isFiring && _gunShots.isPlaying)
        {
            _gunShots.Stop();
            _bulletShells.Stop();
        }
    }

    void Fire()
    {
        // Raycast or projectile logic here
        PerformRaycast();
    }
    
    void SetupIgnoredTransforms()
    {
        // Find root transform (highest parent)
        rootTransform = transform;
        while (rootTransform.parent != null)
        {
            rootTransform = rootTransform.parent;
        }
        
        // Add all children of root to ignored list
        AddAllChildren(rootTransform);
        
        Debug.Log($"Ignoring {ignoredTransforms.Count} transforms for raycast");
    }
    
    void AddAllChildren(Transform parent)
    {
        ignoredTransforms.Add(parent);
        
        foreach (Transform child in parent)
        {
            AddAllChildren(child);
        }
    }
    
    void PerformRaycast()
    {
        if (impactPool == null)
        {
            Debug.LogWarning("Impact Pool is not assigned!");
            return;
        }
        
        // Determine raycast origin
        Vector3 origin = raycastOrigin != null ? raycastOrigin.position : transform.position;
        Vector3 direction = raycastOrigin != null ? raycastOrigin.forward : transform.forward;
        
        // Perform raycast
        RaycastHit hit;
        if (Physics.Raycast(origin, direction, out hit, raycastRange, hitLayers))
        {
            // Check if hit object should be ignored
            if (ShouldIgnoreHit(hit.transform))
            {
                // Hit own object, try raycast again from hit point
                Vector3 newOrigin = hit.point + direction * 0.1f;
                if (Physics.Raycast(newOrigin, direction, out hit, raycastRange, hitLayers))
                {
                    if (!ShouldIgnoreHit(hit.transform))
                    {
                        PlayImpactEffect(hit);
                    }
                }
            }
            else
            {
                PlayImpactEffect(hit);
            }
        }
    }
    
    bool ShouldIgnoreHit(Transform hitTransform)
    {
        // Check if hit transform or any of its parents are in ignored list
        Transform current = hitTransform;
        while (current != null)
        {
            if (ignoredTransforms.Contains(current))
            {
                return true;
            }
            current = current.parent;
        }
        return false;
    }
    
    void PlayImpactEffect(RaycastHit hit)
    {
        // Play impact effect at hit point with surface normal
        impactPool.PlayImpactEffect(hit.point, hit.normal);
    }

    void UpdateCameraShake()
    {
        if (cameraTransform == null) return;

        if (isFiring)
        {
            shakePhase += Time.deltaTime * shakeFrequency;
            
            cameraShakeOffset.x = Mathf.PerlinNoise(shakePhase, 0f) * 2f - 1f;
            cameraShakeOffset.y = Mathf.PerlinNoise(0f, shakePhase) * 2f - 1f;
            cameraShakeOffset.z = 0f;
            
            cameraShakeOffset *= shakeIntensity;
        }
        else
        {
            cameraShakeOffset = Vector3.Lerp(
                cameraShakeOffset,
                Vector3.zero,
                Time.deltaTime * 15f
            );
        }

        cameraTransform.localPosition = originalCameraPosition + cameraShakeOffset;
    }

    void UpdateRecoil()
    {
        if (minigunTransform == null) return;

        if (isFiring)
        {
            recoilPhase += Time.deltaTime * recoilFrequency;
            
            float rotX = Mathf.Sin(recoilPhase * 1.5f) * recoilRotationAmount;
            float rotY = Mathf.Cos(recoilPhase * 1.2f) * recoilRotationAmount * 0.8f;
            
            minigunRotationRecoil = Vector3.Lerp(
                minigunRotationRecoil,
                new Vector3(rotX, rotY, 0f),
                Time.deltaTime * 20f
            );
        }
        else
        {
            minigunRotationRecoil = Vector3.Lerp(
                minigunRotationRecoil,
                Vector3.zero,
                Time.deltaTime * recoilRecoverySpeed
            );
        }
    }

    void UpdateMinigunRotation()
    {
        if (minigunTransform == null || cameraTransform == null) return;

        Quaternion targetRotation = cameraTransform.rotation * 
                                   Quaternion.Euler(minigunRotationRecoil.x, minigunRotationRecoil.y, 0f);
        
        minigunTransform.rotation = Quaternion.Slerp(
            minigunTransform.rotation,
            targetRotation,
            Time.deltaTime * 15f
        );
    }

    void OnDestroy()
    {
        // Cleanup is automatic with GameObject destruction
    }

    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 20), $"Spin: {(currentSpinProgress * 100):F0}%");
        GUI.Label(new Rect(10, 30, 300, 20), $"Barrel Speed: {currentBarrelSpeed:F0}°/s");
        if (isFiring)
        {
            GUI.Label(new Rect(10, 50, 300, 20), "FIRING!");
        }
    }
}