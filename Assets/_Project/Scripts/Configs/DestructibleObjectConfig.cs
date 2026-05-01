using UnityEngine;

[CreateAssetMenu(fileName = "DestructibleObjectConfig", menuName = "LastConvoy/Configs/DestructibleObject")]
public class DestructibleObjectConfig : ScriptableObject
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 20f;

    [Header("Explosion")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private AudioClip explosionClip;
    [SerializeField] private float explosionVolume = 0.8f;
    [SerializeField] private float audioSpatialBlend = 0.7f;
    [SerializeField] private float audioMinDistance = 5f;
    [SerializeField] private float audioMaxDistance = 100f;

    public float MaxHealth => maxHealth;
    public GameObject ExplosionPrefab => explosionPrefab;
    public AudioClip ExplosionClip => explosionClip;
    public float ExplosionVolume => explosionVolume;
    public float AudioSpatialBlend => audioSpatialBlend;
    public float AudioMinDistance => audioMinDistance;
    public float AudioMaxDistance => audioMaxDistance;
}
