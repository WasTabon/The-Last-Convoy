using UnityEngine;

public class MinigunController : MonoBehaviour
{
    [Header("Minigun Settings")]
    [SerializeField] private float spinUpTime = 1.5f;
    [SerializeField] private float fireRate = 0.1f;
    [SerializeField] private float maxBarrelRotationSpeed = 1800f;
    
    [Header("Barrel Reference")]
    [SerializeField] private Transform barrelTransform;
    
    [Header("Camera Shake")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float shakeIntensity = 0.08f;
    [SerializeField] private float shakeFrequency = 25f;
    [SerializeField] private float shakeRecoverySpeed = 15f;

    private float currentSpinProgress = 0f;
    private bool isSpinning = false;
    private bool isFiring = false;
    private float nextFireTime = 0f;
    private float currentBarrelSpeed = 0f;

    private Vector3 originalCameraPosition;
    private Vector3 currentShakeVelocity = Vector3.zero;
    private Vector3 currentShakeOffset = Vector3.zero;

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
    }

    void Update()
    {
        HandleInput();
        UpdateSpinState();
        RotateBarrel();
        HandleFiring();
        UpdateCameraShake();
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
    }

    void Fire()
    {
        float angle = Time.time * shakeFrequency;
        Vector3 shakeDirection = new Vector3(
            Mathf.Sin(angle) * shakeIntensity,
            Mathf.Cos(angle * 1.3f) * shakeIntensity,
            0f
        );

        currentShakeVelocity += shakeDirection;
    }

    void UpdateCameraShake()
    {
        if (cameraTransform == null) return;

        if (isFiring)
        {
            currentShakeOffset += currentShakeVelocity * Time.deltaTime;
            
            currentShakeVelocity = Vector3.Lerp(
                currentShakeVelocity,
                Vector3.zero,
                Time.deltaTime * shakeRecoverySpeed * 0.5f
            );
        }
        else
        {
            currentShakeVelocity = Vector3.Lerp(
                currentShakeVelocity,
                Vector3.zero,
                Time.deltaTime * shakeRecoverySpeed
            );
        }

        currentShakeOffset = Vector3.Lerp(
            currentShakeOffset,
            Vector3.zero,
            Time.deltaTime * shakeRecoverySpeed
        );

        currentShakeOffset.z = 0f;

        cameraTransform.localPosition = originalCameraPosition + currentShakeOffset;
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