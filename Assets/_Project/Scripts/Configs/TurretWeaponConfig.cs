using UnityEngine;

[CreateAssetMenu(fileName = "TurretWeaponConfig", menuName = "LastConvoy/Configs/TurretWeapon")]
public class TurretWeaponConfig : ScriptableObject
{
    [Header("Firing")]
    [SerializeField] private float fireRate = 0.2f;
    [SerializeField] private float damagePerShot = 10f;
    [SerializeField] private float raycastRange = 300f;
    [SerializeField] private LayerMask hitLayers = -1;

    [Header("Audio")]
    [SerializeField] private AudioClip fireClip;
    [SerializeField] private float fireVolume = 0.8f;
    [SerializeField] private float firePitchMin = 0.95f;
    [SerializeField] private float firePitchMax = 1.05f;

    [Header("Crosshair")]
    [SerializeField] private Color crosshairColor = Color.white;
    [SerializeField] private float crosshairSize = 20f;
    [SerializeField] private float crosshairThickness = 2f;
    [SerializeField] private float crosshairGap = 8f;

    public float FireRate => fireRate;
    public float DamagePerShot => damagePerShot;
    public float RaycastRange => raycastRange;
    public LayerMask HitLayers => hitLayers;

    public AudioClip FireClip => fireClip;
    public float FireVolume => fireVolume;
    public float FirePitchMin => firePitchMin;
    public float FirePitchMax => firePitchMax;

    public Color CrosshairColor => crosshairColor;
    public float CrosshairSize => crosshairSize;
    public float CrosshairThickness => crosshairThickness;
    public float CrosshairGap => crosshairGap;
}
