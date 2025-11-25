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
        Debug.Log("FIRE!");
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